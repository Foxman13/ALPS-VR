/************************************************************************
	ALPSConfig describes a configuration for one particular device
		
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
public class ALPSConfig{

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	public const float INCH_TO_MM = 25.4f;
	public static float DEFAULT_DPI = 96f;

	//Vector between eyes and the pivot point (neck)
	public static Vector2 neckPivotToEye = new Vector2 (80f,120f);
	
	//Configuration name
	public Device deviceName;
	
	//Is barrel distortion enabled or not
	public bool enableBarrelDistortion;
	
	//Is chromatic correction enabled or not
	public bool enableChromaticCorrection;
	
	//Does the application run in full screen or must it have the same
	//size on every device
	public bool fixedSize;
	
	//Inter pupillary distance in millimeters. Must match the distance between
	//users' eyes
	public float IPD;
	
	//Inter lens distance in millimeters. Should match the IPD but can be tweaked
	//to increase or decrease the stereo effect. Basically, with an ILD set to 0, there
	//is no 3D effect.
	public float ILD;
	
	//Vertical field of view of both cameras
	[Range(1,179)]
	public float fieldOfView;
	
	//Chromatic correction coefficient
	public float chromaticCorrection;
	
	//Barrel distortion parameters.
	public float k1;
	public float k2;
	
	//DPI
	public float DPI;

	//Width of the viewport in mm
	[SerializeField]
	private int width;
	public int Width {
		get{return (int)(fixedSize?width:ALPSController.screenWidthPix/DPI*INCH_TO_MM);}
		set{
			if(value<0) width = 0;
			else width = value;
		}
	}

	//Height of the viewport in mm
	[SerializeField]
	private int height;
	public int Height {
		get{
			return (int)(fixedSize?height:ALPSController.screenHeightPix/DPI*INCH_TO_MM);}
		set{
			if(value<0) height = 0;
			else height = value;
		}
	}

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/// <summary>
	/// Viewport height in pixels. 
	/// </summary>
	public int HeightPix(){
		return (int)(fixedSize ? Height * DPI / INCH_TO_MM : ALPSController.screenHeightPix);
	}

	/// <summary>
	/// Viewport width in pixels. 
	/// </summary>
	public int WidthPix(){
		return (int)(fixedSize ? Width * DPI / INCH_TO_MM : ALPSController.screenWidthPix);
	}

	/// <summary>
	/// Creates a new device configuration.
	/// </summary>
	/// <param name="_DeviceName">Device name.</param>
	/// <param name="_EnableBarrelDistortion">True if barrel distortion must be enabled, false otherwise.</param>
	/// <param name="_EnableChromaticCorrection">True if chromatic correction must be enabled, false otherwise.</param>
	/// <param name="_FixedSize">True is viewport must be fixed insize, false if viewport must be fullscreen.</param>
	/// <param name="_IPD">Inter pupillary distance in millimeters. Must match the distance between users' eyes</param>
	/// <param name="_ILD">IInter lens distance in millimeters. Should match the IPD but can be tweaked to increase or decrease the stereo effect.</param>
	/// <param name="_FieldOfView">Cameras field of view.</param>
	/// <param name="_ChromaticCorrection">Chromatic Correction factor.</param>
	/// <param name="_k1">Barrel distortion first order factor.</param>
	/// <param name="_k2">Barrel distortion second order factor.</param>
	/// <param name="_Width">Viewport width in millimeters if fixed in size, ignored otherwise.</param>
	/// <param name="_Height">Viewport height in millimeters if fixed in size, ignored otherwise.</param>
	public ALPSConfig(Device _DeviceName, bool _EnableBarrelDistortion, bool _EnableChromaticCorrection, bool _FixedSize, float _IPD, float _ILD, float _FieldOfView, float _ChromaticCorrection, float _k1, float _k2, int _Width, int _Height){

		deviceName = _DeviceName;
		IPD = _IPD;
		ILD = _ILD;
		fieldOfView = _FieldOfView;
		chromaticCorrection = _ChromaticCorrection;
		k1 = _k1;
		k2 = _k2;
		Width = _Width;
		Height = _Height;
		enableBarrelDistortion = _EnableBarrelDistortion;
		enableChromaticCorrection = _EnableChromaticCorrection;
		fixedSize = _FixedSize;
	}

	/// <summary>
	/// Returns current screen option : FixedSize or FullScreen.
	/// </summary>
	public ScreenOption GetScreenOption(){
		return fixedSize ? ScreenOption.FixedSize : ScreenOption.FullScreen;
	}

	/// <summary>
	/// Return IPD in pixels.
	/// </summary>
	public int GetIPDPixels(){
		//IPD is in millimeters while DPI is in inches
		return (int)(IPD * DPI / INCH_TO_MM);
	}
}
