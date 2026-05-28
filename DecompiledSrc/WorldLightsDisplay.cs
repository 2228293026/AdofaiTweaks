using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class WorldLightsDisplay : ADOBase
{
	public Transform[] lights;

	public Dictionary<SpriteRenderer, float> spriteAlphas;

	public abstract void UpdateStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true);

	protected virtual void Awake()
	{
		spriteAlphas = new Dictionary<SpriteRenderer, float>();
		SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteAlphas.Add(spriteRenderer, spriteRenderer.color.a);
		}
	}

	public virtual void Fade(float alpha, float duration)
	{
		if (spriteAlphas == null)
		{
			return;
		}
		foreach (KeyValuePair<SpriteRenderer, float> spriteAlpha in spriteAlphas)
		{
			SpriteRenderer key = spriteAlpha.Key;
			float value = spriteAlpha.Value;
			key.DOKill();
			key.DOFade(value * alpha, duration);
		}
	}
}
