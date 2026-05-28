using DG.Tweening;
using UnityEngine;

public class XRHare : ADOBase
{
	public Vector3 popoutOffset;

	public bool rightSide;

	[HideInInspector]
	public bool isUnderMouse;

	private Vector3 origPos;

	private Vector3 popoutPos;

	private void Awake()
	{
		origPos = base.transform.localPosition;
		popoutPos = origPos + popoutOffset * 100f;
		base.gameObject.SetActive(value: false);
		int width = Screen.width;
		float x = ADOBase.controller.camy.camobj.ScreenToWorldPoint(Vector3.zero.WithX(rightSide ? width : 0)).x;
		base.transform.parent.PositionX(x);
	}

	public void Show(bool show)
	{
		if (show)
		{
			base.gameObject.SetActive(value: true);
		}
		base.transform.DOKill();
		Tween t = base.transform.DOLocalMove(show ? popoutPos : origPos, 0.5f).SetEase(Ease.OutExpo);
		if (!show)
		{
			t.OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}

	private void OnMouseEnter()
	{
		isUnderMouse = true;
	}

	private void OnMouseExit()
	{
		isUnderMouse = false;
	}
}
