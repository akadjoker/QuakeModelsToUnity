/*
* 
* Copyright (c) 2015 Luis Santos AKA DJOKER
 * 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class MD3Body : MonoBehaviour {

 
    [HideInInspector]
    public int numMeshFrames;				// This stores the mesh aniamtion frame count
    [HideInInspector]
    public int numVertices;				// This stores the mesh vertex count
    [HideInInspector]
    public int numTriangles;				// This stores the mesh face count
     [HideInInspector]
    public int meshSize;					// This stores the total mesh size
    [HideInInspector]
    public List<Face> Faces;
    [HideInInspector]
    public List<Vector2> TexCoords;
    [HideInInspector]
    public List<Vector3> Vertex;
    [HideInInspector]
    public Mesh frame;
    [HideInInspector]
	public MeshFilter meshFilter;
    [HideInInspector]
	public MeshRenderer meshRenderer;





    public void setup()
    {

        Faces = new List<Face>();
        TexCoords = new List<Vector2>();
        Vertex = new List<Vector3>();

        meshFilter = (MeshFilter)this.transform.gameObject.AddComponent(typeof(MeshFilter));
        meshRenderer = (MeshRenderer)this.transform.gameObject.AddComponent(typeof(MeshRenderer));

    }
    public void buildFrames()
    {

        int uv_count = TexCoords.Count;
        Surface surf = new Surface("SURFACE");

        try
		{


       //     Debug.LogWarning("faces total:" + Faces.Count);
       //     Debug.LogWarning("vertex total:" + Vertex.Count);


        for (int i = 0; i < Faces.Count ; i++)
        {

            int i1 = Faces[i].v0;
            int i2 = Faces[i].v1;
            int i3 = Faces[i].v2;

            Vector3 v1 = Vertex[i1];
            Vector3 v2 = Vertex[i2];
            Vector3 v3 = Vertex[i3];

            Vector2 uv1 =  TexCoords[0 * uv_count + i1];
            Vector2 uv2 = TexCoords[0 * uv_count + i2];
            Vector2 uv3 =  TexCoords[0 * uv_count + i3];
            surf.addFace(v1, v2, v3, uv1, uv2, uv3);

        }

        surf.build();
        surf.Optimize();
        surf.RecalculateNormals();
        surf.CulateTangents();
        frame = surf.getMesh();
        meshFilter.sharedMesh = frame;

        }
        catch (ArgumentOutOfRangeException outOfRange)
        {

            Debug.LogError("Error mesh build:" + outOfRange.Message);
        }

     
    }

   public void animate(int currentFrame,int nextFrame, float poll)
    {

        int currentOffsetVertex = currentFrame * numVertices;
        int nextCurrentOffsetVertex = nextFrame * numVertices;

        Vector3[] vertices= frame.vertices;


        int index=0;
        for (int i=0; i<Faces.Count; i++) 
        {
            int i0 = Faces[i].v0;
            int i1 = Faces[i].v1;
            int i2 = Faces[i].v2;


            Vector3 v1 = Vector3.Lerp(Vertex[currentOffsetVertex + i0], Vertex[nextCurrentOffsetVertex + i0], poll);
            Vector3 v2 = Vector3.Lerp(Vertex[currentOffsetVertex + i1], Vertex[nextCurrentOffsetVertex + i1], poll);
            Vector3 v3 = Vector3.Lerp(Vertex[currentOffsetVertex + i2], Vertex[nextCurrentOffsetVertex + i2], poll);

            vertices[index++] = v1;
            vertices[index++] = v2;
            vertices[index++] = v3;
        }

        frame.vertices = vertices;
        frame.RecalculateBounds();
        frame.RecalculateNormals();
    }
}
