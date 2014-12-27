using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MD3Body : MonoBehaviour {

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	[HideInInspector]
	public List<Mesh> frames;
	public int numFrames;
	//public int frame;

	public void setup()
	{
		meshFilter = (MeshFilter)    this.transform.gameObject.AddComponent(typeof(MeshFilter));
		meshRenderer = (MeshRenderer)this.transform.gameObject.AddComponent(typeof(MeshRenderer));
		meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		meshRenderer.sharedMaterial.color = Color.white;
		frames = new List<Mesh> ();
		//frame = 0;
	}
	void Start () 
	{
		numFrames = frames.Count-1;
	}
	void Update () 
	{
		//setFrame (frame);
	}
	public void setFrame(int frame)
	{
		if (frame < 0)	frame = 0;
		if (frame > frames.Count)	frame = numFrames;
		meshFilter.sharedMesh=frames[frame % numFrames];
	}
}
