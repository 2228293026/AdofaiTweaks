using System;
using DG.Tweening;
using UnityEngine;

public abstract class FloorRenderer : ADOBase
{
	public static int ShaderProperty_Flash;

	public Renderer renderer;

	public Color deselectedColor;

	[NonSerialized]
	public Material material;

	public Color cachedColor;

	public abstract Color color { get; set; }

	public abstract Sprite sprite { get; set; }

	public int sortingOrder
	{
		get
		{
			return renderer.sortingOrder;
		}
		set
		{
			renderer.sortingOrder = value;
		}
	}

	public abstract void SetAngle(float entryAngle, float exitAngle);

	public virtual void SetFlash(float flash)
	{
		renderer.material.SetFloat(ShaderProperty_Flash, flash);
	}

	public virtual void TweenFlash(float endValue, float duration, string tweenID)
	{
		material.DOFloat(endValue, ShaderProperty_Flash, duration).SetUpdate(isIndependentUpdate: true).SetId(tweenID);
	}

	public virtual void Awake()
	{
		renderer = GetComponent<Renderer>();
		material = renderer.material;
		_ = material == null;
		deselectedColor = color;
	}

	public void SetMaterial(Material material)
	{
		renderer.material = (this.material = material);
	}
}
