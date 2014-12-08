/************************************************************************
	ALPSActivity is the ALPS Android plugin for Unity

    Copyright (C) 2014  ALPS VR.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 ************************************************************************/

package com.alpsvr.android;

import android.graphics.Point;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Vibrator;
import android.view.Display;
import android.view.KeyEvent;
import android.view.View;
import com.unity3d.player.UnityPlayerActivity;

	public class ALPSActivity extends UnityPlayerActivity{

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	private SensorManager mSensorManager;
	private SensorManager mSensorManager2;
	private Sensor mGameRotation;
	private Sensor mGyroscope;
	private static float[] orientation=new float[4];
	private static float[] dtGyro=new float[4];
	private Handler mHandler = new Handler();
	private static Vibrator v; 
	private static int width;
	private static int height;
	private long timestamp = 0;
	static float NS2S = (float) (1.0/1000000000.0);
	static float EPSILON = (float) 0.000000001;

	private final SensorEventListener gameRotationListener = new SensorEventListener() {
		public void onSensorChanged(SensorEvent event) {
			//orientation=multiplyQuat(event.values,dtGyro);
			orientation = event.values;
		}
		public void onAccuracyChanged(Sensor sensor, int accuracy) {
		}
	};

	private final SensorEventListener gyroscopeListener = new SensorEventListener() {
		public void onSensorChanged(SensorEvent event) {
			integrateGyro(event);
		}
		public void onAccuracyChanged(Sensor sensor, int accuracy) {
		}
	};
	
	private Runnable resetImmersive = new Runnable(){

		public void run() {
			getWindow().getDecorView().setSystemUiVisibility(
					  View.SYSTEM_UI_FLAG_FULLSCREEN
					| View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
					| View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
					| View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
					| View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
					| View.SYSTEM_UI_FLAG_LAYOUT_STABLE);

		}

	};
	
	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/**
	 * Called when the activity is starting. 
	 */
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
		mSensorManager2 = (SensorManager) getSystemService(SENSOR_SERVICE);
		mGameRotation = mSensorManager.getDefaultSensor(Sensor.TYPE_GAME_ROTATION_VECTOR);
		mGyroscope = mSensorManager2.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD);
		v = (Vibrator) getSystemService(VIBRATOR_SERVICE);

		Display display = getWindowManager().getDefaultDisplay();
		Point size = new Point();
		display.getRealSize(size);
		width = size.x;
		height = size.y;
	}

	/**
	 * Called after onRestoreInstanceState(Bundle), onRestart(), or onPause(), for your activity to start interacting with the user.
	 */
	protected void onResume(){
		super.onResume();
		mSensorManager.registerListener(gameRotationListener, mGameRotation, SensorManager.SENSOR_DELAY_FASTEST);
		mSensorManager2.registerListener(gyroscopeListener, mGyroscope, SensorManager.SENSOR_DELAY_FASTEST);
		
		if (Build.VERSION.SDK_INT >= 19) {
			int flags = View.SYSTEM_UI_FLAG_FULLSCREEN
					  | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
					  | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
					  | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
					  | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
					  | View.SYSTEM_UI_FLAG_LAYOUT_STABLE;

			this.findViewById(android.R.id.content).setSystemUiVisibility(flags);
		}

	}

	/**
	 * Called as part of the activity lifecycle when an activity is going into the background, but has not (yet) been killed.
	 */
	protected void onPause() {
		super.onPause();
		mSensorManager.unregisterListener(gameRotationListener);
		mSensorManager2.unregisterListener(gyroscopeListener);
	}

	/**
	 * Called when a key down event has occurred.
	 */
	public boolean onKeyDown(int keyCode, KeyEvent event){
		if (Build.VERSION.SDK_INT >= 19){
			if (keyCode == KeyEvent.KEYCODE_BACK){
				finish();
			} else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN || keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
				mHandler.postDelayed(resetImmersive, 500);
			}
		}              
		return super.onKeyDown(keyCode, event);        
	}

	/**
	 * The x component of rotation quaternion.
	 * @return The x component of rotation quaternion.
	 */
	public static float getGameRotationX(){
		return (orientation==null?0:orientation[0]);
	}
	
	/**
	 * The y component of rotation quaternion.
	 * @return The y component of rotation quaternion.
	 */
	public static float getGameRotationY(){
		return (orientation==null?0:orientation[1]);
	}
	
	/**
	 * The z component of rotation quaternion.
	 * @return The z component of rotation quaternion.
	 */
	public static float getGameRotationZ(){
		return (orientation==null?0:orientation[2]);
	}
	
	/**
	 * The w component of rotation quaternion.
	 * @return The w component of rotation quaternion.
	 */
	public static float getGameRotationW(){
		return (orientation==null?0:orientation[3]);
	}

	/**
	 * Vibrates constantly for the specified period of time.
	 * @param _ms The number of milliseconds to vibrate.
	 */
	public static void vibrate(int _ms){
		if(v != null) v.vibrate(_ms);
	}

	/**
	 * The display width in pixels.
	 * @return The display width in pixels.
	 */
	public static int getWidthPixel(){
		return width;
	}

	/**
	 * The display height in pixels.
	 * @return The display height in pixels.
	 */
	public static int getHeightPixel(){
		return height;
	}

	/**
	 * Integrates gyroscope output over time.
	 * @param event The gyroscope event
	 */
	//This is the method to integrate gyroscope output over time provided by the Android documentation
	//See : http://developer.android.com/guide/topics/sensors/sensors_motion.html#sensors-motion-gyro
	private void integrateGyro(SensorEvent event){
		// This timestep's delta rotation to be multiplied by the current rotation
		// after computing it from the gyro sample data.
		if (timestamp != 0) {
			final float dT = (event.timestamp - timestamp) * NS2S;
			// Axis of the rotation sample, not normalized yet.
			float axisX = event.values[0];
			float axisY = event.values[1];
			float axisZ = event.values[2];

			// Calculate the angular speed of the sample
			float omegaMagnitude = (float) Math.sqrt(axisX*axisX + axisY*axisY + axisZ*axisZ);

			// Normalize the rotation vector if it's big enough to get the axis
			// (that is, EPSILON should represent your maximum allowable margin of error)
			if (omegaMagnitude > EPSILON) {
				axisX /= omegaMagnitude;
				axisY /= omegaMagnitude;
				axisZ /= omegaMagnitude;
			}

			// Integrate around this axis with the angular speed by the timestep
			// in order to get a delta rotation from this sample over the timestep
			// We will convert this axis-angle representation of the delta rotation
			// into a quaternion before turning it into the rotation matrix.
			float thetaOverTwo = omegaMagnitude * dT / 2.0f;
			float sinThetaOverTwo = (float) Math.sin(thetaOverTwo);
			float cosThetaOverTwo = (float) Math.cos(thetaOverTwo);
			dtGyro[0] = sinThetaOverTwo * axisX;
			dtGyro[1] = sinThetaOverTwo * axisY;
			dtGyro[2] = sinThetaOverTwo * axisZ;
			dtGyro[3] = cosThetaOverTwo;

			//This is a hack to accelerate rotation when user moves the head.
			dtGyro=multiplyQuat(dtGyro,dtGyro);
			dtGyro=multiplyQuat(dtGyro,dtGyro);
			dtGyro=multiplyQuat(dtGyro,dtGyro);

		}
		timestamp = event.timestamp;
	}
	
	/**
	 * Multiplies two quaternions
	 * @param q1 First quaternion
	 * @param q2 Second quaternion
	 * @return Result of q1 * q2
	 */
	protected float[] multiplyQuat(float[] q1, float[] q2){
		float nx = (q1[3])*(q2[0]) + (q1[0])*(q2[3]) + (q1[1])*(q2[2]) - (q1[2])*(q2[1]);
		float ny = (q1[3]*q2[1] - q1[0]*q2[2] + q1[1]*q2[3] + q1[2]*q2[0]);
		float nz = (q1[3]*q2[2] + q1[0]*q2[1] - q1[1]*q2[0] + q1[2]*q2[3]);
		float nw = (q1[3]*q2[3] - q1[0]*q2[0] - q1[1]*q2[1] - q1[2]*q2[2]);
		float[] q = {nx,ny,nz,nw};
		return q;
	}

}