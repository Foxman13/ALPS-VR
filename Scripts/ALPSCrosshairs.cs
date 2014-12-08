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

public class ALPSCrosshairs : MonoBehaviour {

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	public static ALPSConfig deviceConfig;


	/**Private**/
	private Vector2 centerGUI;
	private Vector2 centerGUIBar;
	private Vector2 centerIPD;
	private bool leftEye;
	private Texture2D  arc;
	private Texture2D  bar;

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/// <summary>
	/// Initializes crosshairs. 
	/// </summary>
	public void Awake(){
		leftEye = GetComponent<ALPSCamera> ().leftEye;
		arc = Resources.Load ("Textures/arcs") as Texture2D ;
		bar = Resources.Load ("Textures/progress_point") as Texture2D ;
		
		centerIPD = new Vector2 (ALPSController.screenWidthPix*0.5f + ((leftEye?-1:1) * ((deviceConfig.IPD <= deviceConfig.Width * 0.5f)?deviceConfig.GetIPDPixels()*0.5f:ALPSController.screenWidthPix*0.25f)) ,ALPSController.screenHeightPix * 0.5f);
		centerGUI = new Vector2(centerIPD.x-(arc.width * 0.5f),centerIPD.y-(arc.height * 0.5f));
		centerGUIBar = new Vector2(centerIPD.x-(bar.width * 0.5f),centerIPD.y-(bar.height * 0.5f));
	}

	/// <summary>
	/// Updates crosshairs. 
	/// </summary>
	public void UpdateCrosshairs(){
		centerIPD = new Vector2 (ALPSController.screenWidthPix*0.5f + ((leftEye?-1:1) * ((deviceConfig.IPD <= deviceConfig.Width * 0.5f)?deviceConfig.GetIPDPixels()*0.5f:ALPSController.screenWidthPix*0.25f)) ,ALPSController.screenHeightPix * 0.5f);
		centerGUI = new Vector2(centerIPD.x-(arc.width * 0.5f),centerIPD.y-(arc.height * 0.5f));
		centerGUIBar = new Vector2(centerIPD.x-(bar.width * 0.5f),centerIPD.y-(bar.height * 0.5f));
	}

	/// <summary>
	/// Paints the crosshairs. 
	/// </summary>
	public void OnGUI(){
		GUI.DrawTexture(new Rect(centerGUI.x,centerGUI.y,arc.width,arc.height),arc,ScaleMode.ScaleToFit, true, 0f);
		centerGUIBar = new Vector2(centerIPD.x-((arc.width-4)*ALPSNavigation.Progress() * 0.5f),centerIPD.y-(bar.height * 0.5f));
		GUI.DrawTexture(new Rect(centerGUIBar.x,centerGUIBar.y,(arc.width-4)*ALPSNavigation.Progress(),bar.height),bar,ScaleMode.StretchToFill, true, 0f);
	}
}
