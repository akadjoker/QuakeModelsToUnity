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

public class MeshCombine : ScriptableWizard
{
    public GameObject meshToCombine;
    [MenuItem("Assets/Djoker Tools/Mesh Combine/Combine Meshes")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Combine Mesh", typeof(MeshCombine));
    }
    void OnWizardCreate()
    {
        if (meshToCombine != null)
        {



            MeshFilter[] meshFilters = meshToCombine.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.active = false;
                i++;
            }

            GameObject combinedMesh = new GameObject("CombinedMesh");



            combinedMesh.AddComponent(typeof(MeshFilter));
            combinedMesh.AddComponent(typeof(MeshRenderer));
            combinedMesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            combinedMesh.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);


            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + "CombinedMesh" + ".prefab");
            PrefabUtility.ReplacePrefab(combinedMesh, prefab, ReplacePrefabOptions.ConnectToPrefab);




        }
    }
}
