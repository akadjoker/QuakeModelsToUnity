using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	public GameObject target;
	public float damping = 1;
	public float yaw = 0;
	public float pitch = 0;
	public float roll = 0;
	Vector3 vtarget;
	public float currentAngle = 0;
	public Vector3 offset;
	public bool swap = false;

	
	void Start() 
	{
		vtarget = target.transform.position - transform.position;
	}
	
	void LateUpdate() 
	{
	
		currentAngle = transform.eulerAngles.y;
		float desiredAngle = target.transform.eulerAngles.y;
		float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
		
		Quaternion rotation = Quaternion.Euler(roll, angle-yaw, pitch);
		if (swap) 
		{
			transform.position = target.transform.position + (rotation * vtarget) + offset;
			
		} else {
			
			transform.position = target.transform.position - (rotation * vtarget) + offset;
		}



		
		transform.LookAt(target.transform);
	}
}