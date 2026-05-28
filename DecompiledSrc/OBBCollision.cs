using System;
using System.Linq;
using UnityEngine;

public static class OBBCollision
{
	public static bool CheckTouched(Vector2 obj1Pos, Vector2 obj1Size, float obj1Rot, Vector2 obj2Pos, Vector2 obj2Size, float obj2Rot)
	{
		Vector2[] obj1Corners = GetOrientedCorners(obj1Pos, obj1Size, obj1Rot);
		Vector2[] obj2Corners = GetOrientedCorners(obj2Pos, obj2Size, obj2Rot);
		return GetAxes(obj1Corners, obj2Corners).All((Vector2 axis) => Overlap(Projection(obj1Corners, axis), Projection(obj2Corners, axis)));
	}

	private static Vector2[] GetOrientedCorners(Vector2 position, Vector2 size, float rotation)
	{
		Vector2[] array = new Vector2[4];
		float num = Mathf.Cos(rotation * ((float)Math.PI / 180f));
		float num2 = Mathf.Sin(rotation * ((float)Math.PI / 180f));
		array[0] = new Vector2((0f - size.x) / 2f, size.y / 2f);
		array[1] = new Vector2(size.x / 2f, size.y / 2f);
		array[2] = new Vector2(size.x / 2f, (0f - size.y) / 2f);
		array[3] = new Vector2((0f - size.x) / 2f, (0f - size.y) / 2f);
		for (int i = 0; i < 4; i++)
		{
			float num3 = array[i].x * num - array[i].y * num2;
			float num4 = array[i].x * num2 + array[i].y * num;
			array[i] = new Vector2(num3 + position.x, num4 + position.y);
		}
		return array;
	}

	private static Vector2[] GetAxes(Vector2[] corners1, Vector2[] corners2)
	{
		Vector2[] array = new Vector2[4];
		for (int i = 0; i < 2; i++)
		{
			array[i] = new Vector2(corners1[(i + 1) % 4].y - corners1[i].y, corners1[i].x - corners1[(i + 1) % 4].x).normalized;
			array[i + 2] = new Vector2(corners2[(i + 1) % 4].y - corners2[i].y, corners2[i].x - corners2[(i + 1) % 4].x).normalized;
		}
		return array;
	}

	private static (float min, float max) Projection(Vector2[] corners, Vector2 axis)
	{
		float num = Vector2.Dot(corners[0], axis);
		float num2 = num;
		for (int i = 1; i < corners.Length; i++)
		{
			float num3 = Vector2.Dot(corners[i], axis);
			if (num3 < num)
			{
				num = num3;
			}
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		return (min: num, max: num2);
	}

	private static bool Overlap((float min, float max) proj1, (float min, float max) proj2)
	{
		if (!(proj1.max < proj2.min))
		{
			return !(proj2.max < proj1.min);
		}
		return false;
	}
}
