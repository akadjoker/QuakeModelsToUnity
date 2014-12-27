using UnityEngine;
using System.Collections;

public struct Vec3f  
{

	public float x;
	public float y;
	public float z;

	public Vec3f(float value)
	{
		x = value;
		y = value;
		z = value;
	}
	public Vec3f(float px,float py,float pz)
	{
		x = px;
		y = py;
		z = pz;
	}
	public static Vec3f Add(Vec3f value1, Vec3f value2)
	{
		value1.x += value2.x;
		value1.y += value2.y;
		value1.z += value2.z;
		return value1;
	}
	public static Vec3f Divide(Vec3f value1, Vec3f value2)
	{
		value1.x /= value2.x;
		value1.y /= value2.y;
		value1.z /= value2.z;
		return value1;
	}
	
	public static Vec3f Divide(Vec3f value1, float value2)
	{
		float factor = 1/value2;
		value1.x *= factor;
		value1.y *= factor;
		value1.z *= factor;
		return value1;
	}
	public static Vec3f Multiply(Vec3f value1, Vec3f value2)
	{
		value1.x *= value2.x;
		value1.y *= value2.y;
		value1.z *= value2.z;
		return value1;
	}
	public static Vec3f Subtract(Vec3f value1, Vec3f value2)
	{
		value1.x -= value2.x;
		value1.y -= value2.y;
		value1.z -= value2.z;
		return value1;
	}
	public static Vec3f Multiply(Vec3f value1, float scaleFactor)
	{
		value1.x *= scaleFactor;
		value1.y *= scaleFactor;
		value1.z *= scaleFactor;
		return value1;
	}
	public static float Dot(Vec3f vector1, Vec3f vector2)
	{
		return vector1.x*vector2.x + vector1.y*vector2.y + vector1.z*vector2.z;
	}
	public static Vec3f Negate(Vec3f value)
	{
		value = new Vec3f(-value.x, -value.y, -value.z);
		return value;
	}

}
