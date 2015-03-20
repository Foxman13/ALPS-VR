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

[RequireComponent (typeof (CharacterController))]
[System.Serializable]
public class ALPSNavigation : MonoBehaviour {

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	public static float pitch;
	public float forwardLowerBound 	= 20;
	public float forwardUpperBound 	= 30;
	public float forwardLimit 		= 50;
	public float backwardLowerBound = 20;
	public float backwardUpperBound = 30;
	public float backwardLimit 		= 50;

	public static float ForwardLowerBound;
	public static float ForwardUpperBound;
	public static float BackwardLowerBound ;
	public static float BackwardUpperBound;
	public static float ForwardLimit;
	public static float BackwardLimit;

    public bool EnableVibration = false;

	/**Private**/
	private CharacterController controller;
	private bool moving;
	private GameObject head;

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/// <summary>
	/// Initializes navigation system.
	/// </summary>
	public void Start () {
		ForwardLowerBound = forwardLowerBound;
		ForwardUpperBound = forwardUpperBound;
		BackwardLowerBound = 360 - backwardLowerBound;
		BackwardUpperBound = 360 - backwardUpperBound;
		ForwardLimit = forwardLimit;
		BackwardLimit = 360 - backwardLimit;

		controller = this.gameObject.GetComponent ("CharacterController") as CharacterController;
		this.gameObject.AddComponent <CharacterMotor>();
		head = GameObject.Find ("ALPSHead");
		if (Application.platform == RuntimePlatform.Android) {
			moving = false;
		}
	}

	/// <summary>
	/// Updates navigation state.
	/// </summary>
	public void Update () {
		pitch = head.transform.eulerAngles.x;
		if (pitch >= ForwardLowerBound && pitch <= ForwardUpperBound) {

            if (!moving)
            {
                if (EnableVibration)
                {
#if UNITY_ANDROID
                ALPSAndroid.Vibrate(20);
#elif UNITY_WP_8_1
                    ALPSWP8.Vibrate(200);
#endif
                }

                moving = true;
            }
			controller.Move (new Vector3 (head.transform.forward.x, 0, head.transform.forward.z) * Time.deltaTime * 3);
		} else if (pitch >= BackwardUpperBound && pitch <= BackwardLowerBound) {
            if (!moving)
            {
                if (EnableVibration)
                {
#if UNITY_ANDROID
                ALPSAndroid.Vibrate(20);
#elif UNITY_WP_8_1
                    ALPSWP8.Vibrate(200);
#endif
                }
                moving = true;
            }
			controller.Move (new Vector3 (-head.transform.forward.x, 0, -head.transform.forward.z) * Time.deltaTime * 3);
			
		} else {
            if (moving) moving = false;
		}
	}

	/// <summary>
	/// Initialize Android ALPS Activity.
	/// </summary>
	public static float Progress(){
		if (pitch >= ForwardLowerBound && pitch <= ForwardUpperBound || pitch >= BackwardUpperBound && pitch <= BackwardLowerBound) {
			return 1f;		
		} else {
			if(pitch>=0 && pitch < ForwardLowerBound) return pitch/25f;
			else if(pitch<=360 && pitch > BackwardLowerBound)return (360-pitch)/25f;
			else if(pitch>ForwardUpperBound && pitch <= ForwardLimit)return (ForwardLimit-pitch)/25f;
			else if(pitch<BackwardUpperBound && pitch >= BackwardLimit)return (pitch-BackwardLimit)/25f;
			else return 0f;
		} 
	}
}
