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
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.IO;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteMesh : MonoBehaviour 
{
   



    List<Vector3> vertex;
    List<Vector3> normals;
    List<Vector2> uv;
    List<int> triangles;
    int VertexCount;




    private void SolveTangentsForMesh(Mesh mesh)
    {
        int vertexCount = mesh.vertexCount;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] texcoords = mesh.uv;
        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        Vector4[] tangents = new Vector4[vertexCount];
        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        int tri = 0;

        for (int i = 0; i < (triangleCount); i++)
        {
            int i1 = triangles[tri];
            int i2 = triangles[tri + 1];
            int i3 = triangles[tri + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = texcoords[i1];
            Vector2 w2 = texcoords[i2];
            Vector2 w3 = texcoords[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);
            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;

            tri += 3;
        }

        for (int i = 0; i < (vertexCount); i++)
        {
            Vector3 n = normals[i];
            Vector3 t = tan1[i];

            // Gram-Schmidt orthogonalize
            Vector3.OrthoNormalize(ref n, ref t);

            tangents[i].x = t.x;
            tangents[i].y = t.y;
            tangents[i].z = t.z;

            // Calculate handedness
            tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
   public void addTileRotate(Sprite tile,float x, float y,float width, float height, float originX, float originY,  float scaleX,float scaleY, float angle, bool FlipHorizontally, bool FlipVertically)
    {



        float worldOriginX = x + originX;
        float worldOriginY = y + originY;
        float fx = -originX;
        float fy = -originY;
        float fx2 = width - originX;
        float fy2 = height - originY;

        if (scaleX != 1 || scaleY != 1)
        {
            fx *= scaleX;
            fy *= scaleY;
            fx2 *= scaleX;
            fy2 *= scaleY;
        }


        float p1x = fx; float p1y = fy;
        float p2x = fx; float p2y = fy2;
        float p3x = fx2; float p3y = fy2;
        float p4x = fx2; float p4y = fy;



        float x1;
        float y1;
        float x2;
        float y2;
        float x3;
        float y3;
        float x4;
        float y4;


        float rotation = -angle * Mathf.PI / 180f;

        float sin = Mathf.Sin(rotation);
        float cos = Mathf.Cos(rotation);



        x1 = cos * p1x - sin * p1y;
        y1 = sin * p1x + cos * p1y;

        x2 = cos * p2x - sin * p2y;
        y2 = sin * p2x + cos * p2y;

        x3 = cos * p3x - sin * p3y;
        y3 = sin * p3x + cos * p3y;

        x4 = x1 + (x3 - x2);
        y4 = y3 - (y2 - y1);


        x1 += worldOriginX;
        y1 += worldOriginY;
        x2 += worldOriginX;
        y2 += worldOriginY;
        x3 += worldOriginX;
        y3 += worldOriginY;
        x4 += worldOriginX;
        y4 += worldOriginY;



        vertex.AddRange(new Vector3[] {
						
                    new Vector3(x1,y1,0),
                    new Vector3(x2,y2,0),
                    new Vector3(x3,y3,0),
                    new Vector3(x4,y4,0)
        });

        normals.AddRange(new Vector3[] {
						
                   Vector3.forward,
                   Vector3.forward,
                   Vector3.forward,
                   Vector3.forward
                   
        });




        int textureHeight = tile.texture.height;
        int textureWidth = tile.texture.width;


        Rect rect = tile.textureRect;



        float u1 = (rect.x / textureWidth);
        float u2 = (rect.x + rect.width) / textureWidth;
        float v1 = 1.0f - (rect.y / textureHeight);
        float v2 = 1.0f - (rect.y + rect.height) / textureHeight;
      

        uv.AddRange(new Vector2[] {
                    
                                new Vector2(swap(u1,u2,FlipHorizontally), swap(-v2,-v1,FlipVertically) ),
						    	new Vector2(swap(u1,u2,FlipHorizontally), swap(-v1,-v2,FlipVertically )),
							    new Vector2(swap(u2,u1,FlipHorizontally), swap(-v1,-v2,FlipVertically) ),
							    new Vector2(swap(u2,u1,FlipHorizontally), swap(-v2,-v1,FlipVertically) )
                       
						});


        triangles.AddRange(new int[] {
							VertexCount+0, VertexCount + 1, VertexCount + 2,
							VertexCount + 2, VertexCount + 3, VertexCount + 0,
						});

        VertexCount += 4;
    }

   public void BeginBuildMesh()
   {
       vertex = new List<Vector3>();
       normals = new List<Vector3>();
       uv = new List<Vector2>();
       triangles = new List<int>();
       VertexCount = 0;
   }


    public Mesh EndBuildMesh()
    {


     
    

        Material mat = new Material(Shader.Find("Diffuse"));
        mat.name = "tiles";
     


        Mesh mesh = new Mesh();
        mesh.name = "layer";
        mesh.vertices = vertex.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        SolveTangentsForMesh(mesh);


        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterial = mat;
    
        mesh_filter.mesh = mesh;


      

        return mesh;
    
        

    }





    float swap(float a, float b, bool _true)
    {
        if (_true)
        {
            return b;
        }
        else
        {
            return a;
        }
    }
}
