using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MathHelper 
{
	public static Quaternion NormalizeQuaternion ( Quaternion quaternion)
	{
		Quaternion quaternion2;
		float num2 = (((quaternion.x * quaternion.x) + (quaternion.y * quaternion.y)) + (quaternion.z* quaternion.z)) + (quaternion.w * quaternion.w);
		float num = 1f / ((float) Math.Sqrt((double) num2));
		quaternion2.x = quaternion.x * num;
		quaternion2.y = quaternion.y * num;
		quaternion2.z = quaternion.z * num;
		quaternion2.w = quaternion.w * num;
		return quaternion2;
	}
	public static Quaternion CreateFromRotationMatrix(Matrix4x4 matrix)
	{
		float num8 = (matrix.m11 + matrix.m22) + matrix.m33;
		Quaternion quaternion = new Quaternion();
		if (num8 > 0f)
		{
			float num = (float) Math.Sqrt((double) (num8 + 1f));
			quaternion.w = num * 0.5f;
			num = 0.5f / num;
			quaternion.x = (matrix.m23 - matrix.m32) * num;
			quaternion.y = (matrix.m31 - matrix.m13) * num;
			quaternion.z = (matrix.m12 - matrix.m21) * num;
			return quaternion;
		}
		if ((matrix.m11 >= matrix.m22) && (matrix.m11 >= matrix.m33))
		{
			float num7 = (float) Math.Sqrt((double) (((1f + matrix.m11) - matrix.m22) - matrix.m33));
			float num4 = 0.5f / num7;
			quaternion.x = 0.5f * num7;
			quaternion.y = (matrix.m12 + matrix.m21) * num4;
			quaternion.z = (matrix.m13 + matrix.m31) * num4;
			quaternion.w = (matrix.m23 - matrix.m32) * num4;
			return quaternion;
		}
		if (matrix.m22 > matrix.m33)
		{
			float num6 = (float) Math.Sqrt((double) (((1f + matrix.m22) - matrix.m11) - matrix.m33));
			float num3 = 0.5f / num6;
			quaternion.x = (matrix.m21 + matrix.m12) * num3;
			quaternion.y = 0.5f * num6;
			quaternion.z = (matrix.m32 + matrix.m23) * num3;
			quaternion.w = (matrix.m31 - matrix.m13) * num3;
			return quaternion;
		}
		float num5 = (float) Math.Sqrt((double) (((1f + matrix.m33) - matrix.m11) - matrix.m22));
		float num2 = 0.5f / num5;
		quaternion.x = (matrix.m31 + matrix.m13) * num2;
		quaternion.y = (matrix.m32 + matrix.m23) * num2;
		quaternion.z = 0.5f * num5;
		quaternion.w = (matrix.m12 - matrix.m21) * num2;
		
		return quaternion;
		
	}

	public static float QuaternionLength(Quaternion q)
	{
		float num = (((q.x * q.x) + (q.y * q.y)) + (q.z * q.z)) + (q.w * q.w);
		return (float) Math.Sqrt((double) num);
	}
	
	
	public static float QuaternionLengthSquared(Quaternion q)
	{
		return ((((q.x * q.x) + (q.y * q.y)) + (q.z * q.z)) + (q.w * q.w));
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
	{
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2;
		q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2;
		q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2;
		q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2;
		q.x = Mathf.Sign(  ( m[2,1] - m[1,2] ) );
		q.y = Mathf.Sign(  ( m[0,2] - m[2,0] ) );
		q.z = Mathf.Sign(  ( m[1,0] - m[0,1] ) );
		return q;
	}
	 
 /*
    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
        var w = 4 * qw;
        var qx = (matrix.m21 - matrix.m12) / w;
        var qy = (matrix.m02 - matrix.m20) / w;
        var qz = (matrix.m10 - matrix.m01) / w;
 
        return new Quaternion(qx, qy, qz, qw);
    }
 
    public static Vector3 GetPosition(this Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;
 
        return new Vector3(x, y, z);
    }
	public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
 
        return new Vector3(x, y, z);
    }

	*/
	
	/// <summary>
	/// Extract translation from transform matrix.
	/// </summary>
	/// <param name="matrix">Transform matrix. This parameter is passed by reference
	/// to improve performance; no changes will be made to it.</param>
	/// <returns>
	/// Translation offset.
	/// </returns>
	public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
		Vector3 translate;
		translate.x = matrix.m03;
		translate.y = matrix.m13;
		translate.z = matrix.m23;
		return translate;
	}
	
	/// <summary>
	/// Extract rotation quaternion from transform matrix.
	/// </summary>
	/// <param name="matrix">Transform matrix. This parameter is passed by reference
	/// to improve performance; no changes will be made to it.</param>
	/// <returns>
	/// Quaternion representation of rotation transform.
	/// </returns>
	public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) {
		Vector3 forward;
		forward.x = matrix.m02;
		forward.y = matrix.m12;
		forward.z = matrix.m22;
		
		Vector3 upwards;
		upwards.x = matrix.m01;
		upwards.y = matrix.m11;
		upwards.z = matrix.m21;
		
		return Quaternion.LookRotation(forward, upwards);
	}
	
	/// <summary>
	/// Extract scale from transform matrix.
	/// </summary>
	/// <param name="matrix">Transform matrix. This parameter is passed by reference
	/// to improve performance; no changes will be made to it.</param>
	/// <returns>
	/// Scale vector.
	/// </returns>
	public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
		Vector3 scale;
		scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
		scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
		scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
		return scale;
	}
	
	/// <summary>
	/// Extract position, rotation and scale from TRS matrix.
	/// </summary>
	/// <param name="matrix">Transform matrix. This parameter is passed by reference
	/// to improve performance; no changes will be made to it.</param>
	/// <param name="localPosition">Output position.</param>
	/// <param name="localRotation">Output rotation.</param>
	/// <param name="localScale">Output scale.</param>
	public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
		localPosition = ExtractTranslationFromMatrix(ref matrix);
		localRotation = ExtractRotationFromMatrix(ref matrix);
		localScale = ExtractScaleFromMatrix(ref matrix);
	}
	
	/// <summary>
	/// Set transform component from TRS matrix.
	/// </summary>
	/// <param name="transform">Transform component.</param>
	/// <param name="matrix">Transform matrix. This parameter is passed by reference
	/// to improve performance; no changes will be made to it.</param>
	public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
		transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
		transform.localRotation = ExtractRotationFromMatrix(ref matrix);
		transform.localScale = ExtractScaleFromMatrix(ref matrix);
	}
	
	
	// EXTRAS!
	
	/// <summary>
	/// Identity quaternion.
	/// </summary>
	/// <remarks>
	/// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
	/// </remarks>
	public static readonly Quaternion IdentityQuaternion = Quaternion.identity;
	/// <summary>
	/// Identity matrix.
	/// </summary>
	/// <remarks>
	/// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
	/// </remarks>
	public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;
	
	/// <summary>
	/// Get translation matrix.
	/// </summary>
	/// <param name="offset">Translation offset.</param>
	/// <returns>
	/// The translation transform matrix.
	/// </returns>
	public static Matrix4x4 TranslationMatrix(Vector3 offset) {
		Matrix4x4 matrix = IdentityMatrix;
		matrix.m03 = offset.x;
		matrix.m13 = offset.y;
		matrix.m23 = offset.z;
		return matrix;
	}

	 public static float  CurveValue(float newvalue,float  oldvalue,float increments)
	{
		if (increments>1.0)  oldvalue=oldvalue-(oldvalue-newvalue)/increments;
		if (increments <= 1.0) oldvalue = newvalue;
		
		return oldvalue;
	}
	public static float clamp(float value,float  min,float  max)
	{
		if (max > min)
		{
			if (value < min) return min;
			else if (value > max) return max;
			else return value;
		}
		else
		{
			// Min/max swapped
			if (value < max) return max;
			else if (value > min) return min;
			else return value;
		}
	}
	
}
