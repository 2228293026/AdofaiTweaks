using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ADOFAI.Editor.Components;

public class Switch : MonoBehaviour
{
	public Image background;

	public RectTransform handle;

	public Button button;

	public Vector2 offPosition;

	public Vector2 onPosition;

	public Color offBackground;

	public Color onBackground;

	private bool _value;

	public bool value;

	public UnityEvent<bool> onToggle = new UnityEvent<bool>();

	public void SetValue(bool check, bool immediate = false)
	{
		value = (_value = check);
		handle.DOKill();
		background.DOKill();
		if (immediate)
		{
			handle.anchoredPosition = (check ? onPosition : offPosition);
			background.color = (check ? onBackground : offBackground);
		}
		else
		{
			handle.DOAnchorPos(check ? onPosition : offPosition, 0.2f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
			background.DOColor(check ? onBackground : offBackground, 0.2f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
		}
	}

	private void Awake()
	{
		button.onClick.AddListener(OnClick);
		SetValue(value, immediate: true);
	}

	private void Update()
	{
		if (_value != value)
		{
			SetValue(value);
		}
	}

	private void OnClick()
	{
		SetValue(!value);
		onToggle.Invoke(value);
	}
}
