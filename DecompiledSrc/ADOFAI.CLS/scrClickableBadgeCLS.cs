using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ADOFAI.CLS;

public class scrClickableBadgeCLS : ADOBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public TMP_Text text;

	public Color defaultColor;

	public Color clickableColor;

	public Color hoveredColor;

	public bool clickable;

	public string url;

	public bool hovered { get; set; }

	private void Awake()
	{
		text.SetLocalizedFont();
	}

	private void Update()
	{
		if (!clickable)
		{
			text.color = defaultColor;
			text.fontStyle = FontStyles.Normal;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovered = true;
		UpdateHoverState();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovered = false;
		UpdateHoverState();
	}

	public void UpdateHoverState()
	{
		if (!clickable)
		{
			text.color = defaultColor;
			text.fontStyle = FontStyles.Normal;
		}
		else
		{
			text.color = (hovered ? hoveredColor : clickableColor);
			text.fontStyle = (hovered ? FontStyles.Underline : FontStyles.Normal);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (clickable && !string.IsNullOrEmpty(url))
		{
			ADOBase.platformHelper.OpenURL(url);
			hovered = false;
			UpdateHoverState();
		}
	}
}
