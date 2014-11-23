/************************************************************************
	ALPSBarrelMesh is a factory which creates barrel shaped meshes 

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

public class ALPSBarrelMesh {

	/**Public**/
	public static ALPSConfig DeviceConfig;

	/**Functions**/
	public static Mesh GenerateMesh(int lines, int columns,bool leftEye){
		float k1, k2;
		if (!DeviceConfig.EnableBarrelDistortion) {
			k1 = k2 = 0;		
		} else {
			k1 = DeviceConfig.k1;
			k2 = DeviceConfig.k2;
		}
		int numVertices = (lines + 1) * (columns + 1);
		int numFaces = lines * columns;
		Mesh mesh = new Mesh ();
		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] tri = new int[numFaces*6];
		//If IPD is smaller than half of the width, we take width/2 for IPD
		//Otherwise meshes are outward-oriented
		float widthIPDRatio = (DeviceConfig.IPD <= DeviceConfig.Width * 0.5f || DeviceConfig.FixedSize)?(DeviceConfig.IPD / DeviceConfig.Width) : 0.5f;
		Vector2 center = new Vector2 (leftEye?1-widthIPDRatio:widthIPDRatio,0.5f);
		int x, y;

		//Creation of the vertices
		int numQuad = 0;
		float maxX = (leftEye?1:0);
		float maxY = 0;
		for (y=0; y<=lines; y++) {
			for(x=0; x<=columns; x++){
				int index = y*(lines+1)+x;
				Vector2 vertex = new Vector2();

				float rSqr = Mathf.Pow (center.x-((float)x/(float)columns),2) + Mathf.Pow (center.y-((float)y/(float)lines),2);
				float rMod = 1+k1*rSqr+k2*rSqr*rSqr;

				vertex.x = (float)((float)x/(float)columns-center.x)/(float)rMod+center.x-0.5f;
				vertex.y = (float)((float)y/(float)lines-center.y)/(float)rMod+center.y-0.5f;
				if(leftEye){
					if(vertex.x<maxX)maxX=vertex.x;
				}else{
					if(vertex.x>maxX)maxX=vertex.x;
				}
				if(vertex.y>maxY)maxY=vertex.y;
				vertices[index] = new Vector3(vertex.x, vertex.y, 0);
				uvs[index] = new Vector2((float)x/(float)columns,(float)y/(float)lines);

				if(x<columns && y<lines){
					int v;
					for(v=0;v<6;v++){
						tri[numQuad*6+v] = index + ((v>=2 && v!=3) ? columns : 0) + ((v==0) ? 0 : (v/5)+1);
					}
					numQuad++;
				}
			}
		}

		float scaleFactor = 1f/Mathf.Max(leftEye?-1*(1-maxX):maxX,maxY);

		int i;
		for(i=0; i<vertices.Length; i++) {
			vertices[i]*=scaleFactor;
		}
	
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = tri;
		mesh.RecalculateNormals();
		return mesh;
	}
}
