using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float  SecondsUntilDestroy  = 10;
	private float startTime ;


	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Time.time - startTime >= SecondsUntilDestroy)
		{
			Destroy(this.gameObject);
		}
	}
}

