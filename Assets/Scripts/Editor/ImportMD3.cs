using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class ImportMD3  {

	[MenuItem("Assets/Djoker Tools/Import/ImportMD3")]

	static void init()
	{
		Debug.Log("load md3");

		string filename = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
	    string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));


		readModel(rootPath,filename);
		
		//model.meshFilter.sharedMesh=Surface.createCube ("cube");

		//MeshFilter meshFilter = (MeshFilter)    ObjectRoot.AddComponent(typeof(MeshFilter));
		//MeshRenderer meshRenderer = (MeshRenderer)ObjectRoot.AddComponent(typeof(MeshRenderer));
		//meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		//meshRenderer.sharedMaterial.color = Color.white;
		//meshFilter.sharedMesh =Surface.createCube ("cube");
	}


	private static string readString(BinaryReader file,int count)
	{

		int i = 0;
		byte[] b = file.ReadBytes (count);
		char[] namestr = new char[count];
		while ((i < b.Length) && (b[i] != 0)) 
		{
			namestr[i]=Convert.ToChar(b[i]);
			i++;
		}

		string name = new string (namestr);
		name.Trim ();

		return name;
	}

	public static void readModel(string path,string filename)
	{

		if (File.Exists (path + "/" + filename)) 
		{
			              string nm = Path.GetFileNameWithoutExtension (filename);	
	
			using (FileStream fs = File.OpenRead(path + "/" + filename)) 
			{
								BinaryReader file = new BinaryReader (fs);


				
				char[] fileid=file.ReadChars(4);//4 IDP3
				string header=new string(fileid);
				if(header!="IDP3")
				{
					Debug.LogError(filename+" not md3 model");
					return;
				}



					GameObject ObjectRoot = new GameObject (nm);
					MD3Model model = (MD3Model)ObjectRoot.AddComponent (typeof(MD3Model));
				    model.setup();
				   
			
			


				int Version=file.ReadInt32();//15
				string strFile=readString(file,68);//file.ReadChars(68); //68

			
				
				int numBoneFrames=file.ReadInt32();//num keyframes
				int numTags = file.ReadInt32();//number tags per frame
				int numMeshes = file.ReadInt32();//number of mehs/skins
				int numMaxSkins = file.ReadInt32();////maximum number of unique skins used in md3 file.
				
				int frameStart = file.ReadInt32();//starting position of frame-structur
				
				int tagStart = file.ReadInt32();//starting position of tag-structures
				int tagEnd = file.ReadInt32();//ending position of tag-structures/starting position of mesh-structures
				
				int fileSize = file.ReadInt32();
				
			
				Debug.Log("INFO: start read tags: " + numTags);
				file.BaseStream.Seek(tagStart,SeekOrigin.Begin);

				List<MD3Tag> tags=new List<MD3Tag>();

				GameObject nodeTag=new GameObject("tags");
				nodeTag.transform.parent=model.transform;
	

			
			

		
				for (int i=0;i<numBoneFrames*numTags;i++)
				{
					MD3Tag tag= new MD3Tag();
					tag.name=readString(file,64);//file.ReadChars(64);

				

					Debug.LogWarning("Tag :"+tag.name);


					tag.position.x=file.ReadSingle();
					tag.position.z=file.ReadSingle();
					tag.position.y=file.ReadSingle();


				    tag.matrix[0] = file.ReadSingle();
					tag.matrix[1] = file.ReadSingle();
					tag.matrix[2] = file.ReadSingle();
					tag.matrix[3] = file.ReadSingle();
					tag.matrix[4] = file.ReadSingle();
					tag.matrix[5] = file.ReadSingle();
					tag.axis.y = file.ReadSingle();
					tag.axis.x = file.ReadSingle();
					tag.axis.z = file.ReadSingle();

					/*
					Matrix4x4 rotation = Matrix4x4.identity;
					rotation.m11 = file.ReadSingle();
					rotation.m12 = file.ReadSingle();
					rotation.m13 = file.ReadSingle();
					rotation.m21 = file.ReadSingle();
					rotation.m22 = file.ReadSingle();
					rotation.m23 = file.ReadSingle();
					rotation.m31 = file.ReadSingle();
					rotation.m32 = file.ReadSingle();
					rotation.m33 = file.ReadSingle();
*/
				//	tag.rotation= MathHelper.CreateFromRotationMatrix(rotation);
			
					GameObject node= tag.update();//rotate quaternion
					node.transform.parent=nodeTag.transform;
					tags.Add(tag);

					model.tags.Add(node.transform);



				}

				GameObject nodeBone=new GameObject("Bones");
				nodeBone.transform.parent=model.transform;
				model.numTags=numTags;


				for (int i=0;i<numTags;i++)
				{
				 MD3Tag tag=	tags[0 * numTags +i];
				// GameObject node =GameObject.CreatePrimitive(PrimitiveType.Cube);
				 GameObject node =new GameObject();
				 node.name=tag.name;
				 node.transform.parent=nodeBone.transform;

					//if coment this the bone is 0 but on updade is on location
					//have to reset the child trasfomation
				 node.transform.position=tag.position;
				 node.transform.rotation=tag.rotation;

				 model.bones.Add(node);

				}


				List<MD3Mesh> meshses=new List<MD3Mesh>();
			
				int  offset =  tagEnd;
				for (int i=0;i<numMeshes;i++)
				{
					file.BaseStream.Seek(offset,SeekOrigin.Begin);
					MD3Mesh mesh= readMesh(file,offset,model);
					meshses.Add(mesh);
					offset+=mesh.meshSize;
				}


				for (int i=0;i<meshses.Count;i++)
				{
					MD3Mesh mesh=meshses[i];
					GameObject Node = new GameObject (strFile+"_"+i);
					Node.transform.parent=ObjectRoot.transform;
					MD3Body body = (MD3Body)Node.AddComponent (typeof(MD3Body));
					body.name=mesh.skin+"_"+i;
					body.setup();
					for (int j=0;j<mesh.frames.Count;j++)
					{
						body.frames.Add(mesh.frames[j]);
				    }

					body.meshFilter.sharedMesh=body.frames[0];
					model.bodyFrames.Add(body);
				}

				model.maxFrames=numBoneFrames;


				//model.meshFilter.sharedMesh=model.frames[0];



				Debug.LogWarning("read ok");
			}

				} else {
						Debug.LogError (filename + " dont exits");
				}
		}
		


	public static MD3Mesh readMesh(BinaryReader file,int MeshOffset,MD3Model model)
		{

		MD3Mesh mesh = new MD3Mesh();
	


		mesh.meshID = readString (file, 4);					// This stores the mesh ID (We don't care)
		mesh.skin=readString(file,68);//68;				// This stores the mesh name (We do care)

	
		mesh.numMeshFrames=file.ReadInt32 ();				// This stores the mesh aniamtion frame count
		mesh.numSkins=file.ReadInt32 ();					// This stores the mesh skin count
		mesh.numVertices=file.ReadInt32 ();				// This stores the mesh vertex count
		mesh.numTriangles=file.ReadInt32 ();				// This stores the mesh face count
		mesh.triStart=file.ReadInt32 ();					// This stores the starting offset for the triangles
		mesh.headerSize=file.ReadInt32 ();					// This stores the header size for the mesh
		mesh.TexVectorStart=file.ReadInt32 ();					// This stores the starting offset for the UV coordinates
		mesh.vertexStart=file.ReadInt32 ();				// This stores the starting offset for the vertex indices
		mesh.meshSize=file.ReadInt32();					// This stores the total mesh size

		file.BaseStream.Seek(MeshOffset + mesh.triStart,SeekOrigin.Begin);
		for (int i=0; i<mesh.numTriangles; i++) 
		{
			int f0=file.ReadInt32();
			int f1=file.ReadInt32();
			int f2=file.ReadInt32();
			mesh.Faces.Add(new Face(f0,f1,f2));
		}

		file.BaseStream.Seek(MeshOffset + mesh.TexVectorStart,SeekOrigin.Begin);
		for (int i=0; i<mesh.numVertices; i++) 
		{
			float u=file.ReadSingle();
			float v=file.ReadSingle();
			mesh.TexCoords.Add(new Vector2(u,1*-v));
		}

		file.BaseStream.Seek(MeshOffset + mesh.vertexStart,SeekOrigin.Begin);
		for (int i=0; i<mesh.numVertices*mesh.numMeshFrames; i++) 
		{
			float x=file.ReadInt16()/64.0f;
			float z=file.ReadInt16()/64.0f;
			float y=file.ReadInt16()/64.0f;
			float n1=file.ReadByte()/255.0f;
			float n2=file.ReadByte()/255.0f;
			mesh.Vertex.Add(new Vector3(x,y,z));
		}

	
		mesh.buildFrames ();
	
		return mesh;
		}

	}
	