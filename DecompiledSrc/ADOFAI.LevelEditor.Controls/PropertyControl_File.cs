using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_File : PropertyControl
{
	public TMP_InputField inputField;

	public Image border;

	public Image verticalLine;

	public BrowseButton browseButton;

	public override string text
	{
		get
		{
			return inputField.text;
		}
		set
		{
			inputField.text = value;
			browseButton.filename = value;
		}
	}

	public override List<Selectable> selectables => new List<Selectable> { inputField, browseButton.button };

	private void Awake()
	{
		browseButton.Initialize(this, inputField, ProcessFile);
	}

	private void Update()
	{
		Color color = Color.white.WithAlpha(inputField.interactable ? 1f : 0.4f);
		border.color = color;
		verticalLine.color = color;
	}

	public override void OnRightClick()
	{
		LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
		if (string.IsNullOrEmpty(selectedEvent[propertyInfo.name] as string))
		{
			return;
		}
		using (new SaveStateScope(ADOBase.editor))
		{
			browseButton.filename = "";
			selectedEvent[propertyInfo.name] = browseButton.filename;
			inputField.text = browseButton.filename;
			ToggleOthersEnabled();
			_ = propertyInfo.fileType;
			if (propertyInfo.fileType == FileType.Audio)
			{
				ADOBase.editor.levelData.songFilename = browseButton.filename;
				ADOBase.editor.UpdateSongAndLevelSettings();
			}
			else if (propertyInfo.fileType == FileType.Image)
			{
				if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
				{
					ADOBase.customLevel.SetBackground();
				}
				else if (selectedEvent.IsDecoration)
				{
					ADOBase.editor.UpdateDecorationObject(selectedEvent);
					if (selectedEvent.eventType == LevelEventType.AddParticle)
					{
						ADOBase.editor.particleEditor.SetEvent(selectedEvent);
					}
				}
				else
				{
					ADOBase.customLevel.UpdateBackgroundSprites();
				}
			}
			else if (propertyInfo.fileType == FileType.Video)
			{
				VideoPlayer videoBG = ADOBase.customLevel.videoBG;
				videoBG.Stop();
				((Component)(object)videoBG).gameObject.SetActive(value: false);
			}
		}
	}

	public void ProcessFile(string newFilename, FileType fileType)
	{
		if (!browseButton.CheckIfLevelIsSaved() || newFilename == null || browseButton.filename == newFilename)
		{
			return;
		}
		if (newFilename != "")
		{
			File.Exists(Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), newFilename));
		}
		switch (fileType)
		{
		case FileType.Audio:
		{
			LevelEvent selectedEvent3 = propertiesPanel.inspectorPanel.selectedEvent;
			browseButton.filename = newFilename;
			ToggleOthersEnabled();
			if (Path.GetExtension(browseButton.filename).Replace(".", string.Empty) == "mp3")
			{
				ADOBase.editor.soundToConvert = browseButton.filename;
				ADOBase.editor.soundConversionCallback = ADOBase.editor.SetLevelSong;
				ADOBase.editor.ShowPopup(show: true, scnEditor.PopupType.OggEncode);
				return;
			}
			selectedEvent3[propertyInfo.name] = browseButton.filename;
			inputField.text = browseButton.filename;
			ADOBase.editor.UpdateSongAndLevelSettings();
			break;
		}
		case FileType.Image:
			using (new SaveStateScope(ADOBase.editor))
			{
				LevelEvent selectedEvent2 = propertiesPanel.inspectorPanel.selectedEvent;
				browseButton.filename = newFilename;
				selectedEvent2[propertyInfo.name] = browseButton.filename;
				inputField.text = browseButton.filename;
				ToggleOthersEnabled();
				if (selectedEvent2.eventType == LevelEventType.BackgroundSettings)
				{
					ADOBase.customLevel.SetBackground();
				}
				else if (selectedEvent2.eventType == LevelEventType.AddDecoration)
				{
					ADOBase.editor.UpdateDecorationObject(selectedEvent2);
				}
				else if (selectedEvent2.eventType == LevelEventType.AddParticle)
				{
					ADOBase.editor.UpdateDecorationObject(selectedEvent2);
					ADOBase.editor.particleEditor.SetEvent(selectedEvent2);
				}
				else if (selectedEvent2.eventType == LevelEventType.MoveDecorations)
				{
					if (selectedEvent2.TryGet<string>("decorationImage", out var output) && !string.IsNullOrEmpty(output))
					{
						string filePath = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), output);
						ADOBase.customLevel.imgHolder.AddSprite(output, filePath, out var status);
						ADOBase.editor.UpdateImageLoadResult(output, status);
					}
				}
				else if (propertyInfo.affectsFloors || selectedEvent2.eventType == LevelEventType.ColorTrack)
				{
					ADOBase.customLevel.UpdateFloorSprites();
					ADOBase.editor.ApplyEventsToFloors();
				}
				else
				{
					ADOBase.customLevel.UpdateBackgroundSprites();
				}
			}
			break;
		case FileType.Video:
		{
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			browseButton.filename = newFilename;
			selectedEvent[propertyInfo.name] = browseButton.filename;
			inputField.text = browseButton.filename;
			VideoPlayer videoBG = ADOBase.customLevel.videoBG;
			((Component)(object)videoBG).gameObject.SetActive(value: true);
			videoBG.url = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), (string)ADOBase.editor.levelData.miscSettings["bgVideo"]);
			videoBG.Stop();
			videoBG.Prepare();
			ToggleOthersEnabled();
			break;
		}
		}
		OnValueChange();
	}
}
