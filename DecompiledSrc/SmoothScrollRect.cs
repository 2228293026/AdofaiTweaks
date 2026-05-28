using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class SmoothScrollRect : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public new bool enabled = true;

	public ScrollRect scrollRect;

	public float scrollSensitivity = 1f;

	public float tweenDuration = 0.2f;

	public Ease ease = Ease.OutCubic;

	private const float scrollMultiplier = 1f;

	private RectTransform contentRect;

	private RectTransform viewportRect;

	private float targetPosition;

	private Tween currentTween;

	private bool lastDirectionIsUp;

	private void Awake()
	{
		if (!scrollRect)
		{
			scrollRect = GetComponent<ScrollRect>();
		}
		scrollRect.onValueChanged.AddListener(OnScroll);
		contentRect = scrollRect.content;
		viewportRect = scrollRect.viewport;
		scrollRect.scrollSensitivity = 0f;
	}

	public void OnScroll(PointerEventData evt)
	{
		if (enabled)
		{
			bool flag = currentTween != null;
			bool flag2 = evt.scrollDelta.y > 0f;
			bool flag3 = flag2 != lastDirectionIsUp;
			lastDirectionIsUp = flag2;
			if ((!flag2 || !(scrollRect.verticalNormalizedPosition >= 1f)) && (flag2 || !(scrollRect.verticalNormalizedPosition <= 0f)))
			{
				float num = evt.scrollDelta.y * scrollSensitivity * 1f * -1f;
				targetPosition = Mathf.Clamp(((flag3 || !flag) ? contentRect.anchoredPosition.y : targetPosition) + num, 0f, contentRect.rect.height - viewportRect.rect.height);
				ScrollTo(targetPosition);
			}
		}
	}

	public void ScrollTo(float position)
	{
		currentTween?.Kill();
		currentTween = DOTween.To(() => contentRect.anchoredPosition.y, contentRect.SetAnchorPosY, position, tweenDuration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				currentTween = null;
			});
	}

	private void OnScroll(Vector2 value)
	{
		if (enabled && Input.mouseScrollDelta.y == 0f && currentTween == null)
		{
			targetPosition = contentRect.anchoredPosition.y;
		}
	}
}
