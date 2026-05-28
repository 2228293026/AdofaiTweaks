using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
[RequireComponent(typeof(Selectable))]
public class ScrollRectSelectableChild : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public bool useCustomEdgePadding;

	public float customEdgePadding = 50f;

	private ScrollRect parentScrollRect;

	private Selectable _selectable;

	private RectTransform parentScrollRectContentTransform => parentScrollRect.content;

	private Selectable selectable => _selectable ?? (_selectable = GetComponent<Selectable>());

	private RectTransform rectTransform => base.transform as RectTransform;

	private void Start()
	{
		parentScrollRect = base.transform.GetComponentInParent<ScrollRect>();
		if (parentScrollRect == null)
		{
			Debug.LogError("Rewired Control Mapper: No ScrollRect found! This component must be a child of a ScrollRect!");
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (!(parentScrollRect == null) && eventData is AxisEventData)
		{
			RectTransform rectTransform = parentScrollRect.transform as RectTransform;
			Rect rect = MathTools.TransformRect(this.rectTransform.rect, (Transform)this.rectTransform, (Transform)rectTransform);
			Rect rect2 = rectTransform.rect;
			Rect rect3 = rectTransform.rect;
			float num = ((!useCustomEdgePadding) ? rect.height : customEdgePadding);
			rect3.yMax -= num;
			rect3.yMin += num;
			Vector2 vector = default(Vector2);
			if (!MathTools.RectContains(rect3, rect) && MathTools.GetOffsetToContainRect(rect3, rect, ref vector))
			{
				Vector2 anchoredPosition = parentScrollRectContentTransform.anchoredPosition;
				anchoredPosition.x = Mathf.Clamp(anchoredPosition.x + vector.x, 0f, Mathf.Abs(rect2.width - parentScrollRectContentTransform.sizeDelta.x));
				anchoredPosition.y = Mathf.Clamp(anchoredPosition.y + vector.y, 0f, Mathf.Abs(rect2.height - parentScrollRectContentTransform.sizeDelta.y));
				parentScrollRectContentTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}
