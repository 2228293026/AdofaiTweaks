using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTabButton : GeneralPauseButton
{
	public Image rectangle;

	public Image fill;

	public Text label;

	public Image icon;

	private SettingsMenu settingsMenu => base.pauseMenu.settingsMenu;

	private void Awake()
	{
		rectangleRT = rectangle.GetComponent<RectTransform>();
		rectangle.enabled = false;
	}

	public override void SetFocus(bool focus)
	{
		SetFocus(focus, dontChangeSize: false);
	}

	public void SetFocus(bool focus, bool dontChangeSize)
	{
		if (!dontChangeSize)
		{
			Vector2 endValue = base.rectTransform.sizeDelta.WithX(focus ? 70f : base.rectTransform.sizeDelta.y);
			base.rectTransform.DOKill(complete: true);
			base.rectTransform.DOSizeDelta(endValue, base.pauseMenu.animationTime).SetEase(base.pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true);
		}
		Color color = (focus ? base.pauseMenu.selectedLabelColor : base.pauseMenu.unselectedLabelColor);
		Color endValue2 = ((!focus && dontChangeSize) ? base.pauseMenu.unselectedLabelColor : (focus ? base.pauseMenu.selectedLabelColor : Color.white.WithAlpha(0f)));
		Color color2 = ((!focus && dontChangeSize) ? Color.white.WithAlpha(0.1f) : (focus ? Color.white.WithAlpha(0.2f) : default(Color)));
		label.DOColor(endValue2, base.pauseMenu.animationTime).SetUpdate(isIndependentUpdate: true);
		icon.color = color;
		fill.color = color2;
	}

	public override void Select()
	{
		if (!settingsMenu.editingKeys)
		{
			settingsMenu.StopBrowsingTab();
			settingsMenu.SelectTab(this);
		}
	}
}
