using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_LongText : PropertyControl
{
	public TMP_InputField inputField;

	public TMP_InputField.SubmitEvent onEndEdit => inputField.onEndEdit;

	public override List<Selectable> selectables => new List<Selectable> { inputField };

	public override string text
	{
		get
		{
			return inputField.text;
		}
		set
		{
			inputField.text = value;
		}
	}

	public override void Setup(bool addListener)
	{
		inputField.textComponent.transform.position += new Vector3(0f, -7f, 0f);
		ColorBlock colors = inputField.colors;
		colors.selectedColor = InspectorPanel.selectionColor;
		inputField.colors = colors;
		if (!addListener)
		{
			return;
		}
		inputField.onEndEdit.AddListener(delegate
		{
			using (new SaveStateScope(ADOBase.editor))
			{
				propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.name] = inputField.text;
				ApplyTileChanges();
				if (ADOBase.editor.SelectionIsSingle())
				{
					ADOBase.editor.ShowEventIndicators(ADOBase.editor.selectedFloors[0]);
				}
				OnValueChange();
			}
		});
	}

	public override void ValidateInput()
	{
	}
}
