/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * The IMU algorithms used for sensor fusion has been developed by Sebastian Madgwick
 * and is available under the GNU GLP license at http://www.x-io.co.uk/open-source-imu-and-ahrs-algorithms/
 */

#include <jni.h>
#include <android_native_app_glue.h>
#include <errno.h>
#include <math.h>
#include <android/asset_manager.h>
#include <android/sensor.h>
#include <android/looper.h>
#include <android/log.h>

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "native_activity", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "native_activity", __VA_ARGS__))
#define ASENSOR_TYPE_ROTATION_VECTOR 11
#define sampleFreq	128.0f			// sample frequency in Hz
#define twoKpDef	(2.0f * 0.5f)	// 2 * proportional gain
#define twoKiDef	(2.0f * 0.0f)	// 2 * integral gain
#define betaDef		0.1f	    	// 2 * proportional gain

typedef struct Quaternion{
    float x;
    float y;
    float z;
    float w;
}Quaternion;

void MahonyAHRSupdateIMU(float gx, float gy, float gz, float ax, float ay, float az,int64_t ev_timestamp);
float invSqrt(float x);
void multiplyQuat(Quaternion* q1, Quaternion* q2);

//=====================================================================================================
// Attributes
//=====================================================================================================
volatile float beta = betaDef;	
volatile float twoKp = twoKpDef;											// 2 * proportional gain (Kp)
volatile float twoKi = twoKiDef;											// 2 * integral gain (Ki)
volatile float q0 = 1.0f, q1 = 0.0f, q2 = 0.0f, q3 = 0.0f;					// quaternion of sensor frame relative to auxiliary frame
volatile float integralFBx = 0.0f,  integralFBy = 0.0f, integralFBz = 0.0f;	// integral error terms scaled by Ki
static float acc_x=555,acc_y=555,acc_z=555; 
static float gyr_x=999,gyr_y=999,gyr_z=999; 
static int64_t time_stamp=-1;
static int64_t gyro_time_stamp=-1;
static ASensorEventQueue* sensorEventQueueGyro;
static ASensorEventQueue* sensorEventQueueAcc;
static float N2S = 1.0/1000000000.0;
static float EPSILON = 0.000000001;
static Quaternion deltaGyroQuaternion={0,0,0,0};
static Quaternion q={0,0,0,1};

//=====================================================================================================
// Functions
//=====================================================================================================
/* Getter function used Unity.
 * Returns references to the orientation quaternion values.
 */
void get_q(float* _x, float* _y, float* _z, float* _w){
    q.x = q1;
    q.y = q2;
    q.z = q3;
    q.w = q0;
    multiplyQuat(&q,&deltaGyroQuaternion);
    *_x = q.x;
    *_y = q.y;
    *_z = q.z;
    *_w = q.w;
}

/* Intergrates raw gyroscope data over time.
 * Returns a quaternion corresponding to the device orientation.
 */
void getQuaternionFromGyro(float ev_x,float ev_y,float ev_z,int64_t ev_timestamp){
    if(gyro_time_stamp != -1){
        float dT = (ev_timestamp - gyro_time_stamp) * N2S;
        //Calculate the angular speed of the sample
        float omegaMagnitude = sqrt(ev_x*ev_x + ev_y*ev_y + ev_z*ev_z);

        //Normalize the rotation vector
        if(omegaMagnitude > EPSILON){
            ev_x /= omegaMagnitude;
            ev_y /= omegaMagnitude;
            ev_z /= omegaMagnitude;
        }


        float thetaOverTwo = omegaMagnitude * dT / 2.0f;
        float sinThetaOverTwo = sin(thetaOverTwo);
        float cosThetaOverTwo = cos(thetaOverTwo);
        deltaGyroQuaternion.x = sinThetaOverTwo * ev_x;
        deltaGyroQuaternion.y = sinThetaOverTwo * ev_y;
        deltaGyroQuaternion.z = sinThetaOverTwo * ev_z;
        deltaGyroQuaternion.w = cosThetaOverTwo;

        multiplyQuat(&deltaGyroQuaternion,&deltaGyroQuaternion);
        multiplyQuat(&deltaGyroQuaternion,&deltaGyroQuaternion);
        multiplyQuat(&deltaGyroQuaternion,&deltaGyroQuaternion);
    }
    gyro_time_stamp = ev_timestamp;

}

/* Loop function to process gyroscope data
 */
int get_sensor_events_gyro(int fd, int events, void* data) {
    ASensorEvent event;

    while (ASensorEventQueue_getEvents(sensorEventQueueGyro, &event, 1) > 0) {
        if(event.type==ASENSOR_TYPE_GYROSCOPE){
            gyr_x = event.vector.x;
            gyr_y = event.vector.y;
            gyr_z = event.vector.z;

            getQuaternionFromGyro(gyr_x,gyr_y,gyr_z,event.timestamp);
        }
    }
    //should return 1 to continue receiving callbacks, or 0 to unregister                                                                                                                           
    return 1;
}

/* Loop function to process accelerometer data
 */
