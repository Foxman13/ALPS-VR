/************************************************************************
	ALPSContollerEditor is a custom editor for ALPSController class
	
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
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(ALPSController))]
public class ALPSContollerEditor : Editor {
	 
	/**Public**/
	public ALPSConfig DeviceConfig;
	public ALPSController Controller;

	public Device Device{
		get{
			return DeviceConfig.DeviceName;
		}
		set{
			if(DeviceConfig.DeviceName != value){
				Controller.setDevice(value);
				OnEnable();
			}
		}
	}

	public ScreenOption screenSize{
		get{
			return DeviceConfig.FixedSize?ScreenOption.FixedSize:ScreenOption.FullScreen;
		}
		set{
			DeviceConfig.FixedSize=(value == ScreenOption.FixedSize)?true:false;
		}
	}

	/**Functions**/
	void OnEnable()
	{
		Controller = (ALPSController)target;
		DeviceConfig = (Controller.DeviceConfig == null)? ALPSDevice.getConfig(Device.DEFAULT):Controller.DeviceConfig;
		Controller.DeviceConfig = DeviceConfig;
		ALPSCamera.DeviceConfig = DeviceConfig;
		ALPSBarrelMesh.DeviceConfig = DeviceConfig;
		ALPSCrosshair.DeviceConfig = DeviceConfig;
		screenSize = DeviceConfig.getScreenOption ();
	}

	public override void OnInspectorGUI(){

		//Device
		Device = (Device)EditorGUILayout.EnumPopup("Device:",Device);

		//IPD
		DeviceConfig.IPD = EditorGUILayout.FloatField (new GUIContent("IPD", "Inter Pupilary Distance in millimeter. This must match the distance between the user's eyes"),DeviceConfig.IPD);
		//Stereo distance
		DeviceConfig.ILD = EditorGUILayout.FloatField (new GUIContent("ILD","Inter Lens Distance in millimeter. This is the distance between both cameras and this should match the IPD. Can be tweaked to increase or decrease the stereo effect."),DeviceConfig.ILD);

		//Field Of View
		DeviceConfig.FieldOfView = EditorGUILayout.Slider ("Vertical FOV",DeviceConfig.FieldOfView, 1, 180);

		//Screen size
		screenSize = (ScreenOption)EditorGUILayout.EnumPopup("Screen size:",screenSize);

		if (screenSize == ScreenOption.FixedSize) {
			DeviceConfig.Width = EditorGUILayout.IntField (new GUIContent("\twidth", "Width of the viewport in millimeter"), DeviceConfig.Width);
			DeviceConfig.Height = EditorGUILayout.IntField (new GUIContent("\theight", "Height of the viewport in millimeter"), DeviceConfig.Height);
			DeviceConfig.FixedSize = true;
		} else {
			DeviceConfig.FixedSize = false;
		}

		//Barrel distortion
		DeviceConfig.EnableBarrelDistortion = EditorGUILayout.Toggle ("Barrel distortion", DeviceConfig.EnableBarrelDistortion); 
		if (DeviceConfig.EnableBarrelDistortion) {
			DeviceConfig.k1 = EditorGUILayout.FloatField ("\tk1", DeviceConfig.k1);
			DeviceConfig.k2 = EditorGUILayout.FloatField ("\tk2", DeviceConfig.k2);
		}

		//Chromatic correction
		DeviceConfig.EnableChromaticCorrection = EditorGUILayout.Toggle ("Chromatic correction",DeviceConfig.EnableChromaticCorrection);
		if (DeviceConfig.EnableChromaticCorrection) {
			DeviceConfig.ChromaticCorrection = EditorGUILayout.FloatField ("\tCorrection intensity", DeviceConfig.ChromaticCorrection);
		}

		Controller.CrosshairEnabled = EditorGUILayout.Toggle("Crosshair", Controller.CrosshairEnabled); 

		if (GUI.changed) {
			Controller.clearDirty();
			EditorUtility.SetDirty(target);
		}
	}
}
