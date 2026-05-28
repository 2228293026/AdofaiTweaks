using DG.Tweening;
using UnityEngine;

public class Torch : scrPrefabDecoration
{
	[Header("Torch")]
	public ParticleSystem flame;

	public Transform glowMask;

	public override void HitFloor()
	{
		flame.Play();
		glowMask.DOScale(glowMask.localScale * 5f, 0.3f).SetEase(Ease.OutQuad);
	}
}
