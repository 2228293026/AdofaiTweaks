using DG.Tweening;
using UnityEngine;

public class Mawaru_Coin : Mawaru_Sprite
{
	public bool collected;

	public Vector3 pos => base.transform.position + Vector3.up * -0.2f;

	public void Collect()
	{
		collected = true;
		frameDelay = 0.03f;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(base.transform.DOMoveY(0.8f, 0.2f * base.speed).SetRelative(isRelative: true))
			.Append(base.transform.DOMoveY(-0.4f, 0.2f * base.speed).SetRelative(isRelative: true));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.2f * base.speed)
			.Append(render.material.DOColor(TaroBGScript.instance.whiteClear, 0.2f * base.speed));
	}
}