int get_sensor_events_acc(int fd, int events, void* data) {
    ASensorEvent event;

    while (ASensorEventQueue_getEvents(sensorEventQueueAcc, &event, 1) > 0) {
        if(event.type==ASENSOR_TYPE_ACCELEROMETER) { 
			acc_x = event.acceleration.x;
			acc_y = event.acceleration.y;
			acc_z = event.acceleration.z;
            MahonyAHRSupdateIMU(gyr_x,gyr_y,gyr_z,acc_x,acc_y,acc_z,event.timestamp);
            //MadgwickAHRSupdateIMU(gyr_x,gyr_y,gyr_z,acc_x,acc_y,acc_z,event.timestamp);
		}
    }
    //should return 1 to continue receiving callbacks, or 0 to unregister                                                                                                                           
    return 1;
}

/* Main function
 */
void android_main(struct android_app* state) { 
    app_dummy();
    AAssetManager* assetManager = state->activity->assetManager; 
    AConfiguration* config;
    AConfiguration_fromAssetManager(config,assetManager);
    AConfiguration_setNavHidden(config,ACONFIGURATION_NAVHIDDEN_YES);
    AConfiguration_setNavigation(config,ACONFIGURATION_NAVIGATION_NONAV );
}

/* Initialization function.
 * Creates the sensor managers, loops and event queues.
 */
void init(){
    app_dummy();

    void* sensor_data_gyro;
    void* sensor_data_acc;
    //Create a Looper for this thread
    ALooper* looper_gyro = ALooper_forThread();
    if(looper_gyro == NULL){
        looper_gyro = ALooper_prepare(ALOOPER_PREPARE_ALLOW_NON_CALLBACKS);
    }
    
    ALooper* looper_acc = ALooper_forThread();
    if(looper_acc == NULL){
        looper_acc = ALooper_prepare(ALOOPER_PREPARE_ALLOW_NON_CALLBACKS);
    }

    //Get an instance of SensorManager
    ASensorManager* sensorManagerGyro 		= ASensorManager_getInstance();
     ASensorManager* sensorManagerAcc 		= ASensorManager_getInstance();
   
    ASensor const* accelerometerSensor 	= ASensorManager_getDefaultSensor(sensorManagerGyro, ASENSOR_TYPE_ACCELEROMETER);
    ASensor const* gyroscopeSensor 		= ASensorManager_getDefaultSensor(sensorManagerAcc, ASENSOR_TYPE_GYROSCOPE);
    
    //Create a sensor event queue
    sensorEventQueueGyro = ASensorManager_createEventQueue(sensorManagerGyro, looper_gyro,  3, get_sensor_events_gyro, sensor_data_gyro);
    sensorEventQueueAcc  = ASensorManager_createEventQueue(sensorManagerAcc, looper_acc,  3, get_sensor_events_acc, sensor_data_acc);

    if(gyroscopeSensor != NULL){
        ASensorEventQueue_enableSensor(sensorEventQueueGyro, gyroscopeSensor);
        ASensorEventQueue_setEventRate(sensorEventQueueGyro, gyroscopeSensor,(1000L/sampleFreq)*1000); 
    }
    if(accelerometerSensor != NULL){
		ASensorEventQueue_enableSensor(sensorEventQueueAcc, accelerometerSensor);
		ASensorEventQueue_setEventRate(sensorEventQueueAcc, accelerometerSensor,(1000L/sampleFreq)*1000);
	}
}

/* IMU algorithm update. Mahony implementation.
 */
void MahonyAHRSupdateIMU(float gx, float gy, float gz, float ax, float ay, float az,int64_t ev_timestamp) {
	if(time_stamp != -1){
        float dT = (ev_timestamp - time_stamp) * N2S;
        float recipNorm;
        float halfvx, halfvy, halfvz;
        float halfex, halfey, halfez;
        float qa, qb, qc;

        // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
        if(!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f))) {

            // Normalise accelerometer measurement
            recipNorm = invSqrt(ax * ax + ay * ay + az * az);
            ax *= recipNorm;
            ay *= recipNorm;
            az *= recipNorm;        

            // Estimated direction of gravity and vector perpendicular to magnetic flux
            halfvx = q1 * q3 - q0 * q2;
            halfvy = q0 * q1 + q2 * q3;
            halfvz = q0 * q0 - 0.5f + q3 * q3;

            // Error is sum of cross product between estimated and measured direction of gravity
            halfex = (ay * halfvz - az * halfvy);
            halfey = (az * halfvx - ax * halfvz);
            halfez = (ax * halfvy - ay * halfvx);

            // Compute and apply integral feedback if enabled
            if(twoKi > 0.0f) {
                integralFBx += twoKi * halfex * dT/*(1.0f / sampleFreq)*/;	// integral error scaled by Ki
                integralFBy += twoKi * halfey * dT/*(1.0f / sampleFreq)*/;
                integralFBz += twoKi * halfez * dT/*(1.0f / sampleFreq)*/;
                gx += integralFBx;	// apply integral feedback
                gy += integralFBy;
                gz += integralFBz;
            }
            else {
                integralFBx = 0.0f;	// prevent integral windup
                integralFBy = 0.0f;
                integralFBz = 0.0f;
            }

            // Apply proportional feedback
            gx += twoKp * halfex;
            gy += twoKp * halfey;
            gz += twoKp * halfez;
        }

        // Integrate rate of change of quaternion
        gx *= (0.5f * dT/*(1.0f / sampleFreq)*/);		// pre-multiply common factors
        gy *= (0.5f * dT/*(1.0f / sampleFreq)*/);
        gz *= (0.5f * dT/*(1.0f / sampleFreq)*/);
        qa = q0;
        qb = q1;
        qc = q2;
        q0 += (-qb * gx - qc * gy - q3 * gz);
        q1 += (qa * gx + qc * gz - q3 * gy);
        q2 += (qa * gy - qb * gz + q3 * gx);
        q3 += (qa * gz + qb * gy - qc * gx); 

        // Normalise quaternion
        recipNorm = invSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
        q0 *= recipNorm;
        q1 *= recipNorm;
        q2 *= recipNorm;
        q3 *= recipNorm;
    }
    time_stamp=ev_timestamp;
}

