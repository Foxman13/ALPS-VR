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

	//=====================================================================================================
	// Attributes
	//=====================================================================================================

	/**Public**/
	public static ALPSConfig deviceConfig;
	public bool leftEye;

	/**Private**/
	private Mesh mesh;

	//=====================================================================================================
	// Functions
	//=====================================================================================================

	/// <summary>
	/// Initializes the camera.
	/// </summary>
	public void Init(){
		Vector3 camLeftPos = GetComponent<Camera>().transform.localPosition; 
		camLeftPos.x = (leftEye?-1:1) * deviceConfig.ILD * 0.0005f;
		camLeftPos.z = ALPSConfig.neckPivotToEye.x * 0.001f;
		camLeftPos.y = ALPSConfig.neckPivotToEye.y * 0.001f;
		GetComponent<Camera>().transform.localPosition = camLeftPos;
	}

	/// <summary>
	/// Updates the mesh used for barrel distortion.
	/// </summary>
	public void UpdateMesh(){
		GetComponent<Camera>().rect = new Rect ((leftEye?0f:0.5f),0f,0.5f,1f);
		GetComponent<Camera>().aspect = deviceConfig.Width*0.5f / deviceConfig.Height;
		mesh = ALPSBarrelMesh.GenerateMesh(20,20,leftEye);
	}

	/// <summary>
	/// Draws render texture on mesh.
	/// </summary>
	public void Draw(){
		Graphics.DrawMeshNow (mesh,Camera.current.transform.position,Camera.current.transform.rotation);
	}
}