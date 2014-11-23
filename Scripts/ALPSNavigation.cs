/************************************************************************
	ALPSNavigation provides an input-free navigation system
	
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

public class ALPSNavigation : MonoBehaviour {

	private static float pitch;

	/**Private**/
	private CharacterController controller;

		
	private bool moving;


	/**Functions**/
	public void Awake () {

		controller = transform.parent.gameObject.GetComponent ("CharacterController") as CharacterController;
		transform.parent.gameObject.AddComponent ("CharacterMotor");
		if (Application.platform == RuntimePlatform.Android) {
						Debug.Log (Application.platform);
						moving = false;
				}
	}

	public void Update () {
		pitch = this.transform.eulerAngles.x;
		if (pitch >= 20 && pitch <= 30) {
			if (Application.platform == RuntimePlatform.Android){
				if (!moving){
					ALPSAndroid.Vibrate(20);
					moving = true;
				}
			}
			controller.Move (new Vector3 (transform.forward.x, 0, transform.forward.z) * Time.deltaTime * 3);
			
		} else if (pitch >= 330 && pitch <= 340) {
			if (Application.platform == RuntimePlatform.Android){
				if (!moving){
					ALPSAndroid.Vibrate(20);
					moving = true;
				}
			}
			controller.Move (new Vector3 (-transform.forward.x, 0, -transform.forward.z) * Time.deltaTime * 3);
			
		} else {
			if (Application.platform == RuntimePlatform.Android){
				if(moving)moving=false;
			}
		}
	}

	public static float progress(){
		if (pitch >= 20 && pitch <= 30 || pitch >= 330 && pitch <= 340) {
						return 1f;		
		} else {
			if(pitch>=0 && pitch < 20) return pitch/25f;
			else if(pitch<=360 && pitch > 340)return (360-pitch)/25f;
			else if(pitch>30 && pitch <= 50)return (50-pitch)/25f;
			else if(pitch<330 && pitch >= 310)return (pitch-310)/25f;
			else return 0f;
		} 
	}
}
