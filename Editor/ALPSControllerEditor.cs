/************************************************************************
	ALPSControllerEditor is a custom editor for ALPSController class
	
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
public class ALPSControllerEditor : Editor {

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	public ALPSConfig deviceConfig;
	public ALPSController controller;

	public Device Device{
		get{
			return deviceConfig.deviceName;
		}
		set{
			if(deviceConfig.deviceName != value){
				controller.SetDevice(value);
				OnEnable();
			}
		}
	}

	public ScreenOption screenSize{
		get{
			return deviceConfig.fixedSize?ScreenOption.FixedSize:ScreenOption.FullScreen;
		}
		set{
			deviceConfig.fixedSize = (value == ScreenOption.FixedSize)?true:false;
		}
	}

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	public void OnEnable()
	{
		controller = (ALPSController)target;
		deviceConfig = (controller.deviceConfig == null)? ALPSDevice.GetConfig(Device.DEFAULT):controller.deviceConfig;
		controller.deviceConfig = deviceConfig;
		ALPSCamera.deviceConfig = deviceConfig;
		ALPSBarrelMesh.deviceConfig = deviceConfig;
		ALPSCrosshairs.deviceConfig = deviceConfig;
		screenSize = deviceConfig.GetScreenOption ();
	}

	public override void OnInspectorGUI(){

		//Device
		Device = (Device)EditorGUILayout.EnumPopup("Device:",Device);

		//IPD
		deviceConfig.IPD = EditorGUILayout.FloatField (new GUIContent("IPD", "Inter Pupilary Distance in millimeter. This must match the distance between the user's eyes"),deviceConfig.IPD);
		//Stereo distance
		deviceConfig.ILD = EditorGUILayout.FloatField (new GUIContent("ILD","Inter Lens Distance in millimeter. This is the distance between both cameras and this should match the IPD. Can be tweaked to increase or decrease the stereo effect."),deviceConfig.ILD);

		//Field Of View
		deviceConfig.fieldOfView = EditorGUILayout.Slider ("Vertical FOV",deviceConfig.fieldOfView, 1, 180);

		//Screen size
		screenSize = (ScreenOption)EditorGUILayout.EnumPopup("Screen size:",screenSize);

		if (screenSize == ScreenOption.FixedSize) {
			deviceConfig.Width = EditorGUILayout.IntField (new GUIContent("\twidth", "Width of the viewport in millimeter"), deviceConfig.Width);
			deviceConfig.Height = EditorGUILayout.IntField (new GUIContent("\theight", "Height of the viewport in millimeter"), deviceConfig.Height);
			deviceConfig.fixedSize = true;
		} else {
			deviceConfig.fixedSize = false;
		}

		//Barrel distortion
		deviceConfig.enableBarrelDistortion = EditorGUILayout.Toggle ("Barrel distortion", deviceConfig.enableBarrelDistortion); 
		if (deviceConfig.enableBarrelDistortion) {
			deviceConfig.k1 = EditorGUILayout.FloatField ("\tk1", deviceConfig.k1);
			deviceConfig.k2 = EditorGUILayout.FloatField ("\tk2", deviceConfig.k2);
		}

		//Chromatic correction
		deviceConfig.enableChromaticCorrection = EditorGUILayout.Toggle ("Chromatic correction",deviceConfig.enableChromaticCorrection);
		if (deviceConfig.enableChromaticCorrection) {
			deviceConfig.chromaticCorrection = EditorGUILayout.FloatField ("\tCorrection intensity", deviceConfig.chromaticCorrection);
		}

		controller.crosshairsEnabled = EditorGUILayout.Toggle("Crosshair", controller.crosshairsEnabled); 

		if (GUI.changed) {
			controller.ClearDirty();
			EditorUtility.SetDirty(target);
		}
	}
}
