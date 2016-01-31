using System;
using UnityEngine;

public enum MoveType
{
	Cross,
	Shift,
	DoSiDo,
	None
}

public static class MoveTypeMath
{
	public static Vector2 EvaluateLine(Vector2 a, Vector2 b, float t)
	{
		return Vector2.Lerp(a, b, t);
	}
	public static Vector2 EvaluateCircle(Vector2 pointA, Vector2 pointB, Vector2 center, float t, bool invert = false)
	{
		center = invert ? (((pointB - pointA) / 2 + pointA) - center) * 2 + center : center;

		var a = (pointA - center);
		var b = (pointB - center);
		var radius = a.magnitude;
		var angleA = Vector2Extensions.FullAngle(a, Vector2.right);
		var angleB = Vector2Extensions.FullAngle(b, Vector2.right);
		var currentAngle = Mathf.LerpAngle(angleA, angleB, t);
		return new Vector2(Mathf.Cos(currentAngle * Mathf.PI / 180.0f) * radius, Mathf.Sin(currentAngle * Mathf.PI / 180.0f) * radius) + center;
	}
}