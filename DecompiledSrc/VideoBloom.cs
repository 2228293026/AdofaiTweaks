using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Other/VideoBloom")]
[RequireComponent(typeof(Camera))]
public class VideoBloom : PostEffectsBase
{
	public enum TweakMode
	{
		Basic,
		Advanced
	}

	public enum BlendingMode
	{
		Add,
		Screen
	}

	public TweakMode tweakMode;

	[Range(0f, 4f)]
	public float Threshold = 0.75f;

	[Range(0f, 5f)]
	public float MasterAmount = 0.5f;

	[Range(0f, 5f)]
	public float MediumAmount = 1f;

	[Range(0f, 100f)]
	public float LargeAmount;

	public Color Tint = new Color(1f, 1f, 1f, 1f);

	[Range(10f, 100f)]
	public float KernelSize = 50f;

	[Range(1f, 20f)]
	public float MediumKernelScale = 1f;

	[Range(3f, 20f)]
	public float LargeKernelScale = 3f;

	public BlendingMode BlendMode;

	public bool HighQuality = true;

	public Shader videoBloomShader;

	private Material videoBloomMaterial;

	private static readonly int Param0 = Shader.PropertyToID("_Param0");

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Param2 = Shader.PropertyToID("_Param2");

	private static readonly int MediumBloom = Shader.PropertyToID("_MediumBloom");

	private static readonly int LargeBloom = Shader.PropertyToID("_LargeBloom");

	private static readonly int Param1 = Shader.PropertyToID("_Param1");

	public float masterAmount
	{
		get
		{
			return MasterAmount;
		}
		set
		{
			MasterAmount = value;
		}
	}

