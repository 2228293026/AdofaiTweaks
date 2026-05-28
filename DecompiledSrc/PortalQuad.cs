using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PortalQuad : ADOBase
{
	public bool keepRenderQueue;

	private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();

	private Material material => meshRenderer.material;

	private void Awake()
	{
		if (!keepRenderQueue)
		{
			material.renderQueue = 2001;
		}
	}

	public void SetTexture(Texture2D texture)
	{
		if (texture == null)
		{
			Debug.LogError("texture is null");
			return;
		}
		material.mainTexture = texture;
		Vector3 localScale = base.transform.localScale;
		float value = (float)texture.width * 1f / (float)texture.height / (localScale.x / localScale.y);
		material.SetFloat("_Ratio", value);
	}

	public void RemoveTexture()
	{
		material.mainTexture = null;
	}

	public Texture GetTexture()
	{
		return material.mainTexture;
	}

	public TweenerCore<Color, Color, ColorOptions> Fade(float alpha, float duration)
	{
		return material.DOColor(Color.white.WithAlpha(alpha), "_Color", duration).SetUpdate(isIndependentUpdate: true);
	}
}
