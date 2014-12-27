using UnityEngine;
using UnityEditor;
using System.Collections;

public static class CreateCube  {
	
	[MenuItem("Assets/Djoker Tools/Primitives/CreateCube")]
	
	static void init()
	{
		Debug.Log("create cube");
		GameObject ObjectRoot = new GameObject("cube");
		MeshFilter meshFilter = (MeshFilter)    ObjectRoot.AddComponent(typeof(MeshFilter));
		MeshRenderer meshRenderer = (MeshRenderer)ObjectRoot.AddComponent(typeof(MeshRenderer));
		meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		meshRenderer.sharedMaterial.color = Color.white;
		meshFilter.sharedMesh =Surface.createCube ("cube");
	}
	
}
