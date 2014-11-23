/************************************************************************
	ALPSCamera manages both cameras required for stereo vision
	
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

public class ALPSCamera : MonoBehaviour{

	/**Public**/
	public static ALPSConfig DeviceConfig;
	public bool LeftEye;

	/**Private**/
	private Mesh mesh;


	/**Functions**/
	public void Init(){

		Vector3 camLeftPos = camera.transform.localPosition; 
		camLeftPos.x = (LeftEye?-1:1)*DeviceConfig.ILD * 0.0005f;
		camLeftPos.z = ALPSConfig.NeckPivotToEye.x * 0.001f;
		camLeftPos.y = ALPSConfig.NeckPivotToEye.y * 0.001f;
		
		camera.transform.localPosition = camLeftPos;
	}

	public void updateMesh(){
		
		camera.rect = new Rect ((LeftEye?0f:0.5f),0f,0.5f,1f);
		camera.aspect = DeviceConfig.Width*0.5f / DeviceConfig.Height;

		mesh = ALPSBarrelMesh.GenerateMesh(20,20,LeftEye);
	}

	public void Draw(){

		Graphics.DrawMeshNow (mesh,Camera.current.transform.position,Camera.current.transform.rotation);
	}


}