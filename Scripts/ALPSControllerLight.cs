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

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	//One camera for each eye
	public GameObject cameraLeft;
	public GameObject cameraRight;

	//Head represents user's head
	public GameObject head;

	//Inter lens distance in millimeters. Should match the IPD but can be tweaked
	//to increase or decrease the stereo effect. Basically, with an ILD set to 0, there
	//is no 3D effect.
	public float ILD;

	//Vector between eyes and the pivot point (neck)
	public Vector2 neckToEye;

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/// <summary>
	/// Initializes side-by-side rendering and head tracking.
	/// </summary>
	public void Awake () {
		head = new GameObject ("ALPSHead");
		head.transform.parent = transform;
		head.transform.position = transform.position;
		#if UNITY_EDITOR
			head.AddComponent <MouseLook>();
		#elif UNITY_ANDROID
			head.AddComponent("ALPSGyro");
		#endif
		cameraLeft = new GameObject("CameraLeft");
		cameraLeft.AddComponent <Camera>();
		cameraLeft.GetComponent<Camera>().rect = new Rect (0,0,0.5f,1);
		cameraLeft.transform.parent = head.transform;
		cameraLeft.transform.position = head.transform.position;
		cameraLeft.transform.localPosition = new Vector3 (ILD*-0.0005f,neckToEye.y*0.001f,neckToEye.x*0.001f);

		cameraRight = new GameObject("CameraRight");
		cameraRight.AddComponent <Camera>();
		cameraRight.GetComponent<Camera>().rect = new Rect (0.5f,0,0.5f,1);
		cameraRight.transform.parent = head.transform;
		cameraRight.transform.position = head.transform.position;
		cameraRight.transform.localPosition = new Vector3 (ILD*0.0005f,neckToEye.y*0.001f,neckToEye.x*0.001f);

		AudioListener[] listeners = FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
		if (listeners.Length < 1) {
			gameObject.AddComponent <AudioListener>();
		}
	}

	/// <summary>
	/// Copy camera settings to left and right cameras. Will overwrite culling masks.
	/// </summary>
	/// <param name="_cam">The camera from which you want to copy the settings.</param>
	public void SetCameraSettings(Camera _cam){
		cameraLeft.GetComponent<Camera>().CopyFrom (_cam);
		cameraRight.GetComponent<Camera>().CopyFrom (_cam);
		cameraLeft.GetComponent<Camera>().rect = new Rect (0,0,0.5f,1);
		cameraRight.GetComponent<Camera>().rect = new Rect (0.5f,0,0.5f,1);
	}
	
	/// <summary>
	/// Adds left and right layers to the existing culling masks for left and right cameras.
	/// </summary>
	/// <param name="_leftLayer">Name of the layer rendered by the left camera.</param>
	/// <param name="_rightLayer">Name of the layer rendered by the right camera.</param>
	public int SetStereoLayers(string _leftLayer, string _rightLayer){
		int leftLayer = LayerMask.NameToLayer (_leftLayer);
		int rightLayer = LayerMask.NameToLayer (_rightLayer);
		if (leftLayer < 0 && rightLayer < 0) return -1;

		cameraLeft.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer(_leftLayer);
		cameraLeft.GetComponent<Camera>().cullingMask &=  ~(1 << LayerMask.NameToLayer(_rightLayer));

		cameraRight.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer(_rightLayer);
		cameraRight.GetComponent<Camera>().cullingMask &=  ~(1 << LayerMask.NameToLayer(_leftLayer));

		return 0;
	}

	/// <summary>
	/// Returns point of view position. This can be useful for setting up a Raycast.
	/// </summary>
	public Vector3 PointOfView(){
		//returns current position plus NeckToEye vector
		return new Vector3(transform.position.x,transform.position.y + neckToEye.y*0.001f,transform.position.z + neckToEye.x*0.001f);
	}

	/// <summary>
	/// Returns forward direction vector. This can be useful for setting up a Raycast.
	/// </summary>
	public Vector3 ForwardDirection(){
		return cameraLeft.GetComponent<Camera>().transform.forward;
	}

	/// <summary>
	/// Returns left and right cameras.
	/// </summary>
	public Camera[] GetCameras(){
		Camera[] cams = {cameraLeft.GetComponent<Camera>(), cameraRight.GetComponent<Camera>()};
		return cams;
	}
}
