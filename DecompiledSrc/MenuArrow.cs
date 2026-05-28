using DG.Tweening;
using UnityEngine;

public class MenuArrow : MonoBehaviour
{
	public SpriteRenderer arrow;

	public SpriteRenderer shadow;

	public Transform arrTrans;

	public Transform shadTrans;

	public float pointDir;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Color fadedWhite = new Color(1f, 1f, 1f, 0.7f);

	private void Start()
	{
		DOTween.Sequence().Append(arrow.transform.DOLocalMoveX(0.3f, 0.2f).SetEase(Ease.InCubic).SetRelative(isRelative: true)).Append(arrow.transform.DOLocalMoveX(-0.3f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.AppendInterval(0.3f)
			.SetLoops(-1, LoopType.Restart)
			.SetUpdate(isIndependentUpdate: true);
		DOTween.Sequence().Append(shadow.transform.DOLocalMoveX(0.3f, 0.2f).SetEase(Ease.InCubic).SetRelative(isRelative: true)).Append(shadow.transform.DOLocalMoveX(-0.3f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true))
			.AppendInterval(0.3f)
			.SetLoops(-1, LoopType.Restart)
			.SetUpdate(isIndependentUpdate: true);
		arrTrans.transform.localEulerAngles = Vector3.forward * pointDir;
		shadTrans.transform.localEulerAngles = Vector3.forward * pointDir;
		FadeOut(0f);
	}

	public void FadeIn(float dur = 1f)
	{
		arrow.DOColor(fadedWhite, dur);
		shadow.DOColor(Color.white, dur);
	}

	public void FadeOut(float dur = 1f)
	{
		arrow.DOColor(whiteClear, dur);
		shadow.DOColor(whiteClear, dur);
	}
}
