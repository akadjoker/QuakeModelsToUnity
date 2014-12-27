using UnityEngine;
using System.Collections;

public class MD3Player : MonoBehaviour {


	public GameObject Lower;
	public GameObject Upper;

	public MD3Model anLower;
	public MD3Model anUpper;

	public float anglex;
	public float angley;

	public float sensitivity=10;
	public float yawSpeed=100;
	public float pitchSpeed=100;

	public GameObject Bullet ;
	public GameObject Spawn ;

	public Vector3 spwGlobal;
	public Vector3 spwLocal;
	public Vector3 spwDir;
	public float fixyaw=0;

	public float speed = 26.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;

	void Shot()
	{

		GameObject b = (GameObject)Instantiate (Bullet, spwLocal,Quaternion.identity);
		Vector3 moveTo = spwLocal + spwDir * 10000;
		b.rigidbody.AddForce(moveTo);
		//pel.rigidbody.AddForce(transform.forward * 8000);
	}
	// Use this for initialization
	void Start () 
	{


		if (Lower) 
		{

			anLower = Lower.GetComponent("MD3Model") as MD3Model;
			anLower.addAnimation("DEATH1",0,29,20);
			anLower.addAnimation("DEAD1",29,29,20);
			anLower.addAnimation("DEATH2",30,59,20);
			anLower.addAnimation("DEAD2",59,59,20);
			anLower.addAnimation("DEATH3",60,89,20);
			anLower.addAnimation("DEAD3",89,89,20);
			anLower.addAnimation("WALKC",90,97,20);
			anLower.addAnimation("WALK",98,109,12);
			anLower.addAnimation("BACK",110,120,20);
			anLower.addAnimation("SWIM",121,130,20);
			anLower.addAnimation("JUMP",131,140,15);
			anLower.addAnimation("LAND",141,150,18);
			anLower.addAnimation("JUMPB",151,156,18);
			anLower.addAnimation("LANDB",157,164,15);
			anLower.addAnimation("IDLE",165,165,15);
			anLower.addAnimation("IDLEMOVE",166,175,15);
			anLower.addAnimation("IDLECR",175,183,15);
			anLower.addAnimation("TURN",184,189,15);
			anLower.setAnimation("IDLEMOVE");
				

		}
		if (Upper) 
		{
			
			anUpper = Upper.GetComponent("MD3Model") as MD3Model;
			anUpper.addAnimation("DEATH1",0,29,20);
			anUpper.addAnimation("DEAD1",29,29,20);
			anUpper.addAnimation("DEATH2",30,59,20);
			anUpper.addAnimation("DEAD2",59,59,20);
			anUpper.addAnimation("DEATH3",60,89,20);
			anUpper.addAnimation("DEAD3",89,89,20);
			anUpper.addAnimation("GESTURE",90,126,15);
			anUpper.addAnimation("ARM",127,129,15);
			anUpper.addAnimation("ATTACK",129,135,15);
			anUpper.addAnimation("ATTACK2",136,141,15);
			anUpper.addAnimation("DROP",142,146,20);
			anUpper.addAnimation("RAISE",147,150,20);
			anUpper.addAnimation("STAND",151,151,15);
			anUpper.addAnimation("STAND2",152,152,15);
			anUpper.addAnimation("CRAZY",90,130,15);
			anUpper.setAnimation("STAND");

			
		}

	
	}
	void OnDrawGizmos() 
	{
	
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(spwLocal, 1);

		Vector3 drawTo = spwLocal + spwDir * 1000;

		Gizmos.DrawLine (spwLocal, drawTo);



	}
	// Update is called once per frame
	void Update () 
	{

				spwLocal = Spawn.transform.position;
				spwDir = Spawn.transform.TransformDirection (Vector3.forward);// 
	

				//Debug.DrawLine (spwLocal, spwDir, Color.red);


				float yawMouse = Input.GetAxis ("Mouse X");
				float pitchMouse = Input.GetAxis ("Mouse Y");



				angley += yawMouse * (Time.deltaTime * yawSpeed) * sensitivity;
				anglex += pitchMouse * (Time.deltaTime * pitchSpeed) * sensitivity;


				anglex = MathHelper.clamp (anglex, -20, 20);
				angley = MathHelper.clamp (angley, -90, 90);
	
				Upper.transform.localRotation = Quaternion.Euler (0, angley, anglex);


	
				if (Input.GetKey (KeyCode.W)) 
		       {
						moveForward (speed);
						anLower.setAnimation (anLower.getAnimation ("WALK"));
			 if (Input.GetKey (KeyCode.D)) 
			{
				transform.Rotate (0, 2, 0);
			} else
				if (Input.GetKey (KeyCode.A)) 
			{
				transform.Rotate (0, -2, 0);
			}
				
			} else
		
		if (Input.GetKey (KeyCode.D)) {
						transform.Rotate (0, 2, 0);
						anLower.setAnimation (anLower.getAnimation ("TURN"));
				} else
		if (Input.GetKey (KeyCode.A)) 
		{
						transform.Rotate (0, -2, 0);
			anLower.setAnimation (anLower.getAnimation ("TURN"));
				} else {
			anLower.setAnimation (anLower.getAnimation ("IDLEMOVE"));
				}



			

		
		if(Input.GetButtonUp("Fire1"))
		{
			anUpper.SetAnimationRollOver(anUpper.getAnimation("ATTACK"),anUpper.getAnimation("STAND"));
			Shot();
		}
	
	}
	private void moveForward(float speed) 
	{
		//Vector3 dir= Quaternion.Euler(0, fixyaw, 0) * transform.forward;
		//transform.localPosition += dir * speed * Time.deltaTime;
		transform.localPosition += transform.right * speed * Time.deltaTime;

		//rigidbody.AddForce (dir * speed);
	}
	
	private void moveBack(float speed) {
		transform.localPosition -= transform.right * speed * Time.deltaTime;
	}
	
	private void moveRight(float speed) {
		transform.localPosition += transform.forward * speed * Time.deltaTime;
	}
	
	private void moveLeft(float speed) {
		transform.localPosition -= transform.forward * speed * Time.deltaTime;
	}
}
