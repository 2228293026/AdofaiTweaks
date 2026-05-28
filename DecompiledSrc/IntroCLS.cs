using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IntroCLS : ADOBase
{
	private const float animDuration = 0.15f;

	private const float visibleY = 810f;

	private const float hiddenY = 0f;

	public RectTransform panel;

	public Image backgroundImage;

	public Image leftOption;

	public Image rightOption;

	private bool rightSelected;

	public Color unhighlightedColor = Color.clear;

	public Color highlightedColor = new Color(1f, 1f, 1f, 0.75f);

	private void Start()
	{
		ADOBase.controller.isCutscene = true;
		SelectOption(right: true);
		panel.AnchorPosY(0f);
		panel.DOAnchorPosY(810f, 0.3f).SetDelay(0.5f).SetEase(Ease.OutBack);
	}

	private void SelectOption(bool right)
	{
		if (rightSelected != right)
		{
			leftOption.DOKill();
			leftOption.DOColor(right ? unhighlightedColor : highlightedColor, 0.15f);
			rightOption.DOKill();
			rightOption.DOColor(right ? highlightedColor : unhighlightedColor, 0.15f);
			rightSelected = right;
		}
	}

	private void Update()
	{
		if (RDInput.leftPress)
		{
			SelectOption(right: false);
		}
		else if (RDInput.rightPress)
		{
			SelectOption(right: true);
		}
		else if (RDInput.confirmPress && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && Input.touchCount == 0)
		{
			TriggerAction(rightSelected);
		}
	}

	public void TriggerAction(bool right)
	{
		SelectOption(right);
		if (right && ADOBase.controller.isCutscene)
		{
			ADOBase.controller.isCutscene = false;
			Persistence.displayedCLSIntro = true;
			panel.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InBack).OnComplete(delegate
			{
				backgroundImage.DOColor(unhighlightedColor, 0.15f).OnComplete(delegate
				{
					base.gameObject.SetActive(value: false);
				});
			});
		}
		else
		{
			SteamWorkshop.OpenWorkshop();
		}
	}
}
