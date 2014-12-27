using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class MD3Mesh 
{

	[HideInInspector]
	public List<Mesh> frames;

	public	string 	    meshID;//4					// This stores the mesh ID (We don't care)

	public	string      skin;//68;				// This stores the mesh name (We do care)
	//[HideInInspector]
	public int 		numMeshFrames;				// This stores the mesh aniamtion frame count
//	[HideInInspector]
	public int    	    numSkins;					// This stores the mesh skin count
	[HideInInspector]
	public int         numVertices;				// This stores the mesh vertex count
	[HideInInspector]
	public int 	    numTriangles;				// This stores the mesh face count
	[HideInInspector]
	public int 		triStart;					// This stores the starting offset for the triangles
	[HideInInspector]
	public int 		headerSize;					// This stores the header size for the mesh
	[HideInInspector]
	public int          TexVectorStart;					// This stores the starting offset for the UV coordinates
	[HideInInspector]
	public int 		vertexStart;				// This stores the starting offset for the vertex indices
	[HideInInspector]
	public int 		meshSize;					// This stores the total mesh size
	[HideInInspector]
	public List<Face> Faces;
	[HideInInspector]
	public List<Vector2> TexCoords;
	[HideInInspector]
	public List<Vector3> Vertex;



	public  MD3Mesh()
	{
		frames = new List<Mesh>();
		Faces  = new List<Face>();
		TexCoords = new List<Vector2>();
		Vertex = new List<Vector3>();

    }
	public void buildFrames()
	{
		for (int f=0; f<numMeshFrames; f++) 
		{
						int currentOffsetVertex = f * numVertices;

						Surface surf = new Surface ("frame_"+f);
						for (int i=0; i<Faces.Count; i++) 
			{
								surf.AddTriangle (Faces [i].v0, Faces [i].v1, Faces [i].v2);
						}

						for (int i=0; i<TexCoords.Count; i++) 
			{
				        surf.AddVertex (Vertex [currentOffsetVertex+i], TexCoords [i]);
						}
		
						surf.build ();
						surf.Optimize ();
						surf.RecalculateNormals ();
						frames.Add (surf.getMesh ());
				}
		
		/*
	
		for (int f=0; f<numMeshFrames; f++) 
		{
		 int currentOffsetVertex = f * numVertices;

		    Surface surf=new Surface("frame_"+f);
			for (int i=0; i<Faces.Count; i++) 
			{
				Vector3 v1 = Vertex[currentOffsetVertex+ Faces[i].v0];
				Vector3 v2 = Vertex[currentOffsetVertex+ Faces[i].v1];
				Vector3 v3 = Vertex[currentOffsetVertex+ Faces[i].v2];
				Vector2 uv1 = TexCoords[Faces[i].v0];
				Vector2 uv2 = TexCoords[Faces[i].v1];
				Vector2 uv3 = TexCoords[Faces[i].v2];
				surf.addFace(v1, v2, v3, uv1, uv2, uv3);
				

			}

		 for (int i=0; i<Faces.Count; i++) 
		 {
			surf.AddTriangle(Faces[i].v0,Faces[i].v1,Faces[i].v2);
		 }
		for (int i=0; i<TexCoords.Count; i++) 
		{
				surf.AddVertex(Vertex[currentOffsetVertex + i],TexCoords[i]);
		}

			surf.build();
			surf.Optimize();
			surf.RecalculateNormals();
			frames.Add(surf.getMesh());

	  }
*/


	}




}
