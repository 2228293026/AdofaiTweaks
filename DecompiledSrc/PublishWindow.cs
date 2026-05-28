using System.Collections.Generic;
using System.IO;
using ADOFAI;
using DG.Tweening;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PublishWindow : ADOBase
{
	[Header("Publish Popup")]
	public GameObject windowContainer;

	public GameObject publishContainer;

	public GameObject updateContainer;

	public GameObject resultContainer;

	public GameObject confirmContainer;

	public GameObject buttonsSteam;

	public TMP_Text txtPopupTitle;

	public TMP_Text txtTitle;

	public TMP_Text txtCreator;

	public TMP_Text txtDescription;

	public TMP_Text txtLevelCount;

	public TMP_Text txtTags;

	public TMP_Text thumbnailText;

	public TMP_Text txtResultText;

	public Image portal;

	public RawImage icon;

	public scrBlur iconBlur;

	public DifficultyIndicator difficulty;

	public Button btnPublish;

	public Button btnExportZip;

	public Button btnCancel;

	public Button btnGoToUpdate;

	public Button btnStartUpdate;

	public Button btnBack;

	public Button btnSetThumbnail;

	public Button btnFinish;

	public Button btnViewUploadedItem;

	public Button btnSublevelInfo;

	public Button btnCancelConfirmation;

	public Button btnWorkshopAgreement;

	public Button btnConfirmPublish;

	public Button btnResultAcceptAgreement;

	public Slider publishSlider;

	public Slider updateSlider;

	public TMP_Text publishBtnText;

	public TMP_Text updateBtnText;

	public WorkshopLevelList workshopLevelList;

	public Toggle previewAgreementToggle;

	public Toggle updateAgreementToggle;

	public bool uploadInProgress;

	private bool publishActionIsUpdate;

	private bool lastUploadInProgress;

	private string workshopThumbnailPath;

	private float uploadProgress;

	private void Start()
	{
		btnPublish.onClick.AddListener(delegate
		{
			PerformPublishAction(publishActionIsUpdate: false);
		});
		btnExportZip.onClick.AddListener(delegate
		{
			PerformExportZipAction();
		});
		btnCancel.onClick.AddListener(delegate
		{
			ADOBase.editor.ShowPopup(show: false, scnEditor.PopupType.ExportLevel);
		});
		btnFinish.onClick.AddListener(delegate
		{
			ADOBase.editor.ShowPopup(show: false, scnEditor.PopupType.ExportLevel);
		});
		btnViewUploadedItem.onClick.AddListener(delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			SteamWorkshop.ShowItemOnWorkshop(SteamWorkshop.lastPublishedFileId);
		});
		btnGoToUpdate.onClick.AddListener(delegate
		{
			ChangePage(1);
		});
		btnBack.onClick.AddListener(delegate
		{
			ChangePage(0);
		});
		btnStartUpdate.onClick.AddListener(delegate
		{
			PerformPublishAction(publishActionIsUpdate: true);
		});
		btnSublevelInfo.onClick.AddListener(delegate
		{
			ADOBase.editor.ShowPropertyHelp(show: true, btnSublevelInfo.transform, RDString.Get("editor.dialog.sublevelHelp", new Dictionary<string, object> { 
			{
				"level",
				Path.GetFileNameWithoutExtension(ADOBase.levelPath)
			} }));
		});
		btnCancelConfirmation.onClick.AddListener(delegate
		{
			ShowConfirmation(show: false);
		});
		btnConfirmPublish.onClick.AddListener(delegate
		{
			PerformPublishAction();
		});
		btnWorkshopAgreement.onClick.AddListener(delegate
		{
			OpenAgreement();
		});
		btnResultAcceptAgreement.onClick.AddListener(delegate
		{
			OpenAgreement();
		});
	}

	private void OpenAgreement()
	{
		Application.OpenURL("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
	}

	private void UpdateLevel()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		PublishedFileId_t id = workshopLevelList.foundItems[workshopLevelList.selectedIndex].id;
		ADOBase.editor.PublishToSteam(id);
	}

	private void Update()
	{
		if (uploadInProgress)
		{
			UpdateProgress();
		}
	}

	public void UpdateProgress()
	{
		float num = Mathf.Max(uploadProgress, SteamWorkshop.itemUploadProgress);
		if (uploadInProgress)
		{
			publishSlider.value = Mathf.Max(num, publishSlider.value);
			updateSlider.value = Mathf.Max(num, updateSlider.value);
			int num2 = Mathf.RoundToInt(num * 100f);
			publishBtnText.text = string.Format("{0}  {1}%", RDString.Get("editor.dialog.publishing"), num2);
			updateBtnText.text = string.Format("{0}  {1}%", RDString.Get("editor.dialog.updating"), num2);
		}
		if (lastUploadInProgress != uploadInProgress)
		{
			lastUploadInProgress = uploadInProgress;
			btnBack.interactable = !uploadInProgress;
			btnPublish.interactable = !uploadInProgress;
			btnCancel.interactable = !uploadInProgress;
			btnGoToUpdate.interactable = !uploadInProgress;
			btnStartUpdate.interactable = !uploadInProgress;
			publishSlider.gameObject.SetActive(uploadInProgress);
			updateSlider.gameObject.SetActive(uploadInProgress);
			previewAgreementToggle.interactable = !uploadInProgress;
			updateAgreementToggle.interactable = !uploadInProgress;
			if (!uploadInProgress)
			{
				publishBtnText.text = RDString.Get("editor.dialog.publishToWorkshop");
				updateBtnText.text = RDString.Get("editor.dialog.updateSelectedWorld");
				uploadProgress = 0f;
			}
		}
	}

	public void Init()
	{
		bool active = ADOBase.isSteamworks && SteamManager.Initialized;
		txtPopupTitle.text = RDString.Get("editor.export");
		buttonsSteam.SetActive(active);
		btnWorkshopAgreement.gameObject.SetActive(active);
		previewAgreementToggle.gameObject.SetActive(active);
		ChangePage(0);
		LevelData levelData = ADOBase.editor.levelData;
		txtTitle.text = levelData.fullCaption;
		txtCreator.text = "<color=#999999>" + RDString.Get("editor.publish.by") + "</color> " + levelData.author;
		txtDescription.text = levelData.levelDesc;
		txtTags.text = levelData.levelTags;
		int num = ADOBase.customLevel.GetWorldPaths().Length;
		txtLevelCount.text = string.Format("<b>{0}</b> {1}", RDString.Get("editor.publish.tutorialLevels"), num - 1);
		difficulty.SetStars(levelData.difficulty);
		levelData.RefreshRequiredDLC();
		string text = ADOBase.editor.MakeThumbnail(levelData.requiredDLC);
		Sprite sprite = ADOBase.gc.sprite_defaultPortal;
		LoadResult status;
		if (File.Exists(text))
		{
			sprite = TextureManager.LoadNewSprite(text, out status);
			if (!sprite)
			{
				sprite = ADOBase.gc.sprite_defaultPortal;
			}
		}
		portal.sprite = sprite;
		string text2 = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), levelData.previewIcon);
		Texture2D texture2D = ADOBase.gc.tex_defaultIcon;
		if (File.Exists(text2))
		{
			texture2D = TextureManager.LoadTexture(text2, out status);
			if (!texture2D)
			{
				texture2D = ADOBase.gc.tex_defaultIcon;
			}
		}
		icon.texture = texture2D;
		Color previewIconColor = levelData.previewIconColor;
		iconBlur.baseTint = previewIconColor;
		iconBlur.blurTint = Color.black;
		iconBlur.UpdateTexture();
		confirmContainer.GetComponent<RectTransform>().ScaleXY(0f, 0f);
		previewAgreementToggle.isOn = false;
		updateAgreementToggle.isOn = false;
	}

	public void ChangePage(int page)
	{
		publishContainer.SetActive(page == 0);
		updateContainer.SetActive(page == 1);
		resultContainer.SetActive(page == 2);
		switch (page)
		{
		case 0:
			previewAgreementToggle.isOn = updateAgreementToggle.isOn;
			break;
		case 1:
			updateAgreementToggle.isOn = previewAgreementToggle.isOn;
			workshopLevelList.Refresh();
			break;
		case 2:
		{
			bool mustAcceptWorkshopLegalAgreement = SteamWorkshop.mustAcceptWorkshopLegalAgreement;
			string text = RDString.Get("editor.dialog.successfullyUploaded");
			if (mustAcceptWorkshopLegalAgreement)
			{
				text = text + "\n" + RDString.Get("editor.dialog.askToAcceptAgreement");
			}
			txtResultText.text = text;
			btnResultAcceptAgreement.gameObject.SetActive(mustAcceptWorkshopLegalAgreement);
			break;
		}
		}
	}

	private void ShowConfirmation(bool show, bool publishActionIsUpdate = false)
	{
		this.publishActionIsUpdate = publishActionIsUpdate;
		if (show)
		{
			confirmContainer.SetActive(value: true);
		}
		confirmContainer.GetComponent<RectTransform>().DOScale(show ? Vector3.one : Vector3.zero, 0.25f).SetEase(show ? Ease.OutBack : Ease.InSine)
			.SetUpdate(isIndependentUpdate: true);
	}

	private void PerformPublishAction()
	{
		if (publishActionIsUpdate)
		{
			UpdateLevel();
		}
		else
		{
			ADOBase.editor.ExportLevel(uploadToSteam: true);
		}
		ShowConfirmation(show: false);
	}

	private void PerformPublishAction(bool publishActionIsUpdate)
	{
		Toggle toggle = (publishActionIsUpdate ? updateAgreementToggle : previewAgreementToggle);
		if (!toggle.isOn)
		{
			ColorBlock colors = toggle.colors;
			colors.normalColor = Color.red;
			colors.highlightedColor = Color.red;
			toggle.colors = colors;
		}
		else
		{
			if (publishActionIsUpdate)
			{
				UpdateLevel();
			}
			else
			{
				ADOBase.editor.ExportLevel(uploadToSteam: true);
			}
			ShowConfirmation(show: false);
		}
	}

	private void PerformExportZipAction()
	{
		ADOBase.editor.ExportLevel();
		ShowConfirmation(show: false);
	}
}
