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


public static class Export2DColiders 
{
 
    [MenuItem("Assets/Djoker Tools/Export2DColiders")]
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

    

        foreach (GameObject gameObject in selectedObjects) 
        {

            getObjcts(gameObject, toConvert);
            bool hasBox = gameObject.GetComponent<BoxCollider2D>() != null;
            bool hasCircle = gameObject.GetComponent<CircleCollider2D>() != null;
            if (!hasBox)
            {
                continue;
            }

           
            toConvert.Add(gameObject);
        }

        Debug.Log(string.Format("[{0}] {1} objects {2}",tag, selectedObjects.Length, toConvert.Count));

        // List<Sprite> list = new List<Sprite>();
        GameObject ObjectRoot = new GameObject("ColideLayer");







        foreach (GameObject gameObject in toConvert)
        {
            Debug.Log("-----------------------------------");

          

            bool hasBox = gameObject.GetComponent<BoxCollider2D>() != null;
            bool hasCircle = gameObject.GetComponent<CircleCollider2D>() != null;

            Vector3 pos = gameObject.transform.position;

            float x = pos.x;
            float y = pos.y;


            if (hasBox)
            {
                BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
                BoxCollider2D add_box=ObjectRoot.AddComponent<BoxCollider2D>();
                add_box.size = new Vector2(box.size.x, box.size.y);

                if (gameObject.transform.parent != null)
                {

                    add_box.offset = new Vector2(x - box.offset.x, y - box.offset.y);
                }
                else
                {
                    add_box.offset = new Vector2( box.offset.x, box.offset.y);
                }
     
           
                

                

                


            }
            if (hasCircle)
            {
                CircleCollider2D circle= gameObject.GetComponent<CircleCollider2D>();
                CircleCollider2D add_circle= gameObject.GetComponent<CircleCollider2D>();
                add_circle.offset = circle.offset;
                add_circle.radius = circle.radius;
                


            }


        }





        Debug.Log(string.Format("[{0}] finished", tag));
    }



    static void getObjcts(GameObject gameObject,List<GameObject> list)
    {

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject ob = gameObject.transform.GetChild(i).gameObject;
            getObjcts(ob,list);
            bool hasBox = ob.GetComponent<BoxCollider2D>() != null;
            bool hasCircle = ob.GetComponent<CircleCollider2D>() != null;
            if (!hasBox )
            {
                continue;
            }

            list.Add(ob);
        }
    }
}
