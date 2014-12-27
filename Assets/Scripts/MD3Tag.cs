using UnityEngine;
using System.Collections;

public class MD3Tag  {

	public Vector3 position;
	public string name;
	public Vector3 axis;
	public Quaternion rotation;
	public GameObject node;
	public float[] matrix;

	public MD3Tag()
	{
		position = Vector3.zero;
		axis= Vector3.zero;
		matrix = new float[9];
	}


	public GameObject update()
	{

		//rotation = new Quaternion (matrix[6],0.0f,-matrix[7],1 + matrix[8]);
		rotation = new Quaternion (axis.x,0.0f,-axis.y,1 + axis.z);


		rotation=MathHelper.NormalizeQuaternion (rotation);

		GameObject node = new GameObject (name);
		//GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
		node.name = name;

		node.transform.position = position;
		node.transform.rotation = rotation;	
		return node;
   }

}
