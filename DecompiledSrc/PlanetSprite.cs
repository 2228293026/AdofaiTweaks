using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlanetSprite : ADOBase
{
	private const int frameCount = 11;

	private const float fps = 12f;

	private const string AlphaMask = "_AlphaMask";

	private const string PlanetTex = "_PlanetTex";

	[NonSerialized]
	public SpriteRenderer meshRenderer;

	private bool setup;

	public Texture sprite
	{
		get
		{
			return meshRenderer.material.GetTexture("_PlanetTex");
		}
		set
		{
			meshRenderer.material.SetTexture("_PlanetTex", value);
		}
	}

	public Color color
	{
		get
		{
			return meshRenderer.color;
		}
		set
		{
			meshRenderer.color = value;
		}
	}

	public bool visible
	{
		get
		{
			return meshRenderer.enabled;
		}
		set
		{
			meshRenderer.enabled = value;
		}
	}

	public Texture alphaMask
	{
		get
		{
			return meshRenderer.material.GetTexture("_AlphaMask");
		}
		set
		{
			meshRenderer.material.SetTexture("_AlphaMask", value);
		}
	}

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (!setup)
		{
			setup = true;
			meshRenderer = GetComponent<SpriteRenderer>();
		}
	}

	public void LateUpdate()
	{
		meshRenderer.material.SetFloat("_Frame", Time.unscaledTime * 12f % 11f);
	}
}
