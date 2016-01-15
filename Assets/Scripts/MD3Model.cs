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
	public bool Manual = true;



	private List<Anim> animations;
	[HideInInspector]
	public List<MD3Body> bodyFrames;//track all the meshes 
	[HideInInspector]
    public List<Transform> tags;
	[HideInInspector]
	public List<GameObject> bones;
    [HideInInspector]
    public AnimationCompletedCallBack AnimationComplete;
    [HideInInspector]
    public AnimationCompletedCallBack AnimationChange;


	public void setup()
	{
      
		bodyFrames = new List<MD3Body> ();
        tags = new List<Transform>();
		bones=new  List<GameObject> ();
        AnimationComplete = onAnimationComplete;
        AnimationChange = onAnimationChange;
	
	}
      
	
	
     void onAnimationComplete(Anim result)
    {
    }
     void onAnimationChange(Anim result)
    {
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
    public void SetAnimationRollOver(string num, string next)
    {
        SetAnimationRollOver(getAnimation(num), getAnimation(next));
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

			


	
	}

	private float tickcount()
	{
		return Time.time;
	}


	
	// Update is called once per frame
	void Update () 
	{

      

		if (maxFrames <= 1)			return;
		

		
	
	



        if (Manual)
        {

            if (Frame <= 0) Frame = 0;
            if (Frame >= maxFrames - 1) Frame = maxFrames - 1;
            currentFrame = Frame;
            nextFrame = Frame + 1;
            animate();

        }
        else
        {
            float time = tickcount();
            float elapsedTime = time - lastTime;
            lerptime = tickcount() / (1.0f / currAnimation.fps);


            nextFrame = (currentFrame + 1);
            if (nextFrame > currAnimation.frameEnd)
            {
                if (AnimationComplete != null)
                {
                    AnimationComplete(currAnimation);
                }
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

            if (elapsedTime >= (1.0f / currAnimation.fps))
            {
                currentFrame = nextFrame;
                lastTime = tickcount();
                animate();
                if (AnimationChange != null)
                {
                    AnimationChange(currAnimation);
                }
            }
            Frame = nextFrame;
        }



      

		



	}

    void animate()
    {

        try
        {

            int frameOffsetA = currentFrame * numTags;
            int frameOffsetB = nextFrame * numTags;


            for (int i = 0; i < numTags; i++)
            {
                //int index=frameOffsetA + i;
                //bones[i].transform.position=tags[index].position;
                //bones[i].transform.rotation=tags[index].rotation;

                bones[i].transform.position = Vector3.Lerp(tags[frameOffsetA + i].position, tags[frameOffsetB + i].position, lerptime);
                bones[i].transform.rotation = Quaternion.Slerp(tags[frameOffsetA + i].rotation, tags[frameOffsetB + i].rotation, lerptime);

            }
        }
        catch (ArgumentOutOfRangeException outOfRange)
        {

            Debug.LogError("Error bone tags:" + outOfRange.Message);
        }


        try
        {
            for (int i = 0; i < bodyFrames.Count; i++)
            {

                bodyFrames[i].animate(currentFrame, nextFrame, lerptime);
            }
        }
        catch (ArgumentOutOfRangeException outOfRange)
        {

            Debug.LogError("Error frames :" + outOfRange.Message);
        }

    }
}
