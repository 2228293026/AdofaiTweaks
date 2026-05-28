using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ExtensionMethods
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector4 WithX(this Vector4 v, float x)
	{
		return new Vector4(x, v.y, v.z, v.w);
	}

	public static Vector4 WithY(this Vector4 v, float y)
	{
		return new Vector4(v.x, y, v.z, v.w);
	}

	public static Vector4 WithZ(this Vector4 v, float z)
	{
		return new Vector4(v.x, v.y, z, v.w);
	}

	public static Vector4 WithW(this Vector4 v, float w)
	{
		return new Vector4(v.x, v.y, v.z, w);
	}

	public static Vector3 WithX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector2 WithX(this Vector2 v, float x)
	{
		return new Vector2(x, v.y);
	}

	public static Vector2 WithY(this Vector2 v, float y)
	{
		return new Vector2(v.x, y);
	}

	public static Vector3 WithZ(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			axisDirection.Normalize();
		}
		float num = Vector3.Dot(point, axisDirection);
		return axisDirection * num;
	}

	public static Vector3 NearestPointOnLine(this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			lineDirection.Normalize();
		}
		float num = Vector3.Dot(point - pointOnLine, lineDirection);
		return pointOnLine + lineDirection * num;
	}

	public static Vector2 Abs(this Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
	}

	public static Vector2 Sign(this Vector2 vector)
	{
		return new Vector2(Mathf.Sign(vector.x), Mathf.Sign(vector.y));
	}

	public static Vector3 Sign(this Vector3 vector)
	{
		return new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
	}

	public static float GetRatio(this Vector2 size)
	{
		return size.x / size.y;
	}

	public static float FindAngleAtan2(this Vector2 vector)
	{
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	public static Vector2 GetRotatedVector(this Vector2 originalPos, float radians)
	{
		float num = Mathf.Sin(radians);
		float num2 = Mathf.Cos(radians);
		return new Vector2(originalPos.x * num2 - originalPos.y * num, originalPos.x * num + originalPos.y * num2);
	}

	public static Vector2 RotateAroundPoint(Vector2 point, Vector2 pivot, float radians)
	{
		return (point - pivot).GetRotatedVector(radians) + pivot;
	}

	public static string Truncate(this string value, int maxLength)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (value.Length > maxLength)
		{
			return value.Substring(0, maxLength);
		}
		return value;
	}

	public static string ToString(this object anObject, string aFormat)
	{
		return anObject.ToString(aFormat, null);
	}

	public static string ToString(this object anObject, string aFormat, IFormatProvider formatProvider)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Type type = anObject.GetType();
		MatchCollection matchCollection = new Regex("({)([^}]+)(})", RegexOptions.IgnoreCase).Matches(aFormat);
		int num = 0;
		foreach (Match item in matchCollection)
		{
			Group obj = item.Groups[2];
			int length = obj.Index - num - 1;
			stringBuilder.Append(aFormat.Substring(num, length));
			string empty = string.Empty;
			string text = string.Empty;
			int num2 = obj.Value.IndexOf(":");
			if (num2 == -1)
			{
				empty = obj.Value;
			}
			else
			{
				empty = obj.Value.Substring(0, num2);
				text = obj.Value.Substring(num2 + 1);
			}
			PropertyInfo property = type.GetProperty(empty);
			Type type2 = null;
			object target = null;
			if (property != null)
			{
				type2 = property.PropertyType;
				target = property.GetValue(anObject, null);
			}
			else
			{
				FieldInfo field = type.GetField(empty);
				if (field != null)
				{
					type2 = field.FieldType;
					target = field.GetValue(anObject);
				}
			}
			if (type2 != null)
			{
				string empty2 = string.Empty;
				empty2 = ((!(text == string.Empty)) ? (type2.InvokeMember("ToString", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, target, new object[2] { text, formatProvider }) as string) : (type2.InvokeMember("ToString", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, target, null) as string));
				stringBuilder.Append(empty2);
			}
			else
			{
				stringBuilder.Append("{");
				stringBuilder.Append(obj.Value);
				stringBuilder.Append("}");
			}
			num = obj.Index + obj.Length + 1;
		}
		if (num < aFormat.Length)
		{
			stringBuilder.Append(aFormat.Substring(num));
		}
		return stringBuilder.ToString();
	}

	public static void CopyToClipboard(this string str)
	{
		GUIUtility.systemCopyBuffer = str;
	}

	public static int CountBits(this int value)
	{
		int num = 0;
		while (value != 0)
		{
			num++;
			value &= value - 1;
		}
		return num;
	}

	public static int CountBits(this Enum value)
	{
		return ((int)(object)value).CountBits();
	}

	public static void BubbleSort<T>(this List<T> list, List<int> indices)
	{
		int count = list.Count;
		for (int i = 0; i < count - 1; i++)
		{
			for (int j = 0; j < count - 1 - i; j++)
			{
				if (indices[j] > indices[j + 1])
				{
					T value = list[j + 1];
					list[j + 1] = list[j];
					list[j] = value;
					int value2 = indices[j + 1];
					indices[j + 1] = indices[j];
					indices[j] = value2;
				}
			}
		}
	}

	public static void InverseBubbleSort<T>(this List<T> list, List<int> indices)
	{
		int count = list.Count;
		for (int i = 0; i < count - 1; i++)
		{
			for (int j = 0; j < count - 1 - i; j++)
			{
				if (indices[j] < indices[j + 1])
				{
					T value = list[j + 1];
					list[j + 1] = list[j];
					list[j] = value;
					int value2 = indices[j + 1];
					indices[j + 1] = indices[j];
					indices[j] = value2;
				}
			}
		}
	}
}
