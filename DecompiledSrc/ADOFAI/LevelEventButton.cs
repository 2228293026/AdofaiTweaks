using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI;

public class LevelEventButton : ADOBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public Button button;

	public Image icon;

	public int page;

	public int keyCode;

	public LevelEventType type;

	public LevelEventInfo info;

	private bool showingAsFiltered;

	private Color baseColor = Color.white;

	private bool _enableButton = true;

	private bool pointInButton;

	public bool enableButton
	{
		get
		{
			return _enableButton;
		}
		set
		{
			_enableButton = value;
			baseColor = (value ? Color.white : Color.gray.WithAlpha(0.6f));
			ShowAsSelected(pointInButton);
		}
	}

	public void Init(LevelEventType type, int pageNum = 0, int keyCode = 0)
	{
		page = pageNum;
		this.type = type;
		info = GCS.levelEventsInfo[type.ToString()];
		this.keyCode = keyCode;
		icon.sprite = GCS.levelEventIcons[type];
		button.onClick.AddListener(OnClicky);
		ShowAsSelected(selected: false);
	}

	private void OnClicky()
	{
		if (enableButton)
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta))
			{
				ADOBase.editor.FilterEventType(type);
			}
			else
			{
				ADOBase.editor.AddEventAtSelected(type);
			}
			ShowAsFiltered(ADOBase.editor.filteredEventType == type);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointInButton = true;
		if (enableButton)
		{
			ShowAsSelected(selected: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointInButton = false;
		if (enableButton)
		{
			ShowAsSelected(selected: false);
		}
	}

	private void ShowAsSelected(bool selected)
	{
		icon.color = ((selected && enableButton) ? baseColor : baseColor.WithAlpha(baseColor.a * 0.6f));
		ADOBase.editor.eventPickerText.text = (selected ? RDString.Get($"editor.{type}") : "");
	}

	public void ShowAsFiltered(bool filtered)
	{
		icon.color = (baseColor = (filtered ? Color.cyan : Color.white));
		showingAsFiltered = filtered;
		if (!filtered)
		{
			ShowAsSelected(selected: false);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (enableButton && eventData.button == PointerEventData.InputButton.Right)
		{
			if (ADOBase.editor.currentCategory == LevelEventCategory.Favorites)
			{
				ADOBase.editor.RemoveFavoriteEvent(type);
				ADOBase.editor.eventPickerText.text = "";
			}
			else
			{
				ADOBase.editor.AddFavoriteEvent(type);
			}
		}
	}
}
