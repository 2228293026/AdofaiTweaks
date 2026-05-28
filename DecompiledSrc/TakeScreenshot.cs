using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TakeScreenshot
{
	private const int bufferWidth = 352;

	private const int bufferHeight = 198;

	private const int iterations = 2;

	private const float blurSpread = 0.6f;

	private RenderTexture blurredTexture;

	private Material mat;

	private bool setup;

	public TakeScreenshot()
	{
		Setup();
	}

	private void Setup()
	{
		if (!setup)
		{
			mat = new Material(RDConstants.data.blurEffectConeTap);
			blurredTexture = new RenderTexture(352, 198, 24);
			setup = true;
		}
	}

	public IEnumerator GetBlurredScreenshot(RawImage rawImage)
	{
		Setup();
		yield return new WaitForEndOfFrame();
		_ = scrController.instance;
		RenderTexture temporary = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
		ScreenCapture.CaptureScreenshotIntoRenderTexture(temporary);
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		bool flag = graphicsDeviceType != GraphicsDeviceType.Vulkan && graphicsDeviceType != GraphicsDeviceType.OpenGLCore && graphicsDeviceType != GraphicsDeviceType.OpenGLES3;
		mat.mainTexture = temporary;
		mat.SetFloat("_FlipY", flag ? 1f : 0f);
		int width = blurredTexture.width / 4;
		int height = blurredTexture.height / 4;
		RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
		DownSample4x(temporary, temporary2);
		RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0);
		for (int i = 0; i < 2; i++)
		{
			FourTapCone(temporary2, temporary3, i);
			temporary2.DiscardContents();
			Graphics.Blit(temporary3, temporary2);
			temporary3.DiscardContents();
		}
		RenderTexture.ReleaseTemporary(temporary3);
		Graphics.Blit(temporary2, blurredTexture);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary);
		rawImage.texture = blurredTexture;
	}

	private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * 0.6f;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample4x(Texture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}
}
