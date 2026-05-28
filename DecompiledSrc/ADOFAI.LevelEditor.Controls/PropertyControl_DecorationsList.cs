using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_DecorationsList : PropertyControl_List
{
	public static string stringItemNoText;

	public static string stringItemNoImage;

	public static string stringItemNoTag;

	private const string FilterFloorKey = "floor:";

	private const string FilterTypeKey = "type:";

	private const string FilterTagKey = "tag:";

	private string[] filterKeys = new string[3] { "floor:", "tag:", "type:" };

	public Button toggleHideAllButton;

	public Image toggleHideAllIndicator;

	public Button toggleLockAllButton;

	public Image toggleLockAllIndicator;

	[Header("References/Sprites")]
	public Sprite eyeOpenSprite;

	public Sprite eyeClosedSprite;

	public Sprite lockOpenSprite;

	public Sprite lockClosedSprite;

	private bool applyOnDecorationUpdate;

	private bool forceHideAll;

	private bool forceLockAll;

	private List<scrDecoration> cachedDecorations = new List<scrDecoration>();

	public RectTransform buttonsContainer;

	public RectTransform buttonTemplate;

	public LevelEventType[] decorationButtonTypes;

	protected override void Awake()
	{
		base.Awake();
		stringItemNoText = string.Format("<i>({0})</i>", RDString.Get("editor.noText").ToLower());
		stringItemNoImage = string.Format("<i>({0})</i>", RDString.Get("editor.noImage").ToLower());
		stringItemNoTag = string.Format("<i>({0})</i>", RDString.Get("editor.noTag").ToLower());
		LevelEventType[] array = decorationButtonTypes;
		foreach (LevelEventType type in array)
		{
			RectTransform obj = UnityEngine.Object.Instantiate(buttonTemplate, buttonsContainer);
			obj.gameObject.SetActive(value: true);
			obj.GetComponent<Image>().sprite = GCS.levelEventIcons[type];
			obj.GetComponent<Button>().onClick.AddListener(delegate
			{
				using (new SaveStateScope(ADOBase.editor))
				{
					ADOBase.editor.DeselectAllDecorations();
					LevelEvent levelEvent = ADOBase.editor.AddDecoration(type);
					ADOBase.editor.SelectDecoration(levelEvent);
				}
			});
		}
	}

	protected override void Start()
	{
		contentRT = ADOBase.editor.decorationsListContent;
		base.Start();
		removeButton.onClick.AddListener(delegate
		{
			ADOBase.editor.DeleteMultiSelectionDecorations();
		});
		toggleHideAllButton.onClick.AddListener(delegate
		{
			ToggleHideAll();
		});
		toggleLockAllButton.onClick.AddListener(delegate
		{
			ToggleLockAll();
		});
		listItemPool.Initialize();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		toggleHideAllIndicator.sprite = (forceHideAll ? eyeClosedSprite : eyeOpenSprite);
		toggleHideAllIndicator.color = (forceHideAll ? restrictedStateColor : normalStateColor);
		toggleLockAllIndicator.sprite = (forceLockAll ? lockClosedSprite : lockOpenSprite);
		toggleLockAllIndicator.color = (forceLockAll ? restrictedStateColor : normalStateColor);
		if (applyOnDecorationUpdate)
		{
			ApplyOnDecorationUpdate();
		}
	}

	public void OnDecorationUpdate()
	{
		applyOnDecorationUpdate = true;
	}

	protected override void FilterSearchResults(string search, bool adjustRect = true)
	{
		base.FilterSearchResults(search, adjustRect);
		bool flag = false;
		if (string.IsNullOrEmpty(search))
		{
			foreach (LevelEvent decoration2 in ADOBase.editor.decorations)
			{
				filteredEvents.Add(decoration2);
			}
		}
		else
		{
			string text = search.ToLower();
			string[] array = filterKeys;
			foreach (string text2 in array)
			{
				if (!text.Contains(text2))
				{
					continue;
				}
				flag = true;
				switch (text2)
				{
				case "floor:":
				{
					string b = text.Substring("floor:".Length);
					if (!int.TryParse(b, out var result))
					{
						break;
					}
					foreach (LevelEvent decoration3 in ADOBase.editor.decorations)
					{
						if (result == decoration3.floor)
						{
							filteredEvents.Add(decoration3);
						}
					}
					break;
				}
				case "tag:":
				{
					string b = text.Substring("tag:".Length);
					foreach (LevelEvent decoration4 in ADOBase.editor.decorations)
					{
						string text3 = (string)decoration4["tag"];
						if (string.IsNullOrEmpty(text3))
						{
							text3 = "";
						}
						if (string.Equals(text3.ToLower(), b))
						{
							filteredEvents.Add(decoration4);
						}
					}
					break;
				}
				case "type:":
					switch (text.Substring("type:".Length))
					{
					case "image":
						foreach (LevelEvent decoration5 in ADOBase.editor.decorations)
						{
							if (decoration5.eventType == LevelEventType.AddDecoration)
							{
								filteredEvents.Add(decoration5);
							}
						}
						break;
					case "text":
						foreach (LevelEvent decoration6 in ADOBase.editor.decorations)
						{
							if (decoration6.eventType == LevelEventType.AddText)
							{
								filteredEvents.Add(decoration6);
							}
						}
						break;
					case "particle":
						foreach (LevelEvent decoration7 in ADOBase.editor.decorations)
						{
							if (decoration7.eventType == LevelEventType.AddParticle)
							{
								filteredEvents.Add(decoration7);
							}
						}
						break;
					}
					break;
				}
			}
			if (!flag)
			{
				foreach (LevelEvent decoration8 in ADOBase.editor.decorations)
				{
					scrDecoration decoration = scrDecorationManager.GetDecoration(decoration8);
					if (decoration != null)
					{
						string text4 = decoration.decorationName.ToLower();
						string decorationTag = decoration.decorationTag;
						if (decorationTag.IsNullOrEmpty())
						{
							decorationTag = stringItemNoTag;
						}
						string text5 = decorationTag.ToLower();
						bool num = decoration.transform.name == "";
						bool flag2 = text5 == stringItemNoTag;
						if ((!num && text4.Contains(text)) || (!flag2 && text5.Contains(text)))
						{
							filteredEvents.Add(decoration8);
						}
					}
				}
			}
		}
		if (adjustRect && filteredEvents.Count > 0)
		{
			RefreshScrollRectPosition(filteredEvents[0]);
		}
		LevelEvent hoveredDecoration = scrDecorationManager.instance.hoveredDecoration;
		if (hoveredDecoration != null)
		{
			scrDecorationManager.instance.ShowHoverBorders(hoveredDecoration, show: false);
		}
	}

	protected override void CacheOnStartDrag(ListItem itemRef)
	{
		base.CacheOnStartDrag(itemRef);
		if (base.selectedDecorations.Count <= 0 || !base.selectedDecorations.Contains(itemRef.sourceLevelEvent))
		{
			return;
		}
		foreach (LevelEvent selectedDecoration in base.selectedDecorations)
		{
			cachedDecorations.Add(scrDecorationManager.GetDecoration(selectedDecoration));
		}
	}

	protected override void OnItemDropMiddle()
	{
		base.OnItemDropMiddle();
		OnDecorationUpdate();
	}

	protected override void OnItemDropSides()
	{
		base.OnItemDropSides();
		if (!(cachedHighlightedItem != null))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		scrDecoration decoration = scrDecorationManager.GetDecoration(cachedHighlightedItem.sourceLevelEvent);
		bool flag = cachedDecorations.Contains(decoration);
		if (flag)
		{
			num2 = scrDecorationManager.GetDecorationIndex(cachedHighlightedItem.sourceLevelEvent);
		}
		else
		{
			Vector3 position = cachedHighlightedItem.rt.position;
			num = ((!Mathf.Approximately(draggingIndicatorBar.position.y, position.y)) ? 1 : 0);
		}
		cachedDecorations = cachedDecorations.OrderBy((scrDecoration x) => scrDecorationManager.GetDecorationIndex(x.sourceLevelEvent)).ToList();
		foreach (scrDecoration cachedDecoration in cachedDecorations)
		{
			cachedDecoration.transform.SetParent(null);
			base.allDecorations.Remove(cachedDecoration);
			ADOBase.editor.levelData.decorations.Remove(cachedDecoration.sourceLevelEvent);
		}
		if (!flag)
		{
			num2 = scrDecorationManager.GetDecorationIndex(cachedHighlightedItem.sourceLevelEvent) + num;
		}
		num2 = Math.Clamp(num2, 0, base.allDecorations.Count);
		int count = cachedDecorations.Count;
		for (int num3 = 0; num3 < count; num3++)
		{
			cachedDecorations[num3].transform.SetParent(scrDecorationManager.instance.transform);
			cachedDecorations[num3].transform.SetSiblingIndex(num2);
			base.allDecorations.Insert(num2, cachedDecorations[num3]);
			ADOBase.editor.levelData.decorations.Insert(num2, cachedDecorations[num3].sourceLevelEvent);
			lastSelectedIndex = num2;
			num2++;
		}
		OnDecorationUpdate();
	}

	public override void ClearDragCache()
	{
		base.ClearDragCache();
		cachedDecorations.Clear();
	}

	private void ApplyOnDecorationUpdate()
	{
		if (searchField != null)
		{
			FilterSearchResults(searchField.text, adjustRect: false);
		}
		ApplyHideAll();
		RefreshItemsList(forceRefreshAll: true);
		applyOnDecorationUpdate = false;
	}

	private void ToggleHideAll()
	{
		forceHideAll = !forceHideAll;
		ApplyHideAll();
	}

	private void ApplyHideAll()
	{
		foreach (scrDecoration allDecoration in base.allDecorations)
		{
			ADOBase.editor.ForceHideEvent(allDecoration, forceHideAll);
		}
	}

	private void ToggleLockAll()
	{
		forceLockAll = !forceLockAll;
		ApplyLockAll();
	}

	private void ApplyLockAll()
	{
		foreach (scrDecoration allDecoration in base.allDecorations)
		{
			ADOBase.editor.ForceLockEvent(allDecoration, forceLockAll);
		}
	}
}
