using System.Collections.Generic;
using ADOFAI.Editor.Components;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Color : PropertyControl
{
	public ColorField colorField;

	public bool usesAlpha => propertyInfo.color_usesAlpha;

	public override List<Selectable> selectables => new List<Selectable> { colorField.field };

	public override string text
	{
		get
		{
			return colorField.value;
		}
		set
		{
			colorField.value = value;
		}
	}

	public override void Setup(bool addListener)
	{
		if (addListener)
		{
			colorField.onChange.AddListener(OnChange);
		}
		colorField.colorPickerPopup = ADOBase.editor.colorPickerPopup;
		colorField.usesAlpha = usesAlpha;
		colorField.defaultValue = (string)propertyInfo.value_default;
	}

	public override void ValidateInput()
	{
		colorField.value = colorField.value;
	}

	public void OnChange(string s)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			selectedEvent[propertyInfo.name] = s;
			if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
			{
				ADOBase.customLevel.SetBackground();
			}
			else if (selectedEvent.IsDecoration)
			{
				ADOBase.editor.UpdateDecorationObject(selectedEvent);
			}
			ApplyTileChanges();
			OnValueChange();
		}
	}
}
