using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApprovalLevelBadge : ADOBase
{
	public TMP_Text text;

	public Image background;

	public RectTransform rectTransform;

	public Button button;

	[NonSerialized]
	public ApprovalLevel approvalLevel;

	public Sprite defaultSprite;

	public Sprite onlyColorSprite;

	public float defaultSize;

	public float onlyColorSize;

	private ArtistData currentArtistData;

	public void UpdateUI(ApprovalLevel approvalLevel, bool onlyColor = false)
	{
		this.approvalLevel = approvalLevel;
		if (approvalLevel == ApprovalLevel.Pending)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		switch (approvalLevel)
		{
		case ApprovalLevel.Allowed:
			background.color = ADOBase.gc.allowedColor;
			break;
		case ApprovalLevel.Declined:
			background.color = ADOBase.gc.declinedColor;
			break;
		case ApprovalLevel.PartiallyDeclined:
			background.color = ADOBase.gc.particallyDeclinedColor;
			break;
		case ApprovalLevel.ListingRejected:
			background.color = ADOBase.gc.listingRejectedColor;
			break;
		}
		if (onlyColor)
		{
			text.text = string.Empty;
			rectTransform.sizeDelta = new Vector2(defaultSize, defaultSize);
			background.sprite = onlyColorSprite;
			rectTransform.sizeDelta = new Vector2(onlyColorSize, onlyColorSize);
		}
		else
		{
			background.sprite = defaultSprite;
			text.text = RDString.Get("enum.ApprovalLevel." + approvalLevel);
			rectTransform.sizeDelta = new Vector2(text.preferredWidth + 8f, defaultSize);
		}
	}

	public void ShowDisclaimer()
	{
		ADOBase.editor.settingsPanel.currentArtistDisclaimerAction();
	}
}