	public override bool CheckResources()
	{
		videoBloomMaterial = CheckShaderAndCreateMaterial(videoBloomShader, videoBloomMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private float videoBlurGetMaxScaleFor(float radius)
	{
		double num = radius;
		double num2 = ((num < 10.0) ? (0.1 * num * 1.468417) : ((num < 36.3287) ? (0.127368 * num + 0.194737) : (0.8 * (double)(float)Math.Sqrt(num))));
		if (!(num2 <= 0.0))
		{
			return (float)num2;
		}
		return 0f;
	}

	private void Awake()
	{
		if (!CheckResources())
		{
			base.enabled = false;
		}
	}

	private void BloomBlit(RenderTexture source, RenderTexture blur1, RenderTexture blur2, float radius1, float radius2)
	{
		float num = videoBlurGetMaxScaleFor(radius1);
		int num2 = (int)num;
		float value = num - (float)num2;
		num = videoBlurGetMaxScaleFor(radius2);
		int num3 = (int)num;
		float num4 = 1f;
		int width = source.width;
		int height = source.height;
		float num5 = ((num2 != 0) ? 1f : (-1f));
		if (radius1 == 0f)
		{
			Graphics.Blit(source, blur1);
			return;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		Graphics.Blit(source, renderTexture);
		RenderTexture renderTexture2 = null;
		int i;
		Vector4 value2;
		for (i = 0; i < num2; i++)
		{
			num5 = ((i % 2 != 0) ? (-1f) : 1f);
			value2 = new Vector4(num5 * num4 * 1.3333334f, num4 * (1f / 3f), num5 * num4 * (1f / 3f), (0f - num4) * 1.3333334f);
			videoBloomMaterial.SetVector(Param0, value2);
			renderTexture2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
			renderTexture2.filterMode = FilterMode.Bilinear;
			renderTexture2.wrapMode = TextureWrapMode.Clamp;
			videoBloomMaterial.SetTexture(MainTex, renderTexture);
			Graphics.Blit(renderTexture, renderTexture2, videoBloomMaterial, 0);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = renderTexture2;
			num4 *= 1.4142135f;
		}
		RenderTexture.ReleaseTemporary(renderTexture2);
		value2 = new Vector4((0f - num5) * num4 * 1.3333334f, num4 * (1f / 3f), (0f - num5) * num4 * (1f / 3f), (0f - num4) * 1.3333334f);
		videoBloomMaterial.SetVector(Param0, value2);
		videoBloomMaterial.SetFloat(Param2, value);
		videoBloomMaterial.SetTexture(MainTex, renderTexture);
		Graphics.Blit(renderTexture, blur1, videoBloomMaterial, 1);
		if (blur2 != null)
		{
			RenderTexture renderTexture3 = null;
			for (; i < num3; i++)
			{
				num5 = ((i % 2 != 0) ? (-1f) : 1f);
				value2 = new Vector4(num5 * num4 * 1.3333334f, num4 * (1f / 3f), num5 * num4 * (1f / 3f), (0f - num4) * 1.3333334f);
				videoBloomMaterial.SetVector(Param0, value2);
				renderTexture3 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
				renderTexture3.filterMode = FilterMode.Bilinear;
				renderTexture3.wrapMode = TextureWrapMode.Clamp;
				videoBloomMaterial.SetTexture(MainTex, renderTexture);
				Graphics.Blit(renderTexture, renderTexture3, videoBloomMaterial, 0);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = renderTexture3;
				num4 *= 1.4142135f;
			}
			RenderTexture.ReleaseTemporary(renderTexture3);
			value2 = new Vector4((0f - num5) * num4 * 1.3333334f, num4 * (1f / 3f), (0f - num5) * num4 * (1f / 3f), (0f - num4) * 1.3333334f);
			videoBloomMaterial.SetVector(Param0, value2);
			videoBloomMaterial.SetFloat(Param2, num - (float)num3);
			videoBloomMaterial.SetTexture(MainTex, renderTexture);
			Graphics.Blit(renderTexture, blur2, videoBloomMaterial, 1);
		}
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (source == null)
		{
			MonoBehaviour.print("source is null");
			return;
		}
		if ((MediumAmount == 0f && LargeAmount == 0f) || MasterAmount == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		float num = 198f / (float)source.height;
		int num2 = (int)(num * (float)source.width);
		int num3 = (int)(num * (float)source.height);
		Vector4 value = new Vector4(0.5f * MasterAmount * MediumAmount, 0.5f * MasterAmount * LargeAmount, 0f, 0f);
		Vector4 value2 = new Vector4(Tint.r, Tint.g, Tint.b, 1f);
		float num4 = KernelSize * MediumKernelScale * num;
		float num5 = KernelSize * LargeKernelScale * num;
		RenderTexture temporary = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture temporary2 = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture renderTexture = null;
		temporary.filterMode = FilterMode.Bilinear;
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary2.filterMode = FilterMode.Bilinear;
		temporary2.wrapMode = TextureWrapMode.Clamp;
		if (num > 0.99f)
		{
			videoBloomMaterial.SetFloat(Param2, Threshold);
			Graphics.Blit(source, temporary, videoBloomMaterial, 2);
		}
		else
		{
			renderTexture = RenderTexture.GetTemporary(2 * num2, 2 * num3, 0, RenderTextureFormat.ARGBHalf);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, renderTexture);
			videoBloomMaterial.SetFloat(Param2, Threshold);
			Graphics.Blit(renderTexture, temporary, videoBloomMaterial, 2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = null;
		}
		if (LargeAmount != 0f)
		{
			renderTexture = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.ARGBHalf);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
		}
		BloomBlit(temporary, temporary2, renderTexture, num4, (num5 >= num4) ? num5 : num4);
		videoBloomMaterial.SetTexture(MainTex, source);
		videoBloomMaterial.SetTexture(MediumBloom, temporary2);
		videoBloomMaterial.SetTexture(LargeBloom, renderTexture);
		videoBloomMaterial.SetVector(Param0, value);
		videoBloomMaterial.SetVector(Param1, value2);
		Graphics.Blit(source, destination, videoBloomMaterial, (BlendMode == BlendingMode.Screen) ? 4 : 3);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
