using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweakableDropdownItem : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Header("UI")]
	public Image background;

	public Image checkmark;

	public TMP_Text text;

	[Header("Runtime")]
	public TweakableDropdown dropdown;

	public bool isChecked;

	public bool isVisible;

	public bool isArrowSelected;

	public bool isReadonly;

	public string value;

	public int index;

	public bool localizeValue;

	public bool useCustomLabel;

	public string customLabel;

	public string localizedValue
	{
		get
		{
			if (localizeValue)
			{
				return RDString.GetEnumValue(dropdown.enumTypeString, value);
			}
			if (useCustomLabel)
			{
				return customLabel;
			}
			return value;
		}
	}

	private Color contextualColor
	{
		get
		{
			Color result = (isArrowSelected ? dropdown.selectedItemBGColor : dropdown.normalItemBGColor);
			if (IsHighlighted())
			{
				result += dropdown.hoveredItemBGColor - dropdown.normalItemBGColor;
			}
			return result;
		}
	}

	public void ResetVisuals()
	{
		isVisible = true;
		text.text = localizedValue;
		background.color = contextualColor;
	}

	public void SetChecked(bool check)
	{
		checkmark.gameObject.SetActive(check);
		isChecked = check;
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
		isVisible = visible;
	}

	public void OnArrowSelect(bool selected)
	{
		if (!isReadonly && (selected ^ isArrowSelected))
		{
			bool flag = dropdown.arrowSelectedDropdownItems.Contains(this);
			if (isArrowSelected = selected && !flag)
			{
				dropdown.arrowSelectedDropdownItems.Add(this);
			}
			else if (flag)
			{
				dropdown.arrowSelectedDropdownItems.Remove(this);
			}
			background.color = contextualColor;
		}
	}

	public void OnSearch(string searchText)
	{
		string text = localizedValue;
		int num = SanitizeSearchString(text).IndexOf(SanitizeSearchString(searchText));
		if (num >= 0)
		{
			this.text.text = text.Insert(num + searchText.Length, "</color>").Insert(num, "<color=" + dropdown.searchedItemTextColor.ToHex() + ">");
			SetVisible(visible: true);
			return;
		}
		SetVisible(visible: false);
		this.text.text = text;
		isArrowSelected = false;
		background.color = contextualColor;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		background.color = contextualColor;
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		background.color = contextualColor;
	}

	public void OnClick()
	{
		if (!isReadonly)
		{
			dropdown.SelectItem(this);
		}
	}

	private string SanitizeSearchString(string str)
	{
		if (!dropdown.useStrictSearch)
		{
			str = Regex.Replace(str, "-", " ");
		}
		return str.ToLower();
	}
}
