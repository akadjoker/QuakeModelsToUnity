using UnityEngine;
using System.Collections;

public class Face  {

	public int v0;
	public int v1;
	public int v2;
	public int[] indices;


	public Face (int f0,int f1,int f2) 
	{
		v0 = f0;
		v1 = f1;
		v2 = f2;
		indices = new int[3];
		indices [0] = f0;
		indices [1] = f1;
		indices [2] = f2;
	
	}
	

}
