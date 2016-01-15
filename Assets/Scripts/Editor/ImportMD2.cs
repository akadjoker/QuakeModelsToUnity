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
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class ImportMD2  {

	[MenuItem("Assets/Djoker Tools/Import/ImportMD2")]

	static void init()
	{
		Debug.Log("load md2");

		string filename = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
	    string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));


		readMesh(rootPath,filename);
		
		//model.meshFilter.sharedMesh=Surface.createCube ("cube");

		//MeshFilter meshFilter = (MeshFilter)    ObjectRoot.AddComponent(typeof(MeshFilter));
		//MeshRenderer meshRenderer = (MeshRenderer)ObjectRoot.AddComponent(typeof(MeshRenderer));
		//meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		//meshRenderer.sharedMaterial.color = Color.white;
		//meshFilter.sharedMesh =Surface.createCube ("cube");
	}
	public static void readMesh(string path,string filename)
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
           


						GameObject ObjectRoot = new GameObject (nm);
						MD2Model model = (MD2Model)ObjectRoot.AddComponent (typeof(MD2Model));
			            model.setup();
						

			
			
				
	
						using (FileStream fs = File.OpenRead(path + "/" + filename)) 
			{
								BinaryReader file = new BinaryReader (fs);
				
								int Magic = file.ReadInt32 ();
								int Version = file.ReadInt32 ();

				                int SkinWidth = file.ReadInt32 ();
								int SkinHeight = file.ReadInt32 ();

								int FrameSize = file.ReadInt32 ();

				                int NumSkins = file.ReadInt32 ();
								int NumVertices = file.ReadInt32 ();
								int NumTexCoords = file.ReadInt32 ();
								int NumTriangles = file.ReadInt32 ();
								int NumGlCommands = file.ReadInt32 ();
								int NumFrames = file.ReadInt32 ();

								int OffsetSkins = file.ReadInt32 ();
								int OffsetTexCoords = file.ReadInt32 ();
								int OffsetTriangles = file.ReadInt32 ();
								int OffsetFrames = file.ReadInt32 ();
								int OffsetGlCommands = file.ReadInt32 ();
								int OffsetEnd = file.ReadInt32 ();



                                model.maxFrames = NumFrames;
								model.vertex_Count = NumVertices;
								model.face_Count = NumTriangles;
								model.uv_count = NumTexCoords;

				
				             file.BaseStream.Seek(OffsetTriangles,SeekOrigin.Begin);
			
				          for (int i=0; i<NumTriangles; i++) 
				            {
										int v0, v1, v2;

								 		//vertex face
					                    v0 =(int) file.ReadUInt16 ();
				                    	v1 =(int) file.ReadUInt16 ();
				                     	v2 =(int) file.ReadUInt16 ();

                    model.faces.Add(new Face (v0, v1, v2));
									//	faces.Add (new Face (v0, v1, v2));


										//texture faces
										v0 =(int) file.ReadUInt16();
					                    v1 =(int) file.ReadUInt16 ();
					                    v2 =(int) file.ReadUInt16 ();
									//	tfaces.Add (new Face (v0, v1, v2));
                    model.tfaces.Add(new Face (v0, v1, v2));

								}

				file.BaseStream.Seek(OffsetTexCoords,SeekOrigin.Begin);
				Debug.Log("Num TextureCoord"+NumTexCoords);
				for (int i=0; i<NumTexCoords; i++) 
				{ 
					float u=(float) file.ReadInt16()/SkinWidth;
					float v=(float)1*- file.ReadInt16()/SkinWidth;
					//uvCoords.Add(new Vector2(u,v));
                    model.uvCoords.Add(new Vector2(u,v));
				}  
 
				//************************************************** 
 
				file.BaseStream.Seek(OffsetFrames,SeekOrigin.Begin);
				Debug.Log("Num Frames"+NumFrames);
				for (int i=0; i<NumFrames; i++) 
				{
					Vector3 Scale=Vector3.zero;
					Vector3 Translate=Vector3.zero;

					Scale.x = file.ReadSingle();
					Scale.y = file.ReadSingle();
					Scale.z = file.ReadSingle();
					
					Translate.x = file.ReadSingle();
					Translate.y = file.ReadSingle();
					Translate.z = file.ReadSingle();

					char[] name= file.ReadChars(16);


					//Debug.LogWarning(new string(name));

					//frames[i]=new List<Vector3>();
							
					for (int j=0; j<NumVertices; j++) 
					{
						byte x = file.ReadByte();
						byte y = file.ReadByte();
						byte z = file.ReadByte();
						byte w = file.ReadByte();
				
							   
						float sx = Scale.x * x  + Translate.x;
						float sy = Scale.z * z  + Translate.z;
						float sz = Scale.y * y  + Translate.y;
						//frames[i].Add(new Vector3(sx,sy,sz));
                        model.vertex.Add(new Vector3(sx,sy,sz));
					}


				}

		
              

                Surface surface = new Surface("frame");
					
                for (int i=0; i<model.faces.Count; i++) 
					{
                    Vector3 v1 = model.vertex[0 * NumVertices+model.faces[i].v0];
                    Vector3 v2 = model.vertex[0 * NumVertices+model.faces[i].v1];
                    Vector3 v3 = model.vertex[0 * NumVertices+model.faces[i].v2];
                        
                    Vector2 uv1 = model.uvCoords[0 * NumVertices+model.tfaces[i].v0];
                    Vector2 uv2 =  model.uvCoords[0 * NumVertices+model.tfaces[i].v1];
                    Vector2 uv3 =  model.uvCoords[0 * NumVertices+model.tfaces[i].v2];
						surface.addFace(v1, v2, v3, uv1, uv2, uv3);

						
					}
					surface.build();
					surface.RecalculateNormals();
                    surface.CulateTangents();
                    model.frame=surface.getMesh();
				
			

                model.meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
                model.meshRenderer.sharedMaterial.color = Color.white;
               


                model.meshFilter.sharedMesh=model.frame;
		

                string materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + nm + "_mat.asset");
                AssetDatabase.CreateAsset(model.meshRenderer.sharedMaterial, materialAssetPath);

                string meshAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + nm + ".asset");
                AssetDatabase.CreateAsset(model.frame, meshAssetPath);
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
	}
	