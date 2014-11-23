/************************************************************************
	ALPSGyro is an interface for head tracking using Android native sensors

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
using System.Runtime.InteropServices;
using System;
public class ALPSGyro : MonoBehaviour {

	/**Private**/
	private float dt_q_x = 0,dt_q_y=0, dt_q_z=0,dt_q_w=0;
	private float gr_x=0,gr_y=0,gr_z=0,gr_w=0;

	private Quaternion dt_q_gyro = Quaternion.identity;
	private Quaternion game_rotation = Quaternion.identity;
	private Quaternion landscapeLeft = Quaternion.Euler(90, 0, 0);

	/**Functions**/
	#if UNITY_ANDROID
		[DllImport("alps_native_sensor")] public static extern void  init();
		[DllImport("alps_native_sensor")] public static extern void  get_dt_gyro  (ref float dt_q_x,   ref float dt_q_y,  ref float dt_q_z, ref float dt_q_w);
		[DllImport("alps_native_sensor")] public static extern void  get_game_rotation (ref float gr_x,   ref float gr_y,  ref float gr_z, ref float gr_w);
	#endif 	
	
	public void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = 60;
		#if UNITY_ANDROID
			init ();				
		#endif
	}


	public void LateUpdate () {
		#if UNITY_ANDROID
			transform.localRotation = getOrientation();
		#endif
	}
	
	private Quaternion getOrientation ()
	{

		//get delta rotation
		get_dt_gyro(ref dt_q_x, ref dt_q_y, ref dt_q_z, ref dt_q_w);
		dt_q_gyro.x = dt_q_y;
		dt_q_gyro.y = -dt_q_x;
		dt_q_gyro.z = dt_q_z;
		dt_q_gyro.w = dt_q_w;

		//get game rotation
		get_game_rotation(ref gr_x, ref gr_y, ref gr_z, ref gr_w);
		game_rotation.x = gr_y;
		game_rotation.y = -gr_x;
		game_rotation.z = gr_z;
		game_rotation.w = gr_w;

		//Must be optimized. Delta rotation is multiplies five times to accelerate the movement.
		//This doesn't replace faster sensor polling but it makes tracking look more responsive.
		return landscapeLeft * game_rotation * dt_q_gyro;
	}
}
