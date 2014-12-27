
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class MD3Model : MonoBehaviour {





	private int nextFrame;




	private int lastanimation;
	private int rollto_anim ;
	private bool RollOver;
	private float lastTime;
	private float lerptime;

	public int numTags;
	public int maxFrames;
	public int currentAnimation;
	public int currentFrame;
	public Anim currAnimation;

	public int Frame;
	public bool Manual = false;


	private bool ready = false;

	private List<Anim> animations;
	[HideInInspector]
	public List<MD3Body> bodyFrames;//track all the meshes 
	[HideInInspector]
	public List<Transform> tags;
	[HideInInspector]
	public List<GameObject> bones;



//	
	public void setup()
	{
     
		bodyFrames = new List<MD3Body> ();
		tags=new List<Transform>();
		bones=new  List<GameObject> ();
	
	}

	
	public int addAnimation(string name,int  startFrame, int endFrame, int fps)
	{
		animations.Add(new Anim(name, startFrame, endFrame, fps));
		return (animations.Count - 1);
		
	}
	public int NumAnimations()
	{
		return animations.Count - 1;
	}
	
	public void BackAnimation()
	{
		currentAnimation = (currentAnimation - 1) %  (NumAnimations());
		if (currentAnimation < 0) currentAnimation = NumAnimations();
		setAnimation(currentAnimation);
	}
	public void NextAnimation()
	{
		currentAnimation = (currentAnimation +1) %  (NumAnimations());
		if (currentAnimation >NumAnimations()) currentAnimation = 0;
		setAnimation(currentAnimation);
	}
	public void setAnimation(int num)
	{
		if (num == lastanimation) return;
		if (num > animations.Count) return;
		
		currentAnimation = num;	
		currAnimation = animations[currentAnimation];
		currentFrame = animations[currentAnimation].frameStart;
		lastanimation = currentAnimation;
	}
	public void setAnimation(string name)
	{
		
		for (int i=0;i<animations.Count;i++)
		{
			
			if (animations[i].name == name)
			{
				setAnimation(i);
				break;
			}
			
		}
		
	}
	public int getAnimation(string name)
	{
		
		for (int i=0;i<animations.Count;i++)
		{
			
			if (animations[i].name == name)
			{
				return i;
			}
			
		}
		return 0;
	}
	public void SetAnimationRollOver(int num,int next)
	{
		if (num == lastanimation) return;
		if (num > animations.Count) return;
		
		currentAnimation = num;	
		currAnimation = animations[currentAnimation];
		currentFrame = animations[currentAnimation].frameStart;
		lastanimation = currentAnimation;
		RollOver = true;
		rollto_anim = next;
	}

	void Awake () 
	{
		animations = new List<Anim> ();
		
		lastanimation = -1;
		currentAnimation = 0;
		rollto_anim = 0;
		RollOver=false;
		currentFrame = 0;
		lastTime = tickcount ();
		
		
		animations.Add(new Anim("all", 0, maxFrames-1, 15));
		
		
		currentFrame=0;
		nextFrame = 1;
		
		setAnimation(0);


 }
	// Use this for initialization
	void Start () 
	{

				ready = true;


	
	}

	private float tickcount()
	{
		return Time.time;
	}


	
	// Update is called once per frame
	void Update () 
	{

		if (maxFrames <= 1)			return;
		//if (bones.Count <= 1)						return;


		float time = tickcount ();
		float elapsedTime =  time - lastTime;
		lerptime =  tickcount() / (1.0f / currAnimation.fps);

	
	



		nextFrame = (currentFrame+1);
		if (nextFrame > currAnimation.frameEnd)
		{
			nextFrame = currAnimation.frameStart;
		}
		
		if (RollOver)
		{
			if (currentFrame >= currAnimation.frameEnd)
			{
				setAnimation(rollto_anim);
				RollOver = false;
			}
		}
		
		if (elapsedTime >= (1.0f / currAnimation.fps) )
		{
			currentFrame = nextFrame;
			lastTime = tickcount();	
		}
		if (Manual) 
		{
						currentFrame = Frame;
						nextFrame = Frame + 1;
		}


		int frameOffsetA = currentFrame * numTags;
		int frameOffsetB = nextFrame * numTags;
		
		/*
		for (int i=0;i<numTags;i++)
		{
			bones[i].transform.position=Vector3.Lerp(tags[frameOffsetA + i].position, tags[frameOffsetB + i].position, lerptime);
			bones[i].transform.rotation= Quaternion.Slerp(tags[frameOffsetA + i].rotation, tags[frameOffsetB + i].rotation, lerptime);
		}
*/

		int i=0;

		try
		{

			for ( i=0;i<numTags;i++)
			{
				int index=frameOffsetA + i;
				bones[i].transform.position=tags[index].position;
				bones[i].transform.rotation=tags[index].rotation;
			}
		}
		catch (ArgumentOutOfRangeException outOfRange)
		{
			
			Debug.LogError("Error bone tags:"+outOfRange.Message);
		}


		try
		{
		for (i=0; i<bodyFrames.Count; i++) 
		{
			bodyFrames [i].setFrame (currentFrame );
		}
		}
		catch (ArgumentOutOfRangeException outOfRange)
		{
			
			Debug.LogError("Error frames :"+outOfRange.Message);
		}



	}
}
