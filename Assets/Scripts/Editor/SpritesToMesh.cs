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

public static class SpritesToMesh 
{


	[MenuItem("Assets/Djoker Tools/SpritesToMesh")]
	static void init()
	{
		

		
         string tag = "Convert Sprites to Quads";
        
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning(string.Format("[{0}] no game objects selected", tag));
                return;
            }

            List<GameObject> toConvert = new List<GameObject>();

            string name = selectedObjects[0].name;

            foreach (GameObject gameObject in selectedObjects) 
            {

                getObjcts(gameObject, toConvert);
                bool hasSprite = gameObject.GetComponent<SpriteRenderer>() != null;
              
                if (!hasSprite)
                {
                    continue;
                }
                toConvert.Add(gameObject);
            }

            Debug.Log(string.Format("[{0}] {1} objects {2}",tag, selectedObjects.Length, toConvert.Count));

           // List<Sprite> list = new List<Sprite>();
            GameObject ObjectRoot = new GameObject("Mesh_"+name);
            SpriteMesh objmesh = (SpriteMesh)ObjectRoot.AddComponent(typeof(SpriteMesh));

            objmesh.BeginBuildMesh();

       
     

            foreach (GameObject gameObject in toConvert)
            {
                Debug.Log("-----------------------------------");

                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                Sprite _sprite= spriteRenderer.sprite;


                 Rect r = _sprite.rect;
               Vector2 pivot = _sprite.pivot;

                float x=gameObject.transform.position.x;
                float y=gameObject.transform.position.y;

                float sx =Mathf.Abs( gameObject.transform.localScale.x);
                float sy =Mathf.Abs( gameObject.transform.localScale.y);


                bool flipx = false;

                bool flipy = false;

                if (gameObject.transform.localScale.x > 0)
                {
                    flipx = false;
                } else
                    if (gameObject.transform.localScale.x < 0)
                    {
                        flipx = true;
                    }

                if (gameObject.transform.localScale.y > 0)
                {
                    flipy = false;
                }
                else
                    if (gameObject.transform.localScale.y < 0)
                    {
                        flipy = true;
                    }







                float w=_sprite.rect.width/100;
                float h=_sprite.rect.height/100;

                float angle= -gameObject.transform.rotation.eulerAngles.z;
                

                   Debug.Log(string.Format(" sprite [{0}] ", _sprite.name));
                   Debug.Log(string.Format("[{0}] rect", _sprite.rect.ToString()));
                   Debug.Log(string.Format("[{0}] text rect", _sprite.textureRect.ToString()));
                   Debug.Log(string.Format("[{0}] tex rect off", _sprite.textureRectOffset.ToString()));
                   Debug.Log(string.Format("[{0}] pivot", _sprite.pivot.ToString()));


                   objmesh.addTileRotate(_sprite, x - (pivot.x / 100), y - (pivot.y / 100), w, h, pivot.x/100, pivot.y/100, sx, sy, angle, flipx, !flipy);
             
            }



Mesh mesh = objmesh.EndBuildMesh();


   /*
  

             string   importingAssetsDir = "Assets/Prefabs/" + name + "/";
                if (!Directory.Exists(importingAssetsDir))
                {
                    Directory.CreateDirectory(importingAssetsDir);
                }


        
       

          string materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + name + "_mat.asset");
          AssetDatabase.CreateAsset(objmesh.GetComponent<MeshRenderer>().sharedMaterial, materialAssetPath);
          
        string meshAssetPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + name + ".asset");
        AssetDatabase.CreateAsset(mesh, meshAssetPath);
        
          string prefabPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + name + ".prefab");
          var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);

          PrefabUtility.ReplacePrefab(ObjectRoot, prefab, ReplacePrefabOptions.ConnectToPrefab);
          AssetDatabase.Refresh();

        */

          //  buildMesh(list);

            Debug.Log(string.Format("[{0}] finished", tag));
        }


   
    static void getObjcts(GameObject gameObject,List<GameObject> list)
    {

              for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                     GameObject ob = gameObject.transform.GetChild(i).gameObject;
                     getObjcts(ob,list);
                     bool hasSprite = ob.GetComponent<SpriteRenderer>() != null;
                     if (!hasSprite)
                     {
                         continue;
                     }

                     list.Add(ob);
                }
    }
		
}
