using DG.Tweening;
using UnityEngine;

public class scrFloatTween : MonoBehaviour
{
	public float distance;

	public float duration;

	private void Start()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + distance, base.transform.localPosition.z);
		base.transform.DOLocalMoveY(base.transform.localPosition.y - distance * 2f, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(isIndependentUpdate: true);
	}
}
