using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

[RequireComponent(typeof(ListItemPool))]
public class PropertyControl_List : PropertyControl
{
	[Header("PropertiesControlList")]
	public bool itemsReorderable = true;

	public static bool isDraggingItems;

	public static bool isDraggingOverItemMiddle;

	[Header("Colors")]
	public Color normalStateColor;

	public Color restrictedStateColor;

	[Header("References")]
	public RectTransform contentRT;

	public RectTransform viewportRect;

	public RectTransform draggingIndicatorBar;

	public RectTransform multiSelectIcon;

	public Button removeButton;

	public Button showTagsToggleButton;

	[NonSerialized]
	public RectTransform parentReferenceRT;

	[Header("References/Search Bar")]
	public TMP_InputField searchField;

	public TMP_Text searchFieldPlaceholder;

	public TMP_Text noItemsText;

	public Button clearSearchFieldButton;

	public SmoothScrollRect smoothScrollRect;

	public Action<LevelEvent> OnItemSelected;

	public Action OnAllItemsDeselected;

	public static string stringNoItems;

	public static string stringSearch;

	[NonSerialized]
	public int lastSelectedIndex;

	public ListItemPool listItemPool;

	protected List<ListItem> shownItems = new List<ListItem>();

	protected ListItem cachedItemOnMouse;

	protected List<LevelEvent> filteredEvents = new List<LevelEvent>();

	protected static float itemHeight;

	private const float buttonSectionHeight = 10f;

	private Vector2 cachedMousePos;

	private float itemSegmentSize;

	private int itemDistanceUnits;

	private bool mouseIsInsideContent;

	private bool showTags;

	private bool shownTagsToggle;

	protected ListItem cachedHighlightedItem;

	private float cacheContentPosY;

	private float cacheViewportSizeY;

	private RectTransform cachedDraggedItemTransform;

	private List<ListItem> toRemove = new List<ListItem>();

	private bool applyUpdateList;

	private bool shouldUpdateForce;

	private bool applyRefreshScrollRect;

	private LevelEvent cacheEventForRectAdjust;

	protected List<LevelEvent> selectedDecorations => ADOBase.editor.selectedDecorations;

	protected List<scrDecoration> allDecorations => scrDecorationManager.instance.allDecorations;

	public List<ListItem> ShownItems => shownItems;

	public ListItem CachedItemOnMouse => cachedItemOnMouse;

	protected bool holdingShift => RDInput.holdingShift;

	protected bool holdingControl => RDInput.holdingControl;

	protected bool holdingAlt => RDInput.holdingAlt;

	public bool ShowingTagsOnItems { get; private set; }

	protected virtual void Awake()
	{
		stringNoItems = RDString.Get("editor.noItems");
		stringSearch = RDString.Get("editor.search");
	}

	protected virtual void Start()
	{
		itemHeight = listItemPool.itemPrefab.GetComponent<RectTransform>().rect.height;
		clearSearchFieldButton.onClick.AddListener(delegate
		{
			searchField.text = "";
		});
		showTagsToggleButton.onClick.AddListener(delegate
		{
			shownTagsToggle = !shownTagsToggle;
		});
		searchField.onValueChanged.AddListener(delegate(string s)
		{
			FilterSearchResults(s);
		});
		searchFieldPlaceholder.text = stringSearch;
		RectTransform obj = rectTransform.parent as RectTransform;
		obj.offsetMax = obj.offsetMax.WithY(0f);
		multiSelectIcon.transform.SetParent(parentReferenceRT);
		noItemsText.text = stringNoItems;
	}

