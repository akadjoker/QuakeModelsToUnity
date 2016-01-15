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
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class ImportMD3  {

	[MenuItem("Assets/Djoker Tools/Import/ImportMD3")]

	static void init()
	{
		
		string filename = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
	    string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));


		readModel(rootPath,filename);
		
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

        string importingAssetsDir;
     

		if (File.Exists (path + "/" + filename)) 
		{
			              string nm = Path.GetFileNameWithoutExtension (filename);

                          importingAssetsDir = "Assets/Prefabs/" + nm + "/";
                          if (!Directory.Exists(importingAssetsDir))
                          {
                              Directory.CreateDirectory(importingAssetsDir);
                          }
           
	
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
				

			
				//Debug.Log("INFO: start read tags: " + numTags);
				file.BaseStream.Seek(tagStart,SeekOrigin.Begin);

                List<GameObject> tags = new List<GameObject>();
				GameObject nodeTag=new GameObject("tags");
				nodeTag.transform.parent=model.transform;
                model.maxFrames = numBoneFrames;


     
                 



              
		
				for (int i=0;i<numBoneFrames*numTags;i++)
				{
					
					string name=readString(file,64);//file.ReadChars(64);

				

				//	Debug.LogWarning("Tag :"+tag.name);
                    
                  

					float x=file.ReadSingle();
					float z=file.ReadSingle();
					float y=file.ReadSingle();
                    Vector3 position = new Vector3(x, y, z);

				   float matrix0 = file.ReadSingle();
                   float matrix1 = file.ReadSingle();
                   float matrix2 = file.ReadSingle();

                   float matrix3 = file.ReadSingle();
                   float matrix4 = file.ReadSingle();
                   float matrix5 = file.ReadSingle();

					float ay = file.ReadSingle();
					float ax = file.ReadSingle();
					float az = file.ReadSingle();
                 
                    Quaternion  rotation = new Quaternion(ax, 0.0f, -ay, 1 + az);
                    rotation = MathHelper.NormalizeQuaternion(rotation);

                    GameObject node = new GameObject(name);
                    node.transform.position = position;
                    node.transform.rotation = rotation;
                    node.transform.parent=nodeTag.transform;
					tags.Add(node);
                    model.tags.Add(node.transform);
				}

            //    Debug.LogWarning("create bones frm tags");
				GameObject nodeBone=new GameObject("Bones");
            //    GameObject nodeBone = GameObject.CreatePrimitive(PrimitiveType.Cube);
				nodeBone.transform.parent=model.transform;
				model.numTags=numTags;


				for (int i=0;i<numTags;i++)
				{
				 GameObject tag=	tags[0 * numTags + i];
				 GameObject node =new GameObject();
                // GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				 node.name=tag.name;
                 node.transform.parent = nodeBone.transform;
                 node.transform.position=tag.transform.position;
				 node.transform.rotation=tag.transform.rotation;
				 model.bones.Add(node);
				}

            //    Debug.LogWarning("read meshes");

                GameObject meshContainer = new GameObject("MeshContainer");
                meshContainer.transform.parent = ObjectRoot.transform;

             
				int  offset =  tagEnd;
				for (int i=0; i < numMeshes;i++)
				{
					file.BaseStream.Seek(offset,SeekOrigin.Begin);
                    MD3Body b = readMesh(strFile, file, offset, model, meshContainer);
                    offset += b.meshSize;

                   

                    string materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + nm + "_mat.asset");
                    AssetDatabase.CreateAsset(b.meshRenderer.sharedMaterial, materialAssetPath);

                   string meshAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir +"_mesh_"+ nm + ".asset");
                   AssetDatabase.CreateAsset(b.frame, meshAssetPath);
       
				}


           //     Mesh m = CombineInstance(ObjectRoot, meshContainer, importingAssetsDir);



               string prefabPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + nm + ".prefab");
                var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
                PrefabUtility.ReplacePrefab(ObjectRoot, prefab, ReplacePrefabOptions.ConnectToPrefab);
                AssetDatabase.Refresh();


				Debug.LogWarning("read ok");
			}

				} else {
						Debug.LogError (filename + " dont exits");
				}
		}


    private static Mesh CombineInstance(GameObject root, GameObject obj, string path)
    {
        MeshFilter[] smRenderers = obj.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Texture2D> textures = new List<Texture2D>();
        int numSubs = 0;

        foreach (MeshFilter smr in smRenderers)
            numSubs += smr.sharedMesh.subMeshCount;



        int[] meshIndex = new int[numSubs];
        for (int s = 0; s < smRenderers.Length; s++)
        {
            MeshFilter smr = smRenderers[s];

            
            //if (smr.material.mainTexture != null)
            //    textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);

            CombineInstance ci = new CombineInstance();
            ci.mesh = smr.sharedMesh;
            meshIndex[s] = ci.mesh.vertexCount;
            ci.transform = smr.transform.localToWorldMatrix;
            combineInstances.Add(ci);


            UnityEngine.Object.DestroyImmediate(smr.gameObject);
        }

      

      

        MeshFilter r = obj.AddComponent<MeshFilter>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
    
        /*
        Texture2D skinnedMeshAtlas = new Texture2D(128, 128);
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] originalUVs = r.sharedMesh.uv;
        Vector2[] atlasUVs = new Vector2[originalUVs.Length];

        int rectIndex = 0;
        int vertTracker = 0;
        for (int i = 0; i < atlasUVs.Length; i++)
        {
            atlasUVs[i].x = Mathf.Lerp(packingResult[rectIndex].xMin, packingResult[rectIndex].xMax, originalUVs[i].x);
            atlasUVs[i].y = Mathf.Lerp(packingResult[rectIndex].yMin, packingResult[rectIndex].yMax, originalUVs[i].y);

            if (i >= meshIndex[rectIndex] + vertTracker)
            {
                vertTracker += meshIndex[rectIndex];
                rectIndex++;
            }
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "texture.asset");
        AssetDatabase.CreateAsset(skinnedMeshAtlas, assetPath);

        Material combinedMat = new Material(Shader.Find("Diffuse"));
        combinedMat.mainTexture = skinnedMeshAtlas;
        r.sharedMesh.uv = atlasUVs;
   //     r.sharedMaterial = combinedMat;

        assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "material.asset");
        AssetDatabase.CreateAsset(combinedMat, assetPath);

        */
       

        string  assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "mesh.asset");
        AssetDatabase.CreateAsset(r.sharedMesh, assetPath);


        return r.sharedMesh;
    }
    public static MD3Body readMesh(string name, BinaryReader file, int MeshOffset, MD3Model model,  GameObject meshContainer)
		{

        string meshID = readString(file, 4);	
           string skin = readString(file, 68);//68;				// This stores the mesh name (We do care)

           Debug.LogWarning("read mesh:"+skin+","+meshID);

            GameObject obj = new GameObject("Obj_Mesh");
            obj.transform.parent = meshContainer.transform;

            MD3Body mesh = (MD3Body)obj.AddComponent(typeof(MD3Body));
            mesh.name = "Mesh_" + model.bodyFrames.Count;
            mesh.setup();


            mesh.meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
            mesh.meshRenderer.sharedMaterial.color = Color.white;

  
		mesh.numMeshFrames=file.ReadInt32 ();				// This stores the mesh aniamtion frame count
		int numSkins=file.ReadInt32 ();					// This stores the mesh skin count
		mesh.numVertices=file.ReadInt32 ();				// This stores the mesh vertex count
		mesh.numTriangles=file.ReadInt32 ();				// This stores the mesh face count
		int triStart=file.ReadInt32 ();					// This stores the starting offset for the triangles
		int headerSize=file.ReadInt32 ();					// This stores the header size for the mesh
		int TexVectorStart=file.ReadInt32 ();					// This stores the starting offset for the UV coordinates
		int vertexStart=file.ReadInt32 ();				// This stores the starting offset for the vertex indices
		mesh.meshSize=file.ReadInt32();					// This stores the total mesh size

     //   Debug.LogWarning("num mesh frames"+ mesh.numMeshFrames);
     //   Debug.LogWarning("num mesh vertex" + mesh.numVertices);
     //   Debug.LogWarning("num mesh tris" + mesh.numTriangles);


     //   Debug.LogWarning("read triangles");
        file.BaseStream.Seek(MeshOffset + triStart, SeekOrigin.Begin);
		for (int i=0; i<mesh.numTriangles; i++) 
		{
			int f0=file.ReadInt32();
			int f1=file.ReadInt32();
			int f2=file.ReadInt32();
			mesh.Faces.Add(new Face(f0,f1,f2));
		}
      //  Debug.LogWarning("read text coord");
        file.BaseStream.Seek(MeshOffset + TexVectorStart, SeekOrigin.Begin);
		for (int i=0; i<mesh.numVertices; i++) 
		{
			float u=file.ReadSingle();
			float v=file.ReadSingle();
			mesh.TexCoords.Add(new Vector2(u,1*-v));
        }

     //   Debug.LogWarning("red vertices" + mesh.numVertices * mesh.numMeshFrames);

        file.BaseStream.Seek(MeshOffset + vertexStart, SeekOrigin.Begin);
		for (int i=0; i< mesh.numVertices*mesh.numMeshFrames; i++) 
		{
			float x=file.ReadInt16()/64.0f;
			float z=file.ReadInt16()/64.0f;
			float y=file.ReadInt16()/64.0f;
			float n1=file.ReadByte()/255.0f;
			float n2=file.ReadByte()/255.0f;
            Vector3 v = new Vector3(x, y, z);

   

			mesh.Vertex.Add(v);
		}


      //  Debug.LogWarning("build mesh");
		mesh.buildFrames ();
     //   Debug.LogWarning("add mesh to body");
        model.bodyFrames.Add(mesh);

    
          

        return mesh;
		}

	}
	