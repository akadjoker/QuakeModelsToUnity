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


public class MD2Model : MonoBehaviour {

    [HideInInspector]
	public MeshFilter meshFilter;
    [HideInInspector]
	public MeshRenderer meshRenderer;






	public int currentFrame;
	private int nextFrame;

	public Anim currAnimation;



	private int lastanimation;
	public int currentAnimation;
	private int rollto_anim ;
	private bool RollOver;
	private float lastTime;
   	private float lerptime;

    public bool useKesy = false;
    public int Frame;
    public bool Manual = false;


	[HideInInspector]
	private List<Anim> animations;
    [HideInInspector]
    public List<Vector3> vertex;
    [HideInInspector]
    public List<Vector2> uvCoords;
    [HideInInspector]
    public List<Face> faces;
    [HideInInspector]
    public List<Face> tfaces;
    [HideInInspector]
    public int vertex_Count;
    [HideInInspector]
    public int uv_count;
    [HideInInspector]
    public int face_Count;

    public int maxFrames;


    [HideInInspector]
    public AnimationCompletedCallBack AnimationComplete;
    [HideInInspector]
    public AnimationCompletedCallBack AnimationChange;


	[HideInInspector]
	public Mesh frame;

	public  void setup()
	{
		 meshFilter = (MeshFilter)    this.transform.gameObject.AddComponent(typeof(MeshFilter));
		 meshRenderer = (MeshRenderer)this.transform.gameObject.AddComponent(typeof(MeshRenderer));
	//	 meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
	//	 meshRenderer.sharedMaterial.color = Color.white;

        vertex = new List<Vector3>();
        uvCoords= new List<Vector2> ();
        faces= new List<Face>();
        tfaces= new List<Face>();

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
	public void setAnimationByName(string name)
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


	// Use this for initialization
	void Start () 
	{

		animations = new List<Anim> ();
	
		lastanimation = -1;
		currentAnimation = 0;
		rollto_anim = 0;
		RollOver=false;
		currentFrame = 0;
		lastTime = tickcount ();
	
		animations.Add(new Anim("stand", 0, 39, 9));
		animations.Add(new Anim("run", 40, 45, 10));
		animations.Add(new Anim("attack", 46, 53, 10));
		animations.Add(new Anim("pain_a", 54, 57, 7));
		animations.Add(new Anim("pain_b", 58, 61, 7));
		animations.Add(new Anim("pain_c", 62, 65, 7));
		animations.Add(new Anim("jump", 66, 71, 7));
		animations.Add(new Anim("flip", 72, 83, 7));
		animations.Add(new Anim("salute", 84, 94, 7));
		animations.Add(new Anim("fallback", 95, 111, 10));
		animations.Add(new Anim("wave", 112, 122, 7));
		animations.Add(new Anim("point", 123, 134, 6));
		animations.Add(new Anim("crouch_stand", 135, 153, 10));
		animations.Add(new Anim("crouch_walk", 154, 159, 7));
		animations.Add(new Anim("crouch_attack", 160, 168, 10));
		animations.Add(new Anim("crouch_pain", 169, 172, 7));
		animations.Add(new Anim("crouch_death", 173, 177, 5));
		animations.Add(new Anim("death_fallback", 178, 183, 7));
		animations.Add(new Anim("death_fallbackforward", 184, 189, 7));
		animations.Add(new Anim("death_fallbackslow", 190, 197, 7));
		animations.Add(new Anim("boom", 198, 198, 5));
		animations.Add(new Anim("all", 0, 198, 15));
		
		currentFrame=0;
		nextFrame = 1;

		setAnimation(0);
	

	
	
	}

	private float tickcount()
	{
		return Time.time;
	}


    void animate(int currentFrame,int nextFrame, float poll)
    {

        int currentOffsetVertex = currentFrame * vertex_Count;
        int nextCurrentOffsetVertex = nextFrame * vertex_Count;
   
        Vector3[] vertices= frame.vertices;


        int index=0;
        for (int i=0; i<faces.Count; i++) 
        {
            int i0 = faces[i].v0;
            int i1 = faces[i].v1;
            int i2 = faces[i].v2;


              Vector3 v1 = Vector3.Lerp(vertex[currentOffsetVertex + i0], vertex[nextCurrentOffsetVertex + i0], poll);
              Vector3 v2 = Vector3.Lerp(vertex[currentOffsetVertex + i1], vertex[nextCurrentOffsetVertex + i1], poll);
              Vector3 v3 = Vector3.Lerp(vertex[currentOffsetVertex + i2], vertex[nextCurrentOffsetVertex + i2], poll);

            vertices[index++] = v1;
            vertices[index++] = v2;
            vertices[index++] = v3;
        }

        frame.vertices = vertices;
        frame.RecalculateBounds();
        frame.RecalculateNormals();
    }
	
	// Update is called once per frame
	void Update () 
	{
        if (maxFrames <= 1) return;


        if (useKesy)
        {

            
				if (Input.GetKey (KeyCode.LeftArrow)) 
		       {
                   BackAnimation();
				} else		
			 if (Input.GetKey (KeyCode.RightArrow)) 
			{
                NextAnimation();
			} 
        }



		float time = tickcount ();
		float elapsedTime =  time - lastTime;
		lerptime = tickcount() / (1.0f / currAnimation.fps);


        if (Manual)
        {
            if (Frame <= 0) Frame = 0;
            if (Frame >= maxFrames - 1) Frame = maxFrames - 1;
            currentFrame = Frame;
            nextFrame = Frame + 1;
            animate(currentFrame, nextFrame, lerptime);
        }
        else
        {



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

                animate(currentFrame, nextFrame, lerptime);
                if (AnimationChange != null)
                {
                    AnimationChange(currAnimation);
                }
            }
            Frame = nextFrame;

        }





		
		



		


	


	}
}
