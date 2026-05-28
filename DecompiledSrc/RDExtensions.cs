using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class RDExtensions
{
	public static void SetAnchorPosX(this RectTransform t, float x)
	{
		t.anchoredPosition = new Vector2(x, t.anchoredPosition.y);
	}

	public static void SetAnchorPosY(this RectTransform t, float y)
	{
		t.anchoredPosition = new Vector2(t.anchoredPosition.x, y);
	}

	public static string RemoveRichTags(this string str)
	{
		return RDUtils.RemoveRichTags(str);
	}

	public static void SetAlpha(this SpriteRenderer sr, float alpha)
	{
		sr.color = sr.color.WithAlpha(alpha);
	}

	public static void MoveX(this Transform t, float x)
	{
		t.position = new Vector3(x, t.position.y, t.position.z);
	}

	public static void MoveY(this Transform t, float y)
	{
		t.position = new Vector3(t.position.x, y, t.position.z);
	}

	public static void MoveZ(this Transform t, float z)
	{
		t.position = new Vector3(t.position.x, t.position.y, z);
	}

	public static void MoveXY(this Transform t, float x, float y)
	{
		t.position = new Vector3(x, y, t.position.z);
	}

	public static void MoveXY(this Transform t, Vector2 v)
	{
		t.position = new Vector3(v.x, v.y, t.position.z);
	}

	public static void MoveXY(this Transform t, Vector3 v)
	{
		t.position = new Vector3(v.x, v.y, t.position.z);
	}

	public static void LocalMoveX(this Transform t, float x)
	{
		t.localPosition = new Vector3(x, t.localPosition.y, t.localPosition.z);
	}

	public static void LocalMoveY(this Transform t, float y)
	{
		t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.z);
	}

	public static void LocalMoveZ(this Transform t, float z)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
	}

	public static void LocalMoveXY(this Transform t, float x, float y)
	{
		t.localPosition = new Vector3(x, y, t.localPosition.z);
	}

	public static void LocalMoveXY(this Transform t, Vector2 vector)
	{
		t.localPosition = new Vector3(vector.x, vector.y, t.localPosition.z);
	}

	public static void TranslateX(this Transform t, float x)
	{
		t.localPosition = new Vector3(t.localPosition.x + x, t.localPosition.y, t.localPosition.z);
	}

	public static void TranslateY(this Transform t, float y)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y + y, t.localPosition.z);
	}

	public static void TranslateZ(this Transform t, float z)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, t.localPosition.z + z);
	}

	public static void ScaleX(this Transform t, float x)
	{
		t.localScale = new Vector3(x, t.localScale.y, t.localScale.z);
	}

	public static void ScaleY(this Transform t, float y)
	{
		t.localScale = new Vector3(t.localScale.x, y, t.localScale.z);
	}

	public static void ScaleXY(this Transform t, float x, float y)
	{
		t.localScale = new Vector3(x, y, t.localScale.z);
	}

	public static void ScaleXY(this Transform t, float xy)
	{
		t.localScale = new Vector3(xy, xy, t.localScale.z);
	}

	public static void ScaleZ(this Transform t, float z)
	{
		t.localScale = new Vector3(t.localScale.x, t.localScale.y, z);
	}

	public static Color ARGBToColor(this uint color)
	{
		byte b = (byte)(color >> 24);
		byte num = (byte)(color >> 16);
		byte b2 = (byte)(color >> 8);
		byte b3 = (byte)color;
		return new Color((float)(int)num / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f, (float)(int)b / 255f);
	}

	public static Color RGBAToColor(this uint color)
	{
		byte num = (byte)(color >> 24);
		byte b = (byte)(color >> 16);
		byte b2 = (byte)(color >> 8);
		byte b3 = (byte)color;
		return new Color((float)(int)num / 255f, (float)(int)b / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f);
	}

	public static void Shake(this Behaviour camera, int jitterCount, int jitterAmount)
	{
		camera.gameObject.GetOrAddComponent<scrThingShake>().activateThingJitter(jitterCount, jitterAmount);
	}

	public static void StopShake(this Behaviour camera)
	{
		camera.gameObject.GetOrAddComponent<scrThingShake>().StopShake();
	}

	public static bool IsActuallyPlayingNotJustScheduled(this AudioSource sound)
	{
		if (sound.time > 0f)
		{
			return sound.isPlaying;
		}
		return false;
	}

	public static int IncrementWrap(this int index, int count)
	{
		return (index + 1) % count;
	}

	public static int DecrementWrap(this int index, int count)
	{
		if (index != 0)
		{
			return index - 1;
		}
		return count - 1;
	}

	public static void Clear(this RenderTexture rt)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		RenderTexture.active = active;
	}

	public static void SaveAsPNG(this RenderTexture rt, string path)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
		texture2D.Apply();
		byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
		File.WriteAllBytes(path, bytes);
		RenderTexture.active = active;
	}

	public static RenderTexture GetEmptyRT(this Texture texture, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32)
	{
		return new RenderTexture(texture.width, texture.height, depth, format);
	}

	public static RenderTexture GetTempRT(this Texture texture, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32)
	{
		return RenderTexture.GetTemporary(texture.width, texture.height, depth, format);
	}

	public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
	{
		foreach (T item in collection)
		{
			action(item);
		}
	}
}
