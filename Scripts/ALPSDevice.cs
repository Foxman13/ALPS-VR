/************************************************************************
	ALPSDevice provides a specific configuration for each supported device

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

	//====================================================================================================
	// Attributes
	//====================================================================================================

	public enum Device{
		DEFAULT,
		ALTERGAZE,
		CARDBOARD,
		FIREFLY
	};

	public enum ScreenOption{
		FixedSize, 
		FullScreen
	};

public class ALPSDevice {

	//====================================================================================================
	// Functions
	//====================================================================================================

	/// <summary>
	/// Returns device configuration corresponding to a device name.
	/// </summary>
	/// <param name="_device">Device name.</param>
	public static ALPSConfig GetConfig(Device _device){
		ALPSConfig config;
		switch (_device) {
			case Device.ALTERGAZE:
			config = new ALPSConfig(Device.ALTERGAZE,true,true,false,62f,62f,85f,-1f,0.4f,0.2f,0,0);
				break;
			case Device.CARDBOARD:
			config = new ALPSConfig(Device.CARDBOARD,true,true,false,62f,62f,85f,-1.5f,0.5f,0.2f,128,75);
				break;
			case Device.FIREFLY:
			config = new ALPSConfig(Device.FIREFLY,true,true,false,62f,62f,85f,-2f,0.7f,0.2f,140,75);
				break;
			case Device.DEFAULT:
			default: 
				config = new ALPSConfig(Device.DEFAULT,false,false,false,62f,62f,85f,0f,0f,0f,0,0);
				break;
		}
		return config;
	}

}
