using System;
using DG.Tweening;
using UnityEngine;

namespace MobileMenu;

[RequireComponent(typeof(Collider2D))]
public abstract class MobileMenuGrabbable : MonoBehaviour
{
	[NonSerialized]
	public Vector2 originalPosition;

	public bool grabbable = true;

	private Tween returnTween;

	private void Start()
	{
		originalPosition = base.transform.localPosition;
	}

	public virtual bool Grab()
	{
		scrSfx.instance.PlaySfx(SfxSound.PlanetCatch, MixerGroup.InterfaceParent);
		if (returnTween != null)
		{
			returnTween.Kill();
		}
		return true;
	}

	public virtual void Move(Vector2 pos)
	{
		base.transform.position = pos;
	}

	public virtual void Ungrab()
	{
		scrSfx.instance.PlaySfx(SfxSound.PlanetRelease, MixerGroup.InterfaceParent);
		returnTween = base.transform.DOLocalMove(originalPosition, 0.5f).SetEase(Ease.OutSine);
	}
}
