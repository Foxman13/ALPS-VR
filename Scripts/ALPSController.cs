/************************************************************************
	ALPSController is the main class which manages custom rendering

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

[System.Serializable]
public class ALPSController : MonoBehaviour {

	/**Public**/
	public ALPSConfig DeviceConfig = ALPSDevice.getConfig(Device.FIREFLY);

	//One camera for each eye
	public Camera CameraLeft;
	public Camera CameraRight;

	//Render textures
	public RenderTexture srcTex;
	public RenderTexture destTex;

	//Screen size
	public static int ScreenWidthPix;
	public static int ScreenHeightPix;

	//Material
	public Material mat;

	//Crosshair
	public bool CrosshairEnabled;

	/**Private**/
	private Rect rectLeft,rectRight;
	private float DPI;

	/**Functions**/
	public void Awake(){
		#if UNITY_EDITOR
			transform.FindChild("Head").gameObject.AddComponent("MouseLook");
			ScreenWidthPix = Screen.width;
			ScreenHeightPix = Screen.height;
		#elif UNITY_ANDROID
			transform.FindChild("Head").gameObject.AddComponent("ALPSGyro");
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			ALPSAndroid.Init ();
			ScreenWidthPix = ALPSAndroid.WidthPixels ();
			ScreenHeightPix = ALPSAndroid.HeightPixels ();
		#endif

		//Make sure the longer dimension is width as the phone is always in landscape mode
		if(ScreenWidthPix<ScreenHeightPix){
			int tmp = ScreenHeightPix;
			ScreenHeightPix = ScreenWidthPix;
			ScreenWidthPix = tmp;
		}

		CameraLeft = transform.FindChild ("Head/EyeLeft").camera;
		CameraRight = transform.FindChild ("Head/EyeRight").camera;
	
		ALPSCamera.DeviceConfig = DeviceConfig;
		ALPSBarrelMesh.DeviceConfig = DeviceConfig;
		ALPSCrosshair.DeviceConfig = DeviceConfig;
		ALPSGUI.Controller = this;

		ALPSCamera[] ALPSCameras = FindObjectsOfType(typeof(ALPSCamera)) as ALPSCamera[];
		foreach (ALPSCamera cam in ALPSCameras) {
			cam.Init();
		}

		mat = new Material(Shader.Find ("Custom/ALPSDistortion"));

		DPI = Screen.dpi;

		//Render Textures
		srcTex = new RenderTexture (2048, 1024, 16);
		destTex = camera.targetTexture;

		CameraLeft.targetTexture = CameraRight.targetTexture = srcTex;

		// Setting the main camera
		camera.aspect = 1f;
		camera.backgroundColor = Color.black;
		camera.clearFlags =  CameraClearFlags.Nothing;
		camera.cullingMask = 0;
		camera.eventMask = 0;
		camera.orthographic = true;
		camera.renderingPath = RenderingPath.Forward;
		camera.useOcclusionCulling = false;
		CameraLeft.camera.depth = 0;
		CameraRight.camera.depth = 1;
		camera.depth = Mathf.Max (CameraLeft.depth, CameraRight.depth) + 1;

		//Enabling gyroscope
		Input.gyro.enabled = true;

		CameraLeft.gameObject.AddComponent("ALPSCrosshair");
		CameraRight.gameObject.AddComponent("ALPSCrosshair");

		clearDirty();
	}

	void OnPostRender(){

		RenderTexture.active = destTex;
		GL.Clear (false,true,Color.black);

		RenderEye (true,srcTex);
		RenderEye (false,srcTex);
	}
	
	void RenderEye(bool LeftEye, RenderTexture source){
		mat.mainTexture = source;
		mat.SetVector("_SHIFT",new Vector2(LeftEye?0:0.5f,0));
		float convergeOffset = ((DeviceConfig.Width * 0.5f) - DeviceConfig.IPD) / DeviceConfig.Width;
		mat.SetVector("_CONVERGE",new Vector2((LeftEye?1f:-1f)*convergeOffset,0));
		mat.SetFloat ("_AberrationOffset",DeviceConfig.EnableChromaticCorrection?DeviceConfig.ChromaticCorrection:0f);
		//mat.SetFloat ("_ChromaticConstant",DeviceConfig.EnableChromaticCorrection?ChromaticConstant:0f);
		float ratio = (DeviceConfig.IPD*0.5f) / DeviceConfig.Width;
		mat.SetVector ("_Center",new Vector2(0.5f+(LeftEye?-ratio:ratio),0.5f));

		GL.Viewport (LeftEye ? rectLeft : rectRight);

		GL.PushMatrix ();
		GL.LoadOrtho ();
		mat.SetPass (0);
		if(LeftEye)CameraLeft.GetComponent<ALPSCamera>().Draw ();
		else CameraRight.GetComponent<ALPSCamera>().Draw ();
		GL.PopMatrix ();
	}

	public void clearDirty(){

		//We give the current DPI to the new ALPSConfig
		DeviceConfig.DPI = DPI;
		if (DeviceConfig.DPI <= 0) {
			DeviceConfig.DPI = ALPSConfig.DEFAULT_DPI;
		}

		float widthPix = DeviceConfig.WidthPix();
		float heightPix = DeviceConfig.HeightPix();

		rectLeft  = new Rect (ScreenWidthPix*0.5f-widthPix*0.5f,ScreenHeightPix*0.5f-heightPix*0.5f,widthPix*0.5f,heightPix);
		rectRight = new Rect (ScreenWidthPix*0.5f,ScreenHeightPix*0.5f-heightPix*0.5f,widthPix*0.5f,heightPix);

		Vector3 camLeftPos = CameraLeft.transform.localPosition; 
		camLeftPos.x = -DeviceConfig.ILD*0.0005f;
		CameraLeft.transform.localPosition = camLeftPos;
		
		Vector3 camRightPos = CameraRight.transform.localPosition;
		camRightPos.x = DeviceConfig.ILD*0.0005f;
		CameraRight.transform.localPosition = camRightPos;
		
		CameraLeft.fieldOfView = DeviceConfig.FieldOfView;
		CameraRight.fieldOfView = DeviceConfig.FieldOfView;

		CameraLeft.GetComponent<ALPSCamera>().updateMesh();
		CameraRight.GetComponent<ALPSCamera>().updateMesh();

		ALPSCrosshair[] ch = GetComponentsInChildren<ALPSCrosshair> ();
		foreach (ALPSCrosshair c in ch) {
			c.updateCrosshair();
			c.enabled = CrosshairEnabled;
		}
	}

	void setFixedSize(bool _fixed){
		if (_fixed != DeviceConfig.FixedSize) {
			clearDirty();
		}
		DeviceConfig.FixedSize = _fixed;
	}

	public void setDevice(Device _device){
		DeviceConfig = ALPSDevice.getConfig (_device);
		ALPSCamera.DeviceConfig = DeviceConfig;
		ALPSBarrelMesh.DeviceConfig = DeviceConfig;
		ALPSCrosshair.DeviceConfig = DeviceConfig;
		clearDirty ();
	}
	 
}