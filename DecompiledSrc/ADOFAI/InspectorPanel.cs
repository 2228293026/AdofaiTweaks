using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI.Editor;
using ADOFAI.LevelEditor.Controls;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI;

public class InspectorPanel : ADOBase
{
	public static readonly Color selectionColor = new Color(39f / 85f, 47f / 51f, 1f, 1f);

	private static LevelEventType cacheSelectedEventType = LevelEventType.None;

	public const float tabHeight = 68f;

	private const float artistPrefabHeight = 35f;

	private const float artistPopupPaddingHeight = 12f;

	private const string addArtistKey = "editor.addArtist";

	[Header("UI")]
	public RectTransform rect;

	public TMP_Text title;

	public RectTransform panels;

	public RectTransform tabs;

	public Button deleteEventButton;

	public Button disableEventButton;

	public Image disableEventButtonImage;

	public GameObject titleCanvas;

	public GameObject messageCanvas;

	public TMP_Text messageText;

	public bool showInspector;

	public bool permaHideInspector;

	public RectTransform artistPopup;

	public RectTransform artistContainer;

	public GameObject artistPrefab;

	public GameObject addArtist;

	public Text addArtistText;

	public ArtistUIDisclaimer artistUIDisclaimer;

	public float artistRectMaxHeight;

	public GameObject approvalLevelPrefab;

	[HideInInspector]
	public ApprovalLevelBadge approvalLevelBadge;

	public Sprite visibleIcon;

	public Sprite hiddenIcon;

	public Sprite powerIcon;

	public RectTransform subTabButtons;

	public RectTransform subTabGroupTemplate;

	public PropertiesSubTabButton subTabButtonTemplate;

	[NonSerialized]
	[Header("Runtime")]
	public List<PropertiesPanel> panelsList;

	[NonSerialized]
	public LevelEventType selectedEventType;

	[NonSerialized]
	public LevelEvent selectedEvent;

	[NonSerialized]
	public int cacheEventIndex;

	private Tween inspectorTween;

	[NonSerialized]
	private Tween unlockKeyLimiterButtonTween;

	public bool floorPanel;

	private ArtistData currentArtist;

	private string lastLevelPath;

	private bool showingPanel;

	[HideInInspector]
	public Action currentArtistDisclaimerAction;

	public EditorWebServices editorWebServices => ADOBase.editor.webServices;

	public void Init(Dictionary<string, LevelEventInfo> levelEventsInfo, bool floorPanel)
	{
		this.floorPanel = floorPanel;
		panelsList = new List<PropertiesPanel>();
		int num = 0;
		foreach (string key in levelEventsInfo.Keys)
		{
			LevelEventType levelEventType = RDUtils.ParseEnum(key, LevelEventType.None);
			if (levelEventType == LevelEventType.EventSettings)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_propertiesPanel);
			gameObject.transform.SetParent(panels, worldPositionStays: false);
			gameObject.name = key;
			PropertiesPanel panel = gameObject.GetComponent<PropertiesPanel>();
			panel.levelEventType = levelEventType;
			if (levelEventType == LevelEventType.EditorComment)
			{
				panelsList.Insert(0, panel);
			}
			else
			{
				panelsList.Add(panel);
			}
			panel.gameObject.SetActive(value: false);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_tab);
			gameObject2.transform.SetParent(tabs, worldPositionStays: false);
			if (levelEventType == LevelEventType.EditorComment)
			{
				gameObject2.transform.SetAsFirstSibling();
			}
			if (levelEventType == LevelEventType.Bookmark)
			{
				gameObject2.transform.SetAsLastSibling();
			}
			InspectorTab component = gameObject2.GetComponent<InspectorTab>();
			component.Init(levelEventType, this);
			component.GetComponent<RectTransform>().AnchorPosY(-68f * (float)num);
			component.SetSelected(selected: false);
			LevelEventInfo levelEventInfo = levelEventsInfo[key];
			panel.Init(this, levelEventInfo);
			if (levelEventInfo.useGroups)
			{
				subTabButtonTemplate.gameObject.SetActive(value: false);
				RectTransform rectTransform = UnityEngine.Object.Instantiate(subTabGroupTemplate, subTabButtons);
				rectTransform.gameObject.name = levelEventInfo.name;
				panel.tabContainer = rectTransform;
				foreach (LevelEventInfo.Group group in levelEventInfo.groups)
				{
					PropertiesSubTabButton propertiesSubTabButton = UnityEngine.Object.Instantiate(subTabButtonTemplate, rectTransform);
					propertiesSubTabButton.gameObject.name = group.name;
					propertiesSubTabButton.SetIcon(Resources.Load<Sprite>("LevelEditor/GroupIcons/" + group.icon));
					propertiesSubTabButton.gameObject.SetActive(value: true);
					propertiesSubTabButton.groupName = group.name;
					propertiesSubTabButton.onClick.AddListener(delegate
					{
						panel.SelectTab(group.name);
					});
					panel.tabButtons.Add(group.name, propertiesSubTabButton);
				}
			}
			num++;
		}
		deleteEventButton?.onClick.AddListener(delegate
		{
			ADOBase.editor.RemoveEventAtSelected(selectedEventType);
		});
		disableEventButton?.onClick.AddListener(delegate
		{
			if (selectedEvent != null)
			{
				if (selectedEvent.IsDecoration)
				{
					ADOBase.editor.ShowEvent(selectedEvent, !selectedEvent.visible);
				}
				else
				{
					ADOBase.editor.EnableEvent(selectedEvent, !selectedEvent.active);
				}
			}
		});
	}

	private void Awake()
	{
		if ((bool)subTabButtonTemplate)
		{
			subTabGroupTemplate.gameObject.SetActive(value: false);
		}
	}

	public void Update()
	{
		LevelEvent levelEvent = selectedEvent;
		Sprite sprite = ((levelEvent == null || !levelEvent.IsDecoration) ? powerIcon : (selectedEvent.visible ? visibleIcon : hiddenIcon));
		if (disableEventButtonImage != null)
		{
			disableEventButtonImage.sprite = sprite;
		}
	}

	public void ShowPanel(LevelEventType eventType, int eventIndex = 0)
	{
		showingPanel = true;
		if (ADOBase.editor.cacheSelectedEventIndex > 0)
		{
			eventIndex = ADOBase.editor.cacheSelectedEventIndex;
		}
		else
		{
			cacheEventIndex = eventIndex;
		}
		if (eventType != LevelEventType.None)
		{
			cacheSelectedEventType = eventType;
		}
		using (new SaveStateScope(ADOBase.editor, clearRedo: false, dataHasChanged: false))
		{
			PropertiesPanel propertiesPanel = null;
			foreach (PropertiesPanel panels in panelsList)
			{
				if (panels.levelEventType == eventType)
				{
					panels.gameObject.SetActive(value: true);
					if ((bool)panels.tabContainer)
					{
						panels.tabContainer.gameObject.SetActive(value: true);
						PropertiesSubTabButton propertiesSubTabButton = panels.tabButtons.Values.First();
						panels.SelectTab(propertiesSubTabButton.groupName);
					}
					titleCanvas.SetActive(value: true);
					propertiesPanel = panels;
				}
				else
				{
					panels.gameObject.SetActive(value: false);
					if ((bool)panels.tabContainer)
					{
						panels.tabContainer.gameObject.SetActive(value: false);
					}
				}
			}
			if (eventType != LevelEventType.None)
			{
				title.text = RDString.Get("editor." + eventType);
				LevelEvent levelEvent = null;
				int num = 1;
				switch (eventType)
				{
				case LevelEventType.SongSettings:
					levelEvent = ADOBase.editor.levelData.songSettings;
					break;
				case LevelEventType.LevelSettings:
					levelEvent = ADOBase.editor.levelData.levelSettings;
					break;
				case LevelEventType.TrackSettings:
					levelEvent = ADOBase.editor.levelData.trackSettings;
					break;
				case LevelEventType.BackgroundSettings:
					levelEvent = ADOBase.editor.levelData.backgroundSettings;
					break;
				case LevelEventType.CameraSettings:
					levelEvent = ADOBase.editor.levelData.cameraSettings;
					break;
				case LevelEventType.MiscSettings:
					levelEvent = ADOBase.editor.levelData.miscSettings;
					break;
				case LevelEventType.EventSettings:
					levelEvent = ADOBase.editor.levelData.eventSettings;
					break;
				case LevelEventType.DecorationSettings:
					levelEvent = ADOBase.editor.levelData.decorationSettings;
					break;
				default:
					if (GCS.levelEventsInfo[eventType.ToString()].isDecoration)
					{
						if (ADOBase.editor.selectedDecorations.Count == 1)
						{
							levelEvent = ADOBase.editor.selectedDecorations[0];
							ModifyMessageText("", enable: false);
						}
						else if (ADOBase.editor.selectedDecorations.Count > 1)
						{
							LevelEvent levelEvent2 = new LevelEvent(-1, ADOBase.editor.selectedDecorations[0].eventType)
							{
								isFake = true
							};
							bool flag = true;
							foreach (LevelEvent selectedDecoration in ADOBase.editor.selectedDecorations)
							{
								if (selectedDecoration.eventType != levelEvent2.eventType)
								{
									eventType = LevelEventType.None;
									titleCanvas.SetActive(value: false);
									ModifyMessageText(RDString.Get("editor.dialog.differentTypeDecorationSelected"), 0f, enable: true);
									HideAllInspectorTabs();
									break;
								}
								levelEvent2.realEvents.Add(selectedDecoration);
								bool flag2 = false;
								foreach (string item in selectedDecoration.GetData().Keys.Concat(new string[1] { "floor" }))
								{
									bool flag3 = item == "floor";
									object d = (flag3 ? ((object)levelEvent2.floor) : levelEvent2[item]);
									object d2 = (flag3 ? ((object)selectedDecoration.floor) : selectedDecoration[item]);
									PropertyInfo propertyInfo = selectedDecoration.info.propertiesInfo[item];
									if (flag || (EventPropertyEquals(propertyInfo, d, d2) && !levelEvent2.disabled[item]))
									{
										levelEvent2.disabled[item] = false;
										if (propertyInfo.type == PropertyType.Vector2)
										{
											Vector2 obj = (Vector2)levelEvent2[item];
											Vector2 vector = (Vector2)selectedDecoration[item];
											bool flag4 = Mathf.Abs(obj.x - vector.x) < 0.0001f || flag;
											bool flag5 = Mathf.Abs(obj.y - vector.y) < 0.0001f || flag;
											levelEvent2[item] = new Vector2(flag4 ? vector.x : float.NaN, flag5 ? vector.y : float.NaN);
										}
										else if (flag3)
										{
											levelEvent2.floor = selectedDecoration.floor;
										}
										else
										{
											levelEvent2[item] = selectedDecoration[item];
										}
										flag2 = true;
									}
									else
									{
										levelEvent2.disabled[item] = true;
									}
								}
								if (!flag2)
								{
									break;
								}
								flag = false;
							}
							if (eventType != LevelEventType.None)
							{
								levelEvent = levelEvent2;
								ModifyMessageText("", enable: false);
							}
						}
						foreach (RectTransform tab in ADOBase.editor.levelEventsPanel.tabs)
						{
							InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
							if (!(component == null))
							{
								if (levelEvent != null && levelEvent.eventType == component.levelEventType)
								{
									component.gameObject.SetActive(value: true);
									component.GetComponent<RectTransform>().SetAnchorPosY(0f);
								}
								else
								{
									component.gameObject.SetActive(value: false);
								}
							}
						}
					}
					else
					{
						ModifyMessageText("", enable: false);
						List<LevelEvent> selectedFloorEvents = ADOBase.editor.GetSelectedFloorEvents(eventType);
						num = selectedFloorEvents.Count;
						if (eventIndex <= selectedFloorEvents.Count - 1)
						{
							levelEvent = selectedFloorEvents[eventIndex];
						}
					}
					break;
				}
				if (!(propertiesPanel == null) && levelEvent != null)
				{
					selectedEvent = levelEvent;
					selectedEventType = levelEvent.eventType;
					if (selectedEventType == LevelEventType.KillPlayer)
					{
						ModifyMessageText(RDString.Get("editor.dialog.usingKillPlayer"), -35f, enable: true);
					}
					propertiesPanel.SetProperties(levelEvent);
					foreach (RectTransform tab2 in tabs)
					{
						InspectorTab component2 = tab2.gameObject.GetComponent<InspectorTab>();
						if (component2 == null)
						{
							continue;
						}
						if (eventType == component2.levelEventType)
						{
							component2.SetSelected(selected: true);
							component2.eventIndex = eventIndex;
							if (component2.cycleButtons != null)
							{
								component2.cycleButtons.text.text = $"{eventIndex + 1}/{num}";
							}
						}
						else
						{
							component2.SetSelected(selected: false);
						}
					}
				}
			}
			else
			{
				selectedEventType = LevelEventType.None;
			}
			showingPanel = false;
		}
	}

	public void ShowPanelOfEvent(LevelEvent evnt)
	{
		int seqID = ADOBase.editor.selectedFloors[0].seqID;
		int num = 0;
		foreach (LevelEvent @event in ADOBase.editor.events)
		{
			if (seqID == @event.floor && @event.eventType == evnt.eventType)
			{
				if (@event == evnt)
				{
					break;
				}
				num++;
			}
		}
		ShowPanel(evnt.eventType, num);
	}

	public InspectorTab GetTabForEventType(LevelEventType eventType)
	{
		foreach (Transform tab in tabs)
		{
			InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
			if (component.levelEventType == eventType)
			{
				return component;
			}
		}
		return null;
	}

	public int EventNumOfTab(LevelEventType eventType)
	{
		InspectorTab tabForEventType = GetTabForEventType(eventType);
		if (!(tabForEventType == null))
		{
			return tabForEventType.eventIndex;
		}
		return 0;
	}

	public InspectorTab GetSelectedEventTab()
	{
		if (selectedEventType != LevelEventType.None)
		{
			return GetTabForEventType(selectedEventType);
		}
		return null;
	}

	public void CycleSelectedEventTab(bool next)
	{
		InspectorTab selectedEventTab = GetSelectedEventTab();
		if (selectedEventTab != null && selectedEventTab.cycleButtons != null)
		{
			selectedEventTab.cycleButtons.CycleEvent(next);
		}
	}

	public void CycleTabs(bool selectNext)
	{
		if (selectedEventType == LevelEventType.None)
		{
			return;
		}
		InspectorTab[] componentsInChildren = tabs.GetComponentsInChildren<InspectorTab>(includeInactive: false);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].levelEventType == selectedEventType)
			{
				int num = (selectNext ? 1 : (-1));
				int num2 = (i + num).FlooredModulo(componentsInChildren.Length);
				ShowPanel(componentsInChildren[num2].levelEventType);
				break;
			}
		}
	}

	public void HideAllInspectorTabs()
	{
		foreach (Transform tab in tabs)
		{
			tab.gameObject.SetActive(value: false);
		}
		titleCanvas.SetActive(value: false);
		if (ADOBase.editor.selectedFloors.Count > 0)
		{
			ModifyMessageText(RDString.Get("editor.dialog.noEventsOnTile"), enable: true);
		}
		else if (ADOBase.editor.selectedDecorations.Count == 0)
		{
			ModifyMessageText(RDString.Get("editor.dialog.noSelectedDecoration"), enable: true);
		}
		ShowPanel(LevelEventType.None);
	}

	public void ShowTabsForFloor(int floorID)
	{
		List<LevelEventType> list = new List<LevelEventType>();
		EventsArray<LevelEvent> events = scnEditor.instance.events;
		_ = scnEditor.instance.decorations;
		foreach (LevelEvent item2 in events)
		{
			if (item2.floor == floorID)
			{
				list.Add(item2.eventType);
			}
		}
		titleCanvas.SetActive(list.Count > 0);
		ModifyMessageText("", enable: false);
		if (list.Count == 0)
		{
			ShowPanel(LevelEventType.None);
			ModifyMessageText(RDString.Get("editor.dialog.noEventsOnTile"), 0f, enable: true);
			ADOBase.editor.DeselectAllDecorations();
		}
		else
		{
			LevelEventType levelEventType = LevelEventType.None;
			bool flag = false;
			Array values = Enum.GetValues(typeof(LevelEventType));
			foreach (LevelEventType item3 in list)
			{
				if (item3 == cacheSelectedEventType)
				{
					levelEventType = item3;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (LevelEventType item4 in values)
				{
					foreach (LevelEventType item5 in list)
					{
						if (item5 == item4)
						{
							levelEventType = item5;
							break;
						}
					}
					if (levelEventType != LevelEventType.None)
					{
						break;
					}
				}
			}
			selectedEventType = LevelEventType.None;
			if (levelEventType != LevelEventType.AddDecoration && levelEventType != LevelEventType.AddText)
			{
				ADOBase.editor.DeselectAllDecorations();
			}
			ShowPanel(levelEventType);
			ShowInspector(show: true);
		}
		List<string> list2 = new List<string>();
		foreach (LevelEventType item6 in list)
		{
			string item = item6.ToString();
			if (!list2.Contains(item))
			{
				list2.Add(item);
			}
		}
		int count = list2.Count;
		float height = tabs.rect.height;
		float num = 68f;
		if ((float)count * 68f >= height)
		{
			float num2 = (height - 68f * (float)count) / (float)(count * count);
			num = height / (float)count + num2;
		}
		int num3 = -1;
		foreach (Transform tab in tabs)
		{
			bool flag2 = list2.Contains(tab.name);
			tab.gameObject.SetActive(flag2);
			if (flag2)
			{
				num3++;
				list2.Remove(tab.name);
			}
			float y = (0f - num) * (float)num3;
			tab.GetComponent<RectTransform>().SetAnchorPosY(y);
		}
	}

	public void ShowInspector(bool show, bool forceAction = false)
	{
		if (!permaHideInspector || forceAction)
		{
			if (forceAction)
			{
				permaHideInspector = !show;
			}
			showInspector = show;
			RectTransform component = GetComponent<RectTransform>();
			float num = (show ? 0f : component.sizeDelta.x);
			if (!floorPanel)
			{
				num *= -1f;
			}
			if (inspectorTween != null && inspectorTween.active)
			{
				inspectorTween.Kill();
			}
			inspectorTween = component.DOAnchorPosX(num, ADOBase.editor.UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(ADOBase.editor.UIPanelEaseMode);
			if (Persistence.showUnlockKeyLimiterButton)
			{
				unlockKeyLimiterButtonTween = ADOBase.editor.buttonUnlockKeyLimiterRT.DOAnchorPosX(show ? 0f : (-77.5f), ADOBase.editor.UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(ADOBase.editor.UIPanelEaseMode);
			}
		}
	}

	public PropertiesPanel GetPanelOfType(LevelEvent levelEvent)
	{
		PropertiesPanel result = null;
		foreach (PropertiesPanel panels in panelsList)
		{
			if (panels.levelEventType == levelEvent.eventType)
			{
				result = panels;
			}
		}
		return result;
	}

	public void UpdatePropertyText(LevelEvent ev, string property)
	{
		PropertiesPanel panelOfType = GetPanelOfType(ev);
		if (panelOfType == null || !panelOfType.properties.ContainsKey(property))
		{
			return;
		}
		Property property2 = panelOfType.properties[property];
		if (property2.info.type == PropertyType.Vector2)
		{
			PropertyControl_Vector2 propertyControl_Vector = property2.control as PropertyControl_Vector2;
			Vector2 vector = (Vector2)ev[property];
			if (!string.IsNullOrEmpty(propertyControl_Vector.inputX.text))
			{
				propertyControl_Vector.inputX.text = $"{vector.x}";
			}
			if (!string.IsNullOrEmpty(propertyControl_Vector.inputY.text))
			{
				propertyControl_Vector.inputY.text = $"{vector.y}";
			}
		}
		else if (property2.info.type == PropertyType.Float)
		{
			(property2.control as PropertyControl_Text).text = ((float)ev[property]).ToString();
		}
		else if (property2.info.type == PropertyType.Int && property == "floor")
		{
			TMP_InputField inputField = (property2.control as PropertyControl_Text).inputField;
			inputField.text = ev.floor.ToString();
			inputField.Select();
			inputField.onEndEdit.Invoke(inputField.text);
		}
	}

	public void ToggleArtistPopup(string search, float yPos, PropertyControl_Text artistPropertyControl)
	{
		if (EditorWebServices.artists == null)
		{
			return;
		}
		bool flag = lastLevelPath != ADOBase.levelPath;
		lastLevelPath = ADOBase.levelPath;
		if (approvalLevelBadge == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(approvalLevelPrefab, artistPropertyControl.transform);
			approvalLevelBadge = gameObject.GetComponent<ApprovalLevelBadge>();
			approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
		}
		if (!showingPanel)
		{
			ADOBase.editor.levelData.artist = search;
		}
		string text = search.Trim();
		if (text == "")
		{
			artistPopup.gameObject.SetActive(value: false);
			approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
			return;
		}
		search = text.ToLower();
		List<ArtistData> list = (from item in EditorWebServices.artists.AsParallel()
			where item.approvalLevel != ApprovalLevel.Pending && item.name.ToLower().Contains(search)
			select item).ToList();
		foreach (Transform item in artistContainer)
		{
			if (item.gameObject != addArtist)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		int num = 0;
		currentArtist = null;
		addArtist.SetActive(value: false);
		currentArtistDisclaimerAction = null;
		if (list != null && list.Count > 0)
		{
			bool flag2 = false;
			foreach (ArtistData artist in list)
			{
				if (!flag2 && artist.name.ToLower() == search)
				{
					approvalLevelBadge.UpdateUI(artist.approvalLevel, onlyColor: true);
					currentArtist = artist;
					flag2 = true;
					currentArtistDisclaimerAction = delegate
					{
						artistUIDisclaimer.SetData(artist, artistPropertyControl, onlyCheckingMode: true);
					};
				}
				if (!flag)
				{
					ArtistUISettings component = UnityEngine.Object.Instantiate(artistPrefab, artistContainer).GetComponent<ArtistUISettings>();
					component.SetData(artist.name, artist.approvalLevel);
					component.button.onClick.AddListener(delegate
					{
						HideArtistDropdown();
						artistUIDisclaimer.SetData(artist, artistPropertyControl, onlyCheckingMode: false);
					});
				}
			}
			num = list.Count;
			if (!flag2)
			{
				num++;
				approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
			}
			if (!flag)
			{
				addArtist.SetActive(!flag2);
			}
		}
		else if (!flag)
		{
			addArtist.SetActive(value: true);
			num++;
		}
		if (flag)
		{
			return;
		}
		if (addArtist.activeSelf)
		{
			addArtistText.text = RDString.Get("editor.addArtist", new Dictionary<string, object> { { "ArtistName", text } });
			addArtist.transform.SetAsLastSibling();
		}
		float num2 = (float)num * 35f + 12f;
		artistPopup.position = artistPopup.position.WithY(yPos);
		artistPopup.anchoredPosition = artistPopup.anchoredPosition.WithY(artistPopup.anchoredPosition.y - 44f);
		artistContainer.sizeDelta = new Vector2(artistContainer.sizeDelta.x, num2);
		float y = Mathf.Min(num2, artistRectMaxHeight);
		artistPopup.sizeDelta = new Vector2(artistPopup.sizeDelta.x, y);
		artistPopup.gameObject.SetActive(value: true);
		ADOBase.editor.PushPopupBlocker(delegate
		{
			if (!artistUIDisclaimer.gameObject.activeSelf)
			{
				if (currentArtist != null)
				{
					artistUIDisclaimer.SetData(currentArtist, artistPropertyControl, onlyCheckingMode: false);
				}
				else
				{
					artistUIDisclaimer.gameObject.SetActive(value: false);
					HideArtistDropdown();
				}
			}
		});
	}

	public void HideArtistDropdown()
	{
		artistPopup.gameObject.SetActive(value: false);
	}

	public void NewArtist()
	{
		HideArtistDropdown();
	}

	private void LateUpdate()
	{
		if (disableEventButton != null && selectedEvent != null)
		{
			Color color = new Color(0.8392157f, 0.2352941f, 0.2039216f, 1f);
			Color color2 = new Color(0.8f, 0.8f, 0.8f, 1f);
			ColorBlock colors = disableEventButton.colors;
			colors.normalColor = (selectedEvent.active ? color2 : color);
			colors.highlightedColor = colors.normalColor * 1.1f;
			colors.pressedColor = colors.normalColor;
			disableEventButton.colors = colors;
		}
	}

	private void ModifyMessageText(string text, bool enable)
	{
		if (!(messageCanvas == null))
		{
			messageCanvas.SetActive(enable);
			messageText.text = text;
		}
	}

	private void ModifyMessageText(string text, float yPos, bool enable)
	{
		if (!(messageCanvas == null))
		{
			messageCanvas.SetActive(enable);
			messageText.text = text;
			messageText.rectTransform.anchoredPosition = messageText.rectTransform.anchoredPosition.WithY(yPos);
		}
	}

	private bool EventPropertyEquals(PropertyInfo p, object d1, object d2)
	{
		switch (p.type)
		{
		case PropertyType.Int:
		case PropertyType.Rating:
			return (int)d1 == (int)d2;
		case PropertyType.String:
		case PropertyType.LongString:
		case PropertyType.Color:
		case PropertyType.File:
		case PropertyType.Enum:
			return d1.ToString() == d2.ToString();
		case PropertyType.Bool:
			return (bool)d1 == (bool)d2;
		case PropertyType.Float:
			return Math.Abs((float)d1 - (float)d2) < 0.0001f;
		case PropertyType.Vector2:
		{
			Vector2 vector = (Vector2)d1;
			Vector2 vector2 = (Vector2)d2;
			if (!(Mathf.Abs(vector.x - vector2.x) < 0.0001f))
			{
				return Mathf.Abs(vector.y - vector2.y) < 0.0001f;
			}
			return true;
		}
		case PropertyType.Tile:
			return (Tuple<int, TileRelativeTo>)d1 == (Tuple<int, TileRelativeTo>)d2;
		default:
			return false;
		}
	}
}
