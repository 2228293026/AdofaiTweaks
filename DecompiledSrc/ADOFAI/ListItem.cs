using ADOFAI.LevelEditor.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI;

public class ListItem : ADOBase
{
	[Header("Components")]
	[SerializeField]
	protected TMP_Text itemName;

	[SerializeField]
	protected TMP_Text itemTagText;

	public RectTransform rt;

	public Transform selectionBackground;

	public Transform selectionHighlight;

	public AdofaiEventTrigger eventTrigger;

	[Header("Buttons")]
	public Button floorButton;

	[Header("Image Components")]
	public Image itemTypeImage;

	public Image tagImage;

	public Image floorButtonImage;

	[Header("Texts")]
	public TMP_Text floorIDText;

	[HideInInspector]
	public LevelEvent sourceLevelEvent;

	protected PropertyControl_List propertyControlList;

	protected float displayedButtonColorMultiplier;

	private int floorID;

	protected virtual void Awake()
	{
		floorButton.onClick.AddListener(delegate
		{
			SelectFloorButton();
		});
		eventTrigger.onPointerClick = delegate
		{
			SelectItemButton();
		};
		eventTrigger.onPointerDown = delegate(PointerEventData e)
		{
			propertyControlList.PointerDown(e, this);
		};
		eventTrigger.onBeginDrag = delegate(PointerEventData e)
		{
			propertyControlList.BeginDrag(e, this);
		};
		eventTrigger.onPointerEnter = delegate(PointerEventData e)
		{
			propertyControlList.PointerEnter(e, this);
		};
		eventTrigger.onPointerExit = delegate(PointerEventData e)
		{
			propertyControlList.PointerExit(e, this);
		};
	}

	protected virtual void LateUpdate()
	{
	}

	public virtual void SetEvent(LevelEvent ev)
	{
		sourceLevelEvent = ev;
		floorID = ev.floor;
		ValidateFloorID();
		ShowItemTag(propertyControlList.ShowingTagsOnItems);
		bool flag = false;
		if (ev["relativeTo"] != null)
		{
			if ((DecPlacementType)ev["relativeTo"] == DecPlacementType.Tile)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		floorButtonImage.gameObject.SetActive(flag);
		floorIDText.gameObject.SetActive(flag);
		if (flag)
		{
			floorIDText.text = floorID.ToString();
		}
		SetSelectedState(IsSelected());
	}

	public virtual void OnHover(bool enteredCursor)
	{
	}

	public bool IsSelected()
	{
		return ADOBase.editor.selectedDecorations.Contains(sourceLevelEvent);
	}

	public void ShowHighlight(bool show)
	{
		selectionHighlight.gameObject.SetActive(show);
	}

	public void ShowItemTag(bool show)
	{
		itemName.enabled = !show;
		itemTypeImage.enabled = !show;
		itemTagText.enabled = show;
		tagImage.enabled = show;
	}

	public void SetSelectedState(bool selected)
	{
		ShowSelectionBackground(selected);
	}

	protected virtual void ShowSelectionBackground(bool selected)
	{
		selectionBackground.gameObject.SetActive(selected);
		itemName.color = (selected ? Color.black : Color.white);
		itemTypeImage.color = (selected ? Color.black : Color.white);
		tagImage.color = (selected ? Color.black : Color.white);
		itemTagText.color = (selected ? Color.black : Color.white);
		floorIDText.color = (selected ? Color.white : Color.black);
		floorButtonImage.color = (selected ? Color.black : Color.white);
	}

	private void ValidateFloorID()
	{
		floorID = Mathf.Clamp(floorID, 0, ADOBase.editor.floors.Count - 1);
	}

	protected virtual void SelectItemButton()
	{
	}

	private void SelectFloorButton()
	{
		ValidateFloorID();
		scrFloor floorToSelect = ADOBase.editor.floors[floorID];
		ADOBase.editor.SelectFloor(floorToSelect);
	}
}