	protected virtual void Update()
	{
		showTags = holdingAlt || shownTagsToggle;
		ShowTagsOnItems(showTags);
		bool flag = !ADOBase.editor.SelectionDecorationIsEmpty();
		if (flag)
		{
			if (!ADOBase.editor.userIsEditingAnInputField)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					SelectPreviousItem();
				}
				else if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					SelectNextItem();
				}
				if (Input.GetKeyDown(KeyCode.Home))
				{
					ADOBase.editor.SelectDecoration(0);
				}
				if (Input.GetKeyDown(KeyCode.End))
				{
					ADOBase.editor.SelectDecoration(ADOBase.editor.decorations.Count - 1);
				}
			}
			if (isDraggingItems)
			{
				if (Input.GetMouseButton(0))
				{
					SetDraggedPosition(Input.mousePosition);
				}
				if (Input.GetMouseButtonUp(0))
				{
					EndDrag(Input.mousePosition);
				}
			}
		}
		removeButton.interactable = flag;
		noItemsText.gameObject.SetActive(allDecorations.Count < 1);
		clearSearchFieldButton.gameObject.SetActive(!searchField.text.IsNullOrEmpty());
	}

	protected virtual void LateUpdate()
	{
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, parentReferenceRT.rect.size.y - 10f);
		if (contentRT.anchoredPosition.y != cacheContentPosY || viewportRect.rect.size.y != cacheViewportSizeY)
		{
			cacheContentPosY = contentRT.anchoredPosition.y;
			cacheViewportSizeY = viewportRect.rect.size.y;
			RefreshItemsList();
		}
		if (applyUpdateList && listItemPool.Initialized)
		{
			ApplyUpdateList(shouldUpdateForce);
		}
		if (applyRefreshScrollRect)
		{
			AdjustItemListScrollRect(cacheEventForRectAdjust);
		}
	}

	public void PointerDown(PointerEventData eventData, ListItem itemRef)
	{
		if (!(itemRef == null))
		{
			cachedDraggedItemTransform = itemRef.rt;
			cachedMousePos = eventData.position;
			multiSelectIcon.position = cachedMousePos;
		}
	}

	public void BeginDrag(PointerEventData eventData, ListItem itemRef)
	{
		if (itemsReorderable && itemRef.IsSelected())
		{
			CacheOnStartDrag(itemRef);
			itemSegmentSize = itemHeight / 4f * ADOBase.editor.levelEditorCanvas.scaleFactor;
			isDraggingItems = true;
		}
	}

	public void SetDraggedPosition(Vector3 eventData)
	{
		if (!itemsReorderable)
		{
			return;
		}
		Vector3 position = eventData;
		float x = viewportRect.transform.position.x;
		float num = (viewportRect.rect.size.x + x) * ADOBase.editor.levelEditorCanvas.scaleFactor;
		float y = viewportRect.transform.position.y;
		float num2 = y - viewportRect.rect.size.y * ADOBase.editor.levelEditorCanvas.scaleFactor;
		mouseIsInsideContent = position.x >= x && position.x <= num && position.y >= num2 && position.y <= y;
		bool active = mouseIsInsideContent && isDraggingItems;
		multiSelectIcon.gameObject.SetActive(active);
		multiSelectIcon.position = position;
		Vector3 position2 = cachedDraggedItemTransform.position;
		float num3 = position.y - position2.y;
		float num4 = position.y - y;
		int num5 = 1000000;
		if (Mathf.Abs(num3) > (float)num5 || num4 > (float)num5)
		{
			num3 = 0f;
			num4 = 0f;
		}
		int num6 = Mathf.RoundToInt(num3 / itemSegmentSize);
		int value = Mathf.RoundToInt(num4 / itemSegmentSize);
		int num7 = Math.Abs(num6);
		int num8 = Math.Abs(value);
		if (num7 % 4 == 0)
		{
			isDraggingOverItemMiddle = false;
			if (cachedHighlightedItem != null)
			{
				cachedHighlightedItem.ShowHighlight(show: false);
			}
			draggingIndicatorBar.gameObject.SetActive(active);
			draggingIndicatorBar.position = new Vector2(draggingIndicatorBar.position.x, position2.y + itemSegmentSize * (float)num6);
			if (num8 / 4 > shownItems.Count)
			{
				draggingIndicatorBar.gameObject.SetActive(value: false);
			}
		}
		else
		{
			isDraggingOverItemMiddle = true;
			draggingIndicatorBar.gameObject.SetActive(value: false);
		}
	}

	public void EndDrag(Vector3 eventData)
	{
		if (!itemsReorderable)
		{
			return;
		}
		multiSelectIcon.gameObject.SetActive(value: false);
		draggingIndicatorBar.gameObject.SetActive(value: false);
		using (new SaveStateScope(ADOBase.editor))
		{
			if (mouseIsInsideContent)
			{
				if (isDraggingOverItemMiddle)
				{
					OnItemDropMiddle();
				}
				else
				{
					OnItemDropSides();
				}
			}
			ClearDragCache();
		}
	}

	protected virtual void CacheOnStartDrag(ListItem itemRef)
	{
	}

	protected virtual void OnItemDropMiddle()
	{
	}

	protected virtual void OnItemDropSides()
	{
	}

	public void PointerEnter(PointerEventData eventData, ListItem item)
	{
		if (!isDraggingItems)
		{
			item.OnHover(enteredCursor: true);
		}
		if (cachedHighlightedItem != null)
		{
			cachedHighlightedItem.ShowHighlight(show: false);
		}
		if (isDraggingItems && !selectedDecorations.Contains(item.sourceLevelEvent))
		{
			cachedHighlightedItem = item;
		}
		else
		{
			scrDecorationManager.instance.ShowHoverBorders(item.sourceLevelEvent);
		}
		cachedItemOnMouse = item;
	}

	public void PointerExit(PointerEventData eventData, ListItem item)
	{
		item.OnHover(enteredCursor: false);
		item.ShowHighlight(show: false);
		scrDecorationManager.instance.ShowHoverBorders(item.sourceLevelEvent, show: false);
		cachedItemOnMouse = null;
	}

	public virtual void ClearDragCache()
	{
		cachedHighlightedItem = null;
		isDraggingItems = false;
	}

	public ListItem SearchForVisibleItem(LevelEvent levelEvent)
	{
		foreach (ListItem shownItem in shownItems)
		{
			if (shownItem.sourceLevelEvent == levelEvent)
			{
				return shownItem;
			}
		}
		return null;
	}

	public void SelectItemsInRange(LevelEvent endRangeItem)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			int decorationIndex = scrDecorationManager.GetDecorationIndex(endRangeItem);
			if (decorationIndex == lastSelectedIndex && selectedDecorations.Count == 1)
			{
				ADOBase.editor.SelectDecoration(endRangeItem, jumpToDecoration: false, showPanel: false);
				return;
			}
			int num = decorationIndex;
			bool num2 = num >= lastSelectedIndex;
			int num3 = (num2 ? lastSelectedIndex : num);
			int num4 = (num2 ? num : lastSelectedIndex);
			for (int i = num3; i <= num4; i++)
			{
				if (filteredEvents.Contains(scrDecorationManager.GetDecoration(i).sourceLevelEvent))
				{
					ADOBase.editor.SelectDecoration(i, jumpToDecoration: false, showPanel: false, ignoreDeselection: true, ignoreAdjustRect: true);
				}
			}
			scnEditor.instance.levelEventsPanel.ShowInspector(show: true, forceAction: true);
			scnEditor.instance.levelEventsPanel.ShowPanel(endRangeItem.eventType);
		}
	}

	public void RefreshItemsList(bool forceRefreshAll = false)
	{
		applyUpdateList = true;
		shouldUpdateForce = forceRefreshAll;
	}

	public void RefreshScrollRectPosition(LevelEvent levelEvent)
	{
		applyRefreshScrollRect = true;
		cacheEventForRectAdjust = levelEvent;
	}

	public void ClearCache()
	{
		ClearDragCache();
		filteredEvents.Clear();
		shownItems.Clear();
	}

	protected virtual void FilterSearchResults(string search, bool adjustRect = true)
	{
		ClearShownItems();
		filteredEvents.Clear();
	}

	private void AdjustItemListScrollRect(LevelEvent levelEvent)
	{
		if (levelEvent == null)
		{
			return;
		}
		FilterSearchResults(searchField.text, adjustRect: false);
		int num = 0;
		using (List<LevelEvent>.Enumerator enumerator = filteredEvents.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != levelEvent)
			{
				num++;
			}
		}
		AdjustItemListScrollRect(num);
	}

	private void AdjustItemListScrollRect(int decorationIndex)
	{
		contentRT.SizeDeltaY(itemHeight * (float)filteredEvents.Count);
		if (!IsItemFullyVisible(decorationIndex))
		{
			float y = contentRT.anchoredPosition.y;
			float num = (float)decorationIndex * itemHeight * -1f;
			float height = viewportRect.rect.height;
			float num2 = ((y + num > 0f) ? num : (num - itemHeight + height));
			smoothScrollRect.ScrollTo(0f - num2);
		}
		RefreshItemsList();
		applyRefreshScrollRect = false;
		cacheEventForRectAdjust = null;
	}

	private void ApplyUpdateList(bool forceRefreshAll = false)
	{
		if (filteredEvents.Count <= 0)
		{
			return;
		}
		if (forceRefreshAll)
		{
			ClearShownItems();
		}
		else
		{
			toRemove.Clear();
			foreach (ListItem shownItem in shownItems)
			{
				if (!IsItemVisible(shownItem.rt))
				{
					toRemove.Add(shownItem);
					listItemPool.SendItemBackToPool(shownItem.gameObject);
				}
			}
			for (int i = 0; i < toRemove.Count; i++)
			{
				shownItems.Remove(toRemove[i]);
			}
		}
		contentRT.SizeDeltaY(itemHeight * (float)filteredEvents.Count);
		int num = Math.Max(0, Mathf.RoundToInt(contentRT.anchoredPosition.y / itemHeight) - 1);
		int num2 = Mathf.RoundToInt(viewportRect.rect.height / itemHeight) + 2;
		for (int j = num; j < num + num2 && j < filteredEvents.Count; j++)
		{
			if (!IsItemVisible(j) || (!ADOBase.editor.decorations.Contains(filteredEvents[j]) && !ADOBase.editor.events.Contains(filteredEvents[j])))
			{
				continue;
			}
			ListItem listItem = SearchForVisibleItem(filteredEvents[j]);
			Vector3 vector = new Vector3(0f, (float)j * itemHeight * -1f, 0f);
			if (listItem == null)
			{
				ListItem component = listItemPool.GetPooledItem(contentRT, vector).GetComponent<ListItem>();
				component.SetEvent(filteredEvents[j]);
				shownItems.Add(component);
				continue;
			}
			bool selectedState;
			if (filteredEvents[j].IsDecoration)
			{
				selectedState = ADOBase.editor.selectedDecorations.Contains(filteredEvents[j]);
			}
			else
			{
				selectedState = false;
				foreach (scrFloor selectedFloor in ADOBase.editor.selectedFloors)
				{
					if (filteredEvents[j].floor == selectedFloor.seqID)
					{
						selectedState = true;
					}
				}
			}
			listItem.SetSelectedState(selectedState);
			listItem.rt.anchoredPosition = vector;
		}
		applyUpdateList = false;
		shouldUpdateForce = false;
	}

	private bool IsItemVisible(int itemIndex)
	{
		float y = contentRT.anchoredPosition.y;
		float num = (float)itemIndex * itemHeight * -1f + y;
		float height = viewportRect.rect.height;
		if (num < itemHeight)
		{
			return num > 0f - height;
		}
		return false;
	}

	private bool IsItemVisible(RectTransform itemRT)
	{
		float y = contentRT.anchoredPosition.y;
		float num = itemRT.anchoredPosition.y + y;
		float height = viewportRect.rect.height;
		if (num < itemHeight)
		{
			return num > 0f - height;
		}
		return false;
	}

	private bool IsItemFullyVisible(int itemIndex)
	{
		float y = contentRT.anchoredPosition.y;
		float num = (float)itemIndex * itemHeight * -1f + y;
		float height = viewportRect.rect.height;
		if (num <= 0f)
		{
			return num - itemHeight > 0f - height;
		}
		return false;
	}

	private void SelectPreviousItem()
	{
		if (filteredEvents.Count >= 1)
		{
			int num = filteredEvents.IndexOf(ADOBase.editor.decorations[lastSelectedIndex]);
			num--;
			if (num < 0)
			{
				num = filteredEvents.Count - 1;
			}
			num %= filteredEvents.Count;
			LevelEvent levelEvent = filteredEvents[num];
			ADOBase.editor.SelectDecoration(levelEvent, jumpToDecoration: true, showPanel: true, holdingControl);
		}
	}

	private void SelectNextItem()
	{
		if (filteredEvents.Count >= 1)
		{
			int num = filteredEvents.IndexOf(ADOBase.editor.decorations[lastSelectedIndex]);
			num = (num + 1) % filteredEvents.Count;
			LevelEvent levelEvent = filteredEvents[num];
			ADOBase.editor.SelectDecoration(levelEvent, jumpToDecoration: true, showPanel: true, holdingControl);
		}
	}

	private void ClearShownItems()
	{
		while (contentRT.childCount > 0)
		{
			listItemPool.SendItemBackToPool(contentRT.GetChild(0).gameObject);
		}
		shownItems.Clear();
	}

	private void ShowTagsOnItems(bool show)
	{
		if (showTags == ShowingTagsOnItems)
		{
			return;
		}
		foreach (ListItem shownItem in shownItems)
		{
			shownItem.ShowItemTag(show);
		}
		ShowingTagsOnItems = show;
		showTagsToggleButton.image.color = (show ? restrictedStateColor : normalStateColor);
	}
}
