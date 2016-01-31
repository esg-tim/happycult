using System;
using UnityEngine;

public static class Vector2Extensions
{
	public static float FullAngle(Vector2 a, Vector2 b)
	{
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(Vector3.back,Vector3.Cross(a,b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		float angle360 =  signed_angle < 0 ? signed_angle + 360 : signed_angle;

		return angle360;
	}

	public static Vector2 Rotate(this Vector2 v, float degrees) {
		float radians = degrees * Mathf.Deg2Rad;
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);

		float tx = v.x;
		float ty = v.y;

		return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
	}
}

