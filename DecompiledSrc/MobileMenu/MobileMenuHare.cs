using DG.Tweening;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuHare : MonoBehaviour
{
	public Vector2 popoutOffset;

	public bool rightSide;

	public Transform spriteTransform;

	private Vector2 origSpritePos;

	private void Awake()
	{
		origSpritePos = spriteTransform.localPosition;
		Reposition();
	}

	private void Update()
	{
		Reposition();
	}

	private void Reposition()
	{
		Camera camobj = scrController.instance.camy.camobj;
		float num = camobj.orthographicSize * camobj.aspect;
		base.transform.localPosition = base.transform.localPosition.WithX(num * (float)(rightSide ? 1 : (-1)));
	}

	public void Show(bool show)
	{
		if (show)
		{
			base.gameObject.SetActive(value: true);
		}
		spriteTransform.DOKill();
		Vector2 vector = origSpritePos + popoutOffset;
		Tween t = spriteTransform.DOLocalMove(show ? vector : origSpritePos, 0.5f).SetEase(Ease.OutExpo);
		if (!show)
		{
			t.OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}
}
