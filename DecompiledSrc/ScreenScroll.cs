using System;
using UnityEngine;

public class ScreenScroll : MonoBehaviour
{
	public Shader SCShader;

	private Material SCMaterial;

	public Vector2 scrollSpeed = Vector2.zero;

	[NonSerialized]
	public Vector2 scrollOffset = Vector2.zero;

	private Material material
	{
		get
		{
			if (SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return SCMaterial;
		}
	}

	private void Start()
	{
		SCShader = Shader.Find("ADOFAI/ScreenScroll");
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			material.SetFloat("_OffsetX", scrollOffset.x);
			material.SetFloat("_OffsetY", scrollOffset.y);
			material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void Update()
	{
		scrollOffset += scrollSpeed * Time.deltaTime;
	}

	private void OnDisable()
	{
		if ((bool)SCMaterial)
		{
			UnityEngine.Object.DestroyImmediate(SCMaterial);
		}
	}

	private void OnEnable()
	{
		scrollOffset = Vector2.zero;
	}
}
