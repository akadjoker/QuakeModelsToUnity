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

				if (File.Exists (path + "/" + filename)) {
						string nm = Path.GetFileNameWithoutExtension (filename);
						GameObject ObjectRoot = new GameObject (nm);
						MD2Model model = (MD2Model)ObjectRoot.AddComponent (typeof(MD2Model));
			            model.setup();
						

			
			
						
						List<Face> faces = new List<Face> ();
						List<Face> tfaces = new List<Face> ();
						List<Vector2> uvCoords = new List<Vector2> ();
			          	Dictionary<int, List<Vector3>> frames =new Dictionary<int, List<Vector3>>();
			
	
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

			
				 int vertex_Count;
				 int uv_count;
				 int face_Count;
				 int frames_Count;
								frames_Count = NumFrames;
								vertex_Count = NumVertices;
								face_Count = NumTriangles;
								uv_count = NumTexCoords;

				Debug.Log("Num Vertices "+NumVertices);
			
				             file.BaseStream.Seek(OffsetTriangles,SeekOrigin.Begin);
				Debug.Log("Num triangles"+NumTriangles);

				          for (int i=0; i<NumTriangles; i++) 
				            {
										int v0, v1, v2;

								 		//vertex face
					                    v0 =(int) file.ReadUInt16 ();
				                    	v1 =(int) file.ReadUInt16 ();
				                     	v2 =(int) file.ReadUInt16 ();
					
										faces.Add (new Face (v0, v1, v2));


										//texture faces
										v0 =(int) file.ReadUInt16();
					                    v1 =(int) file.ReadUInt16 ();
					                    v2 =(int) file.ReadUInt16 ();
										tfaces.Add (new Face (v0, v1, v2));

								}

				file.BaseStream.Seek(OffsetTexCoords,SeekOrigin.Begin);
				Debug.Log("Num TextureCoord"+NumTexCoords);
				for (int i=0; i<NumTexCoords; i++) 
				{ 
					float u=(float) file.ReadInt16()/SkinWidth;
					float v=(float)1*- file.ReadInt16()/SkinWidth;
					uvCoords.Add(new Vector2(u,v));
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

					frames[i]=new List<Vector3>();
							
					for (int j=0; j<NumVertices; j++) 
					{
						byte x = file.ReadByte();
						byte y = file.ReadByte();
						byte z = file.ReadByte();
						byte w = file.ReadByte();
				
							   
						float sx = Scale.x * x  + Translate.x;
						float sy = Scale.z * z  + Translate.z;
						float sz = Scale.y * y  + Translate.y;
						frames[i].Add(new Vector3(sx,sy,sz));

					}


				}

		

				for (int f=0; f<NumFrames; f++) 
				{
					Surface surface= new Surface("frame_"+f);
					for (int i=0; i<faces.Count; i++) 
					{
						Vector3 v1 = frames[f][faces[i].v0];
						Vector3 v2 = frames[f][faces[i].v1];
						Vector3 v3 = frames[f][faces[i].v2];
						Vector2 uv1 = uvCoords[tfaces[i].v0];
						Vector2 uv2 = uvCoords[tfaces[i].v1];
						Vector2 uv3 = uvCoords[tfaces[i].v2];
						surface.addFace(v1, v2, v3, uv1, uv2, uv3);

						
					}
					surface.build();
					surface.RecalculateNormals();
					model.frames.Add(surface.getMesh());
				}

			



				model.meshFilter.sharedMesh=model.frames[0];
				model.ready=true;


				Debug.LogWarning("read ok");
			}

				} else {
						Debug.LogError (filename + " dont exits");
				}
		}
	}
	