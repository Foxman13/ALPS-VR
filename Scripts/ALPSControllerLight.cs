/************************************************************************
	ALPSControllerLight is the main class for non Pro license holders
	
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

using UnityEngine;
using System.Collections;

public class ALPSControllerLight : MonoBehaviour {

	/**Public**/

	//One camera for each eye
	public GameObject CameraLeft;
	public GameObject CameraRight;

	//Inter lens distance in millimeters. Should match the IPD but can be tweaked
	//to increase or decrease the stereo effect. Basically, with an ILD set to 0, there
	//is no 3D effect.
	public float ILD;

	//Vector between eyes and the pivot point (neck)
	public Vector2 NeckToEye;

	public bool AddTracking;

	/**Functions**/
	void Awake () {
		if (AddTracking) {
			#if UNITY_EDITOR
				gameObject.AddComponent ("MouseLook");
			#elif UNITY_ANDROID
				gameObject.AddComponent("ALPSGyro");
				Screen.orientation = ScreenOrientation.LandscapeLeft;
				ALPSAndroid.Init ();
			#endif
		}
		CameraLeft = new GameObject("CameraLeft");
		CameraLeft.AddComponent ("Camera");
		CameraLeft.camera.rect = new Rect (0,0,0.5f,1);
		CameraLeft.transform.parent = transform;
		CameraLeft.transform.position = transform.position;
		CameraLeft.transform.localPosition = new Vector3 (ILD*-0.0005f,NeckToEye.y*0.001f,NeckToEye.x*0.001f);
		AudioListener[] listeners = FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

		CameraRight = new GameObject("CameraRight");
		CameraRight.AddComponent ("Camera");
		CameraRight.camera.rect = new Rect (0.5f,0,0.5f,1);
		CameraRight.transform.parent = transform;
		CameraRight.transform.position = transform.position;
		CameraRight.transform.localPosition = new Vector3 (ILD*0.0005f,NeckToEye.y*0.001f,NeckToEye.x*0.001f);


		if (listeners.Length < 1) {
			gameObject.AddComponent ("AudioListener");
		}

		CameraLeft.camera.fieldOfView = 90f;
		CameraRight.camera.fieldOfView = 90f;
		SetStereoLayers ("left","right");

	}

	public void SetCameraSettings(Camera _cam){
		CameraLeft.camera.CopyFrom (_cam);
		CameraRight.camera.CopyFrom (_cam);
		CameraLeft.camera.rect = new Rect (0,0,0.5f,1);
		CameraRight.camera.rect = new Rect (0.5f,0,0.5f,1);
	}

	public int SetStereoLayers(string _leftLayer, string _rightLayer){
		int leftLayer = LayerMask.NameToLayer (_leftLayer);
		int rightLayer = LayerMask.NameToLayer (_rightLayer);
		if (leftLayer < 0 && rightLayer < 0) return -1;

		CameraLeft.camera.cullingMask |= 1 << LayerMask.NameToLayer(_leftLayer);
		CameraLeft.camera.cullingMask &=  ~(1 << LayerMask.NameToLayer(_rightLayer));

		CameraRight.camera.cullingMask |= 1 << LayerMask.NameToLayer(_rightLayer);
		CameraRight.camera.cullingMask &=  ~(1 << LayerMask.NameToLayer(_leftLayer));

		return 0;
	}

	//This can be useful for setting up a Raycast
	public Vector3 PointOfView(){
		//returns current position plus NeckToEye vector
		return new Vector3(transform.position.x,transform.position.y + NeckToEye.y*0.001f,transform.position.z + NeckToEye.x*0.001f);
	}

	public Vector3 ForwardDirection(){
		return CameraLeft.camera.transform.forward;
	}

	public Camera[] GetCameras(){
		Camera[] cams = {CameraLeft.camera, CameraRight.camera};
		return cams;
	}
}
