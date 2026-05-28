using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class BrowseButton : ADOBase
{
	public Button button;

	public Image buttonIcon;

	[NonSerialized]
	public string filename;

	private PropertyControl owner;

	private TMP_InputField inputField;

	private Action<string, FileType> processFileFunction;

	private bool initialized;

	public void Initialize(PropertyControl owner, TMP_InputField inputField, Action<string, FileType> processFileFunction)
	{
		this.owner = owner;
		this.inputField = inputField;
		this.processFileFunction = processFileFunction;
		button.onClick.AddListener(delegate
		{
			BrowseFile();
		});
		inputField.onEndEdit.AddListener(delegate(string s)
		{
			this.processFileFunction(s, owner.propertyInfo.fileType);
		});
		initialized = true;
	}

	private void Update()
	{
		if (initialized)
		{
			button.interactable = inputField.interactable;
			Color color = Color.white.WithAlpha(inputField.interactable ? 1f : 0.4f);
			buttonIcon.color = color;
		}
	}

	public bool CheckIfLevelIsSaved()
	{
		if (string.IsNullOrEmpty(ADOBase.levelPath))
		{
			scnEditor.PopupType popupType = scnEditor.PopupType.SaveBeforeSongImport;
			switch (owner.propertyInfo.fileType)
			{
			case FileType.Audio:
				popupType = scnEditor.PopupType.SaveBeforeSongImport;
				break;
			case FileType.Image:
				popupType = scnEditor.PopupType.SaveBeforeImageImport;
				break;
			case FileType.Video:
				popupType = scnEditor.PopupType.SaveBeforeVideoImport;
				break;
			}
			ADOBase.editor.ShowPopup(show: true, popupType);
			return false;
		}
		return true;
	}

	private void BrowseFile()
	{
		if (CheckIfLevelIsSaved())
		{
			string arg = null;
			FileType fileType = owner.propertyInfo.fileType;
			switch (fileType)
			{
			case FileType.Audio:
				arg = RDEditorUtils.ShowFileSelectorForAudio(RDString.Get("editor.dialog.selectSound"), -1L);
				break;
			case FileType.Image:
				arg = RDEditorUtils.ShowFileSelectorForImage(RDString.Get("editor.dialog.selectImage"), -1L);
				break;
			case FileType.Video:
				arg = RDEditorUtils.ShowFileSelectorForVideo(RDString.Get("editor.dialog.selectVideo"), -1L);
				break;
			}
			processFileFunction(arg, fileType);
		}
	}
}
