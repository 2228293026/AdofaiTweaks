using System;
using UnityEngine;
using UnityEngine.UI;

public class scrBlur : ADOBase
{
	public Texture2D tileTexture;

	[Header("Variables")]
	public float tinting = 0.4f;

	public float blurSize = 4f;

	public int passes = 8;

	public Color baseTint;

	public Color blurTint;

	public Texture texture;

	public Material blurMaterial;

	public RenderTexture destTexture;

	private bool init;

	private RawImage rawImage;

	private void Init()
	{
		rawImage = GetComponent<RawImage>();
		int width = rawImage.texture.width;
		int height = rawImage.texture.height;
		blurMaterial = new Material(ADOBase.gc.tileBlurShader);
		destTexture = new RenderTexture(width, height, 0);
		destTexture.Create();
		init = true;
	}

	public void UpdateTexture()
	{
		if (!init)
		{
			Init();
		}
		texture = rawImage.texture;
		blurMaterial.SetTexture("_BaseTex", texture);
		blurMaterial.SetTexture("_TileTex", tileTexture);
		blurMaterial.SetColor("_BaseTint", baseTint);
		blurMaterial.SetColor("_BlurTint", blurTint);
		blurMaterial.SetFloat("_Tinting", tinting);
		blurMaterial.SetFloat("_BlurSize", blurSize);
		rawImage.texture = BlurTexture(texture);
	}

	public Texture BlurTexture(Texture sourceTexture)
	{
		RenderTexture active = RenderTexture.active;
		try
		{
			RenderTexture temporary = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			RenderTexture temporary2 = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			for (int i = 0; i < passes; i++)
			{
				blurMaterial.SetFloat("_Pass", i);
				Graphics.Blit((i == 0) ? sourceTexture : temporary2, temporary, blurMaterial, 0);
				Graphics.Blit(temporary, temporary2, blurMaterial, 1);
			}
			Graphics.Blit(temporary2, destTexture, blurMaterial, 2);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		finally
		{
			RenderTexture.active = active;
		}
		return destTexture;
	}
}
