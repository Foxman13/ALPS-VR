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

	/**Public**/
	public const float INCH_TO_MM = 25.4f;
	public static float DEFAULT_DPI = 96f;
	//Vector between eyes and the pivot point (neck)
	public static Vector2 NeckPivotToEye = new Vector2 (80f,120f);
	
	//Configuration name
	public Device DeviceName;
	
	//Is barrel distortion enabled or not
	public bool EnableBarrelDistortion;
	
	//Is chromatic correction enabled or not
	public bool EnableChromaticCorrection;
	
	//Does the application run in full screen or must it have the same
	//size on every device
	public bool FixedSize;
	
	//Inter pupillary distance in millimeters. Must match the distance between
	//users' eyes
	public float IPD;
	
	//Inter lens distance in millimeters. Should match the IPD but can be tweaked
	//to increase or decrease the stereo effect. Basically, with an ILD set to 0, there
	//is no 3D effect.
	public float ILD;
	
	//Vertical field of view of both cameras
	[Range(1,179)]
	public float FieldOfView;
	
	//Chromatic correction coefficient
	public float ChromaticCorrection;
	
	//Barrel distortion parameters.
	public float k1;
	public float k2;
	
	//DPI
	public float DPI;

	//Width of the viewport in mm
	[SerializeField]
	private int width;
	public int Width {
		get{return (int)(FixedSize?width:ALPSController.ScreenWidthPix/DPI*INCH_TO_MM);}
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
			return (int)(FixedSize?height:ALPSController.ScreenHeightPix/DPI*INCH_TO_MM);}
		set{
			if(value<0) height = 0;
			else height = value;
		}
	}

	/**Functions**/
	public int HeightPix(){

		return (int)(FixedSize ? Height * DPI / INCH_TO_MM : ALPSController.ScreenHeightPix);
	}

	public int WidthPix(){

		return (int)(FixedSize ? Width * DPI / INCH_TO_MM : ALPSController.ScreenWidthPix);
	}


	public ALPSConfig(Device _DeviceName, bool _EnableBarrelDistortion, bool _EnableChromaticCorrection, bool _FixedSize, float _IPD, float _ILD, float _FieldOfView, float _ChromaticCorrection, float _k1, float _k2, int _Width, int _Height){

		DeviceName = _DeviceName;
		IPD = _IPD;
		ILD = _ILD;
		FieldOfView = _FieldOfView;
		ChromaticCorrection = _ChromaticCorrection;
		k1 = _k1;
		k2 = _k2;
		Width = _Width;
		Height = _Height;
		EnableBarrelDistortion = _EnableBarrelDistortion;
		EnableChromaticCorrection = _EnableChromaticCorrection;
		FixedSize = _FixedSize;
	}

	public ScreenOption getScreenOption(){

		return FixedSize ? ScreenOption.FixedSize : ScreenOption.FullScreen;
	}

	public int getIPDPixels(){

		//IPD is in millimeters while DPI is in inches
		return (int)(IPD * DPI / INCH_TO_MM);
	}
}
