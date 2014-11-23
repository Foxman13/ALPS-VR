/************************************************************************
	ALPSAndroid is an interface with the Android system
	
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

public class ALPSAndroid : MonoBehaviour {

	/**Private**/
	private static AndroidJavaClass jc;

	/**Functions**/
	// Use this for initialization
	public static void Init () {
		jc = new AndroidJavaClass ("com.alpsvr.immersive.ALPSActivity"); 
	}

	/// <summary>
	/// Vibrate constantly for 8 milliseconds.
	/// </summary>
	public static void Vibrate(){
		Vibrate(8);
	}

	/// <summary>
	/// Vibrate constantly for the specified period of time.
	/// </summary>
	/// <param name="_milliseconds">The number of milliseconds to vibrate.</param>
	public static void Vibrate(int _milliseconds){
		jc.CallStatic ("vibrate",_milliseconds);
	}

	/// <summary>
	/// The absolute width of the display in pixels.
	/// </summary>
	public static int WidthPixels(){
		return jc.CallStatic<int> ("getWidthPixel");
	}

	/// <summary>
	/// The absolute height of the display in pixels.
	/// </summary>
	public static int HeightPixels(){
		return jc.CallStatic<int> ("getHeightPixel");
	}
}
