using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Bool : PropertyControl
{
	public Button offButton;

	public Button onButton;

	public Sprite selectedToggleBackground;

	public Sprite unselectedToggleBackground;

	private bool _value;

	public override List<Selectable> selectables => new List<Selectable> { onButton, offButton };

	public bool value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			UpdateButtons();
		}
	}

	public override void Setup(bool addListener)
	{
		if (addListener)
		{
			onButton.onClick.AddListener(delegate
			{
				SetValue(on: true);
			});
			offButton.onClick.AddListener(delegate
			{
				SetValue(on: false);
			});
		}
	}

	private void SetValue(bool on)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			value = on;
			selectedEvent[propertyInfo.name] = on;
			ToggleOthersEnabled();
			if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
			{
				ADOBase.customLevel.SetBackground();
			}
			else if (selectedEvent.IsDecoration)
			{
				ADOBase.editor.UpdateDecorationObject(selectedEvent);
			}
			else if (selectedEvent.eventType == LevelEventType.SetFilterAdvanced)
			{
				PropertyControl_FilterProperties obj = propertiesPanel.properties["filterProperties"].control as PropertyControl_FilterProperties;
				obj.enableProperties = true;
				obj.ReloadFilterProperties(selectedEvent);
			}
			ApplyTileChanges();
			OnValueChange();
		}
	}

	private void SetSelected(Button button, bool selected)
	{
		button.image.sprite = (selected ? selectedToggleBackground : unselectedToggleBackground);
		button.GetComponentInChildren<TMP_Text>().color = (selected ? Color.black : Color.white);
	}

	private void UpdateButtons()
	{
		SetSelected(onButton, value);
		SetSelected(offButton, !value);
	}
}
