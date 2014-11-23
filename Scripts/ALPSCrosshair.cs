/************************************************************************
	ALPSCrosshair adds a crosshair in the middle of the screen
	
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

public class ALPSCrosshair : MonoBehaviour {

	/**Public**/
	public static ALPSConfig DeviceConfig;


	/**Private**/
	private Vector2 CenterGUI;
	private Vector2 CenterGUIBar;
	private Vector2 CenterIPD;
	private bool LeftEye;
	private Texture2D  arc;
	private Texture2D  bar;

	/**Functions**/
	public void Awake(){
		LeftEye = GetComponent<ALPSCamera> ().LeftEye;
		arc = Resources.Load ("Textures/arcs") as Texture2D ;
		bar = Resources.Load ("Textures/progress_point") as Texture2D ;
		
		CenterIPD = new Vector2 (ALPSController.ScreenWidthPix*0.5f + ((LeftEye?-1:1) * ((DeviceConfig.IPD <= DeviceConfig.Width * 0.5f)?DeviceConfig.getIPDPixels()*0.5f:ALPSController.ScreenWidthPix*0.25f)) ,ALPSController.ScreenHeightPix * 0.5f);
		CenterGUI = new Vector2(CenterIPD.x-(arc.width * 0.5f),CenterIPD.y-(arc.height * 0.5f));
		CenterGUIBar = new Vector2(CenterIPD.x-(bar.width * 0.5f),CenterIPD.y-(bar.height * 0.5f));
	}

	public void updateCrosshair(){
		CenterIPD = new Vector2 (ALPSController.ScreenWidthPix*0.5f + ((LeftEye?-1:1) * ((DeviceConfig.IPD <= DeviceConfig.Width * 0.5f)?DeviceConfig.getIPDPixels()*0.5f:ALPSController.ScreenWidthPix*0.25f)) ,ALPSController.ScreenHeightPix * 0.5f);
		CenterGUI = new Vector2(CenterIPD.x-(arc.width * 0.5f),CenterIPD.y-(arc.height * 0.5f));
		CenterGUIBar = new Vector2(CenterIPD.x-(bar.width * 0.5f),CenterIPD.y-(bar.height * 0.5f));
	}

	void OnGUI(){
		GUI.DrawTexture(new Rect(CenterGUI.x,CenterGUI.y,arc.width,arc.height),arc,ScaleMode.ScaleToFit, true, 0f);
		CenterGUIBar = new Vector2(CenterIPD.x-((arc.width-4)*ALPSNavigation.progress() * 0.5f),CenterIPD.y-(bar.height * 0.5f));
		GUI.DrawTexture(new Rect(CenterGUIBar.x,CenterGUIBar.y,(arc.width-4)*ALPSNavigation.progress(),bar.height),bar,ScaleMode.StretchToFill, true, 0f);
	}
}
