using System;
using DG.Tweening;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuGrabbableMine : MobileMenuGrabbable
{
	public SpriteRenderer explosion;

	public scrSpike spike;

	[NonSerialized]
	public bool exploded;

	private bool Explode()
	{
		if (exploded)
		{
			return false;
		}
		exploded = true;
		explosion.enabled = true;
		explosion.DOFade(0f, 0.8f).SetEase(Ease.Linear);
		explosion.transform.localScale = Vector3.one * 2.4f;
		explosion.transform.DOScale(1.5f, 0.8f).SetEase(Ease.OutCubic);
		spike.shadow.gameObject.SetActive(value: false);
		spike.ball.GetComponent<SpriteRenderer>().enabled = false;
		scrSfx.instance.PlaySfx(SfxSound.FireTile, MixerGroup.SfxParent);
		((Behaviour)(object)GetComponent<Collider2D>()).enabled = false;
		return true;
	}

	private void Update()
	{
		explosion.transform.eulerAngles = Vector3.forward * Mathf.RoundToInt(Time.timeSinceLevelLoad / 0.1f) * 422.7f;
	}

	public override bool Grab()
	{
		return Explode();
	}

	public override void Move(Vector2 pos)
	{
	}

	public override void Ungrab()
	{
	}
}
