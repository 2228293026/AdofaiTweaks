using System.Collections.Generic;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtistUIDisclaimer : ADOBase
{
	public const string SelectedArtistKey = "editor.artistDisclaimer.selected";

	public const string ApprovalLevelToken = "editor.artistDisclaimer.";

	public const string ConditionKey = "editor.artistDisclaimer.condition";

	public const string ConditionDescKey = "editor.artistDisclaimer.conditionDescription";

	public const string ConditionDeclinedDescKey = "editor.artistDisclaimer.conditionDeclinedDescription";

	public const string AcceptConditionKey = "editor.artistDisclaimer.acceptCondition";

	public const string ConditionListingRejectedKey = "editor.artistDisclaimer.conditionListingRejectedDescription";

	public const string TitleKey = "editor.artistDisclaimer.title";

	public const string ConfirmKey = "editor.artistDisclaimer.confirm";

	public const string CancelKey = "editor.artistDisclaimer.cancel";

	public const string CloseKey = "editor.artistDisclaimer.close";

	public TMP_Text selectedArtistText;

	public TMP_Text approvalLevelText;

	public TMP_Text conditionText;

	public TMP_Text conditionDescriptionText;

	public TMP_Text conditionCheckText;

	public GameObject confirm;

	public GameObject cancel;

	public GameObject close;

	public RectTransform selectedArtistRect;

	public RectTransform approvalLevelRect;

	public RectTransform conditionDescRect;

	public RectTransform conditionCheckRect;

	public Toggle conditionCheckToggle;

	public TMP_Text titleText;

	public TMP_Text confirmText;

	public TMP_Text cancelText;

	public TMP_Text closeText;

	public RectTransform rectTransform;

	public VerticalLayoutGroup verticalLayoutGroup;

	public GameObject fader;

	private PropertyControl_Text artistPropertyControl;

	private ArtistData currentArtistData;

	private bool onlyChecking;

	private ApprovalLevelBadge ApprovalLevelBadge
	{
		get
		{
			return ADOBase.editor.settingsPanel.approvalLevelBadge;
		}
		set
		{
			ADOBase.editor.settingsPanel.approvalLevelBadge = value;
		}
	}

	private void Awake()
	{
		titleText.text = RDString.Get("editor.artistDisclaimer.title");
		confirmText.text = RDString.Get("editor.artistDisclaimer.confirm");
		cancelText.text = RDString.Get("editor.artistDisclaimer.cancel");
		closeText.text = RDString.Get("editor.artistDisclaimer.close");
	}

	public void SetData(ArtistData data, PropertyControl_Text artistPC, bool onlyCheckingMode)
	{
		currentArtistData = data;
		artistPropertyControl = artistPC;
		onlyChecking = onlyCheckingMode;
		selectedArtistText.text = RDString.Get("editor.artistDisclaimer.selected", new Dictionary<string, object> { { "artist", data.name } });
		string key = "editor.artistDisclaimer." + data.approvalLevel;
		approvalLevelText.text = RDString.Get(key);
		conditionText.text = RDString.Get("editor.artistDisclaimer.condition", new Dictionary<string, object> { { "artist", data.name } });
		ApprovalLevel approvalLevel = data.approvalLevel;
		conditionDescriptionText.text = RDString.Get(approvalLevel switch
		{
			ApprovalLevel.Declined => "editor.artistDisclaimer.conditionDeclinedDescription", 
			ApprovalLevel.ListingRejected => "editor.artistDisclaimer.conditionListingRejectedDescription", 
			_ => "editor.artistDisclaimer.conditionDescription", 
		}, new Dictionary<string, object> { ["artist"] = data.name });
		if (data.approvalLevel == ApprovalLevel.Declined && !GCS.isDev)
		{
			confirm.SetActive(value: false);
			cancel.SetActive(value: false);
			close.SetActive(value: true);
			conditionCheckRect.gameObject.SetActive(value: false);
		}
		else
		{
			cancel.SetActive(value: true);
			close.SetActive(value: false);
			conditionCheckRect.gameObject.SetActive(!onlyChecking);
			confirm.SetActive(!onlyChecking);
			if (!onlyChecking)
			{
				confirm.GetComponent<Button>().interactable = false;
				conditionCheckToggle.isOn = false;
			}
		}
		fader.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		ADOBase.editor.settingsPanel.HideArtistDropdown();
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = currentArtistData.name;
			artistPropertyControl.inputField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
			string text = string.Join(",", currentArtistData.GetLinks());
			LevelEvent levelSettings = ADOBase.editor.levelData.levelSettings;
			levelSettings["artistLinks"] = text;
			PropertyControl control = ADOBase.editor.settingsPanel.GetPanelOfType(levelSettings).properties["artistLinks"].control;
			if (string.IsNullOrEmpty(control.text) && !string.IsNullOrEmpty(text))
			{
				control.text = text;
			}
		}
	}

	public void ShowArtistDisclaimer()
	{
		ADOBase.platformHelper.OpenURL("https://7thbe.at/verified-artists/adofai/artist/" + currentArtistData.id);
	}

	public void Confirm()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = currentArtistData.name;
			artistPropertyControl.inputField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
		}
		Hide();
	}

	public void Cancel()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = string.Empty;
			artistPropertyControl.inputField.text = string.Empty;
			ApprovalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
		}
		Hide();
	}

	private void Hide()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.settingsPanel.HideArtistDropdown();
		}
		fader.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		ADOBase.editor.popupBlocker.gameObject.SetActive(value: false);
	}
}
