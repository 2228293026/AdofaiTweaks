using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl : ADOBase
{
	public PropertyInfo propertyInfo;

	public PropertiesPanel propertiesPanel;

	public RectTransform propertyTransform;

	public RectTransform rectTransform;

	public PropertyControl randomControl;

	public virtual List<Selectable> selectables
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public virtual string text
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public virtual void OnRightClick()
	{
	}

	public virtual void ValidateInput()
	{
	}

	public virtual void Setup(bool addListener)
	{
	}

	public virtual void EnumSetup(string enumTypeString, List<string> enumVals, bool localize = true, List<string> customLabels = null, bool enableBrowse = false)
	{
	}

	public void OnValueChange()
	{
		if (!ADOBase.editor.particleEditor.gameObject.activeInHierarchy)
		{
			return;
		}
		ADOBase.editor.particleEditor.UpdatePreview(propertyInfo.dict.TryGetValueAs("restartParticle", out var valueAs, _default: false) && valueAs);
		foreach (PropertiesPanel value in ADOBase.editor.particleEditor.Tabs.Values)
		{
			foreach (Property value2 in value.properties.Values)
			{
				value2.control.UpdateEnabled();
			}
		}
	}

	public void ToggleOthersEnabled()
	{
		if (propertiesPanel.name == "LevelSettings")
		{
			Property property = propertiesPanel.properties["specialArtistType"];
			ApprovalLevelBadge approvalLevelBadge = ADOBase.editor.settingsPanel.approvalLevelBadge;
			bool flag = approvalLevelBadge == null || approvalLevelBadge.approvalLevel == ApprovalLevel.Pending;
			property.control.SetEnabled(flag);
			Property property2 = propertiesPanel.properties["artistPermission"];
			bool flag2 = false;
			if (approvalLevelBadge != null)
			{
				if (approvalLevelBadge.approvalLevel == ApprovalLevel.Pending)
				{
					flag2 = (SpecialArtistType)ADOBase.editor.settingsPanel.selectedEvent["specialArtistType"] == SpecialArtistType.None;
				}
			}
			else
			{
				flag2 = true;
			}
			property2.control.SetEnabled(flag2);
		}
		foreach (Property value in propertiesPanel.properties.Values)
		{
			if (!(value.info.name == "specialArtistType") && !(value.info.name == "artistPermission"))
			{
				value.control.UpdateEnabled();
			}
		}
	}

	public void UpdateEnabled()
	{
		bool flag = propertyInfo.CheckIfEnabled(propertiesPanel.inspectorPanel.selectedEvent, propertiesPanel.selectedTab);
		bool shown = propertyInfo.CheckIfShown(propertiesPanel.inspectorPanel.selectedEvent, propertiesPanel.selectedTab);
		SetEnabled(flag, shown);
	}

	public virtual void SetEnabled(bool enabled, bool shown = true)
	{
		Color color = (enabled ? Color.white : Color.gray);
		TMP_Text[] componentsInChildren = GetComponentsInChildren<TMP_Text>();
		foreach (TMP_Text tMP_Text in componentsInChildren)
		{
			if (!(tMP_Text.color == Color.black))
			{
				tMP_Text.color = color;
			}
		}
		Selectable[] componentsInChildren2 = GetComponentsInChildren<Selectable>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].interactable = enabled;
		}
		Slider[] componentsInChildren3 = GetComponentsInChildren<Slider>();
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			Image component = componentsInChildren3[i].transform.GetChild(0).GetChild(0).GetComponent<Image>();
			component.color = component.color.WithAlpha(enabled ? 1f : 0.5f);
		}
		propertyInfo.isEnabled = enabled;
		SetShown(shown);
	}

	public void SetShown(bool shown)
	{
		if (!propertyInfo.invisible)
		{
			_ = propertyTransform.gameObject.activeSelf != shown;
		}
		else
			_ = 0;
		propertyTransform.gameObject.SetActive(shown);
	}

	public void ApplyTileChanges()
	{
		if (propertyInfo.affectsPath)
		{
			ADOBase.editor.RemakePath();
			ADOBase.editor.DeselectAllDecorations();
			ADOBase.editor.UpdateDecorationObjects();
		}
		else if (propertyInfo.affectsFloors)
		{
			ADOBase.editor.ApplyEventsToFloors();
		}
	}

	public virtual void SetRandomLayout()
	{
	}

	public virtual void OnSelectedEventChanged(LevelEvent levelEvent)
	{
	}
}