/* IMU algorithm update. Madgwick implementation.
 */
void MadgwickAHRSupdateIMU(float gx, float gy, float gz, float ax, float ay, float az) {
	float recipNorm;
	float s0, s1, s2, s3;
	float qDot1, qDot2, qDot3, qDot4;
	float _2q0, _2q1, _2q2, _2q3, _4q0, _4q1, _4q2 ,_8q1, _8q2, q0q0, q1q1, q2q2, q3q3;

	// Rate of change of quaternion from gyroscope
	qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
	qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
	qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
	qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

	// Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
	if(!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f))) {

		// Normalise accelerometer measurement
		recipNorm = invSqrt(ax * ax + ay * ay + az * az);
		ax *= recipNorm;
		ay *= recipNorm;
		az *= recipNorm;   

		// Auxiliary variables to avoid repeated arithmetic
		_2q0 = 2.0f * q0;
		_2q1 = 2.0f * q1;
		_2q2 = 2.0f * q2;
		_2q3 = 2.0f * q3;
		_4q0 = 4.0f * q0;
		_4q1 = 4.0f * q1;
		_4q2 = 4.0f * q2;
		_8q1 = 8.0f * q1;
		_8q2 = 8.0f * q2;
		q0q0 = q0 * q0;
		q1q1 = q1 * q1;
		q2q2 = q2 * q2;
		q3q3 = q3 * q3;

		// Gradient decent algorithm corrective step
		s0 = _4q0 * q2q2 + _2q2 * ax + _4q0 * q1q1 - _2q1 * ay;
		s1 = _4q1 * q3q3 - _2q3 * ax + 4.0f * q0q0 * q1 - _2q0 * ay - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * az;
		s2 = 4.0f * q0q0 * q2 + _2q0 * ax + _4q2 * q3q3 - _2q3 * ay - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * az;
		s3 = 4.0f * q1q1 * q3 - _2q1 * ax + 4.0f * q2q2 * q3 - _2q2 * ay;
		recipNorm = invSqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
		s0 *= recipNorm;
		s1 *= recipNorm;
		s2 *= recipNorm;
		s3 *= recipNorm;

		// Apply feedback step
		qDot1 -= beta * s0;
		qDot2 -= beta * s1;
		qDot3 -= beta * s2;
		qDot4 -= beta * s3;
	}

	// Integrate rate of change of quaternion to yield quaternion
	q0 += qDot1 * (1.0f / sampleFreq);
	q1 += qDot2 * (1.0f / sampleFreq);
	q2 += qDot3 * (1.0f / sampleFreq);
	q3 += qDot4 * (1.0f / sampleFreq);

	// Normalise quaternion
	recipNorm = invSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
	q0 *= recipNorm;
	q1 *= recipNorm;
	q2 *= recipNorm;
	q3 *= recipNorm;
}

/* Fast inverse square-root
 * See: http://en.wikipedia.org/wiki/Fast_inverse_square_root
 */
float invSqrt(float x) {
	float halfx = 0.5f * x;
	float y = x;
	long i = *(long*)&y;
	i = 0x5f3759df - (i>>1);
	y = *(float*)&i;
	y = y * (1.5f - (halfx * y * y));
	return y;
}

/* Multiplies quaternions q1 and q2. Result goes in q1.
 */
void multiplyQuat(Quaternion* q1, Quaternion* q2){
    float nx = (q1->w)*(q2->x) + (q1->x)*(q2->w) + (q1->y)*(q2->z) - (q1->z)*(q2->y);
    float ny = (q1->w*q2->y - q1->x*q2->z + q1->y*q2->w + q1->z*q2->x);
    float nz = (q1->w*q2->z + q1->x*q2->y - q1->y*q2->x + q1->z*q2->w);
    float nw = (q1->w*q2->w - q1->x*q2->x - q1->y*q2->y - q1->z*q2->z);
    q1->x = nx;
    q1->y = ny;
    q1->z = nz;
    q1->w = nw;
}

