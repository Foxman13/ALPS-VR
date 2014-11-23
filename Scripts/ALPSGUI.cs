/************************************************************************
	ALPSGUI provides a basic menu to choose a headset

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

public class ALPSGUI : MonoBehaviour {

	/**Public**/
	public static ALPSController Controller;

	/**Private**/
	private float beginX;
	private float endX;
	private float maxOffset;
	private bool GUIVisible;
	private bool showing;
	private bool hiding;
	private GUISkin ALPSSkin;

	/**Functions**/
	void Start(){
		beginX = 0;
		endX = 0;
		maxOffset = -ALPSController.ScreenWidthPix*0.25f;
		GUIVisible = false;
		showing = false;
		hiding = false;
		ALPSSkin = Resources.Load ("ALPSSkin") as GUISkin;
		ALPSSkin.box.overflow.right = (int) maxOffset-1;
		ALPSSkin.button.overflow.right = (int) maxOffset-1;
		ALPSSkin.box.contentOffset = new Vector2((int) maxOffset-1,0);
		ALPSSkin.button.contentOffset = new Vector2((int) maxOffset-1,0);
	}

	void Update() {
		if(Input.touchCount > 0){
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				beginX = Input.GetTouch (0).position.x;
			} else if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				endX = Input.GetTouch (0).position.x;		
				if (endX - beginX >= 150) {
					SwipeRight ();
				} else if (endX - beginX <= -150) {
					SwipeLeft ();
				}
			}
		}
		if (showing) {
			int deltaOffset = (int)(-maxOffset*Time.deltaTime/0.3f);
			ALPSSkin.box.overflow.right +=deltaOffset;
			ALPSSkin.button.overflow.right +=deltaOffset;
			ALPSSkin.box.contentOffset = new Vector2(ALPSSkin.box.contentOffset.x+deltaOffset,0);
			ALPSSkin.button.contentOffset = new Vector2(ALPSSkin.button.contentOffset.x+deltaOffset,0);
			if (ALPSSkin.box.overflow.right >= 0) {
				ALPSSkin.box.overflow.right = 0;
				ALPSSkin.button.overflow.right = 0;
					ALPSSkin.box.contentOffset = new Vector2(0,0);
				ALPSSkin.button.contentOffset = new Vector2(0,0);
				showing = false;
			}
			
		} else if (hiding) {
			int deltaOffset = (int)(-maxOffset*Time.deltaTime/0.3f);
			ALPSSkin.box.overflow.right -=deltaOffset;//+= (int) (maxOffset * cumulativeTime);
			ALPSSkin.button.overflow.right -=deltaOffset;
			ALPSSkin.box.contentOffset = new Vector2(ALPSSkin.box.contentOffset.x-deltaOffset,0);
			ALPSSkin.button.contentOffset = new Vector2(ALPSSkin.button.contentOffset.x-deltaOffset,0);
			if (ALPSSkin.box.overflow.right <= maxOffset) {
				ALPSSkin.box.overflow.right = (int)maxOffset-1;
				ALPSSkin.button.overflow.right = (int)maxOffset-1;
				ALPSSkin.box.contentOffset = new Vector2((int) maxOffset-1,0);
				ALPSSkin.button.contentOffset = new Vector2((int) maxOffset-1,0);
				hiding = false;
				GUIVisible = false;
			}
		}
	}

	private void SwipeRight(){
		if (!GUIVisible) {
			GUIVisible = true;
			showing = true;
		}
	}

	private void SwipeLeft(){
		if (GUIVisible) {
			hiding = true;
		}
	}

	void OnGUI(){
		if (GUIVisible) {
			GUI.skin = ALPSSkin;
		
			// Make a background box
			GUI.Box (new Rect (0, 0, ALPSController.ScreenWidthPix * 0.25f, ALPSController.ScreenHeightPix), "Choose a device");
		
			if (GUI.Button (new Rect (0, ALPSController.ScreenHeightPix * 0.15f, ALPSController.ScreenWidthPix * 0.25f, ALPSController.ScreenHeightPix * 0.15f), "Default")) {
				Controller.setDevice (Device.DEFAULT);
			}
		
			if (GUI.Button (new Rect (0, ALPSController.ScreenHeightPix * 0.30f, ALPSController.ScreenWidthPix * 0.25f, ALPSController.ScreenHeightPix * 0.15f), "Altergaze")) {
				Controller.setDevice (Device.ALTERGAZE);
			}
		
			if (GUI.Button (new Rect (0, ALPSController.ScreenHeightPix * 0.45f, ALPSController.ScreenWidthPix * 0.25f, ALPSController.ScreenHeightPix * 0.15f), "Cardboard")) {
				Controller.setDevice (Device.CARDBOARD);
			}
		
			if (GUI.Button (new Rect (0, ALPSController.ScreenHeightPix * 0.60f, ALPSController.ScreenWidthPix * 0.25f, ALPSController.ScreenHeightPix * 0.15f), "Firefly VR")) {
				Controller.setDevice (Device.FIREFLY);
			}
		}
	}
}
