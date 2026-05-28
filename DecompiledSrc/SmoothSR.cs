using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmoothSR : ScrollRect
{
	private const float rate = 0.2f;

	private const float maxAnimationTime = 0.2f;

	public bool smoothEnabled = true;

	public float targetPosition;

	private new void Awake()
	{
		base.Awake();
		targetPosition = base.verticalNormalizedPosition;
		base.scrollSensitivity = 0f;
	}

	public void ScrollTo(float position)
	{
		targetPosition = position;
	}

	public override void OnScroll(PointerEventData ped)
	{
		if (smoothEnabled)
		{
			targetPosition = Mathf.Clamp01(targetPosition + Input.mouseScrollDelta.y * 0.1f);
		}
	}

	private void Update()
	{
		if (smoothEnabled)
		{
			if (!Input.GetMouseButton(0))
			{
				float value = Mathf.Lerp(base.verticalNormalizedPosition, t: 1f - Mathf.Pow(0.8f, Time.unscaledDeltaTime * 60f), b: targetPosition);
				base.verticalNormalizedPosition = Mathf.Clamp01(value);
			}
			else
			{
				targetPosition = base.verticalNormalizedPosition;
			}
		}
	}
}
