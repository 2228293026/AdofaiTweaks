using DG.Tweening;
using UnityEngine;

public class Mawaru_Arm : MonoBehaviour
{
	public Mawaru_Sprite hand;

	public Sprite ok;

	public Sprite nah;

	public int fnum;

	public scrFloor floor;

	public bool slapped;

	public GameObject handContainer;

	public bool hitgood;

	public void Hit()
	{
		hitgood = true;
		slapped = true;
		float speed = TaroBGScript.instance.speed;
		base.transform.DOMoveY(-10f * Mathf.Sign(base.transform.localScale.y), 5f).SetRelative(isRelative: true);
		hand.render.sprite = ok;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DOScale(Vector3.one * 2f, 0f))
			.Append(handContainer.transform.DOScale(Vector3.one, 0.3f * speed).SetEase(Ease.OutQuad));
	}

	public void HitOK()
	{
		hitgood = false;
		slapped = true;
		float speed = TaroBGScript.instance.speed;
		hand.render.sprite = nah;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DOScale(Vector3.one * 2f, 0f))
			.Append(handContainer.transform.DOScale(Vector3.one, 0.3f * speed).SetEase(Ease.OutQuad));
	}
}
