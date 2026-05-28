using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADOFAI.Editor;
using ADOFAI.Editor.Components;
using ADOFAI.Editor.Models;
using ADOFAI.LevelEditor.Controls;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI;

public class PropertiesPanel : ADOBase
{
	public struct PropertySelectable(Selectable sel, PropertyControl control, Property propertyRef, bool isPropertyCheckbox = false)
	{
		public Selectable selectable = sel;

		public PropertyControl control = control;

		public Property propertyRef = propertyRef;

		public bool isPropertyCheckbox = isPropertyCheckbox;
	}

	[Header("UI")]
	public GridLayoutGroup layout;

	public ScrollRect scrollRect;

	public RectTransform content;

	public VerticalLayoutGroup contentLayout;

	public RectTransform viewport;

	public ContentSizeFitter contentSizeFitter;

	public Dictionary<string, PropertiesSubTabButton> tabButtons = new Dictionary<string, PropertiesSubTabButton>();

	[Header("Runtime")]
	public Dictionary<string, Property> properties = new Dictionary<string, Property>();

	[NonSerialized]
	public LevelEventType levelEventType;

	[NonSerialized]
	public InspectorPanel inspectorPanel;

	public List<PropertySelectable> propertySelectables = new List<PropertySelectable>();

	private EventSystem eventSystem;

	private static readonly List<string> dontRenderKeys = new List<string> { "selectTarget" };

	[CanBeNull]
	public RectTransform tabContainer { get; set; }

	[CanBeNull]
	public string selectedTab { get; set; }

	private void Awake()
	{
		contentLayout = content.GetComponent<VerticalLayoutGroup>();
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Tab))
		{
			return;
		}
		if ((object)eventSystem == null)
		{
			eventSystem = EventSystem.current;
		}
		if (propertySelectables == null || ADOBase.editor.playMode || propertySelectables.Count == 0 || eventSystem == null || eventSystem.currentSelectedGameObject == null)
		{
			return;
		}
		Selectable currentSel = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
		int num = propertySelectables.FindIndex((PropertySelectable ps) => ps.selectable == currentSel);
		if (currentSel == null || num < 0)
		{
			return;
		}
		bool holdingControl = RDInput.holdingControl;
		bool holdingShift = RDInput.holdingShift;
		int num2 = num;
		int num3 = 0;
		bool flag;
		PropertySelectable propertySelectable;
		PropertyControl control;
		bool flag2;
		do
		{
			flag = false;
			flag2 = false;
			num2 = ((holdingControl || holdingShift) ? (num2 - 1) : (num2 + 1));
			num2 = ((num2 >= 0) ? (num2 % propertySelectables.Count) : (propertySelectables.Count - 1));
			propertySelectable = propertySelectables[num2];
			control = propertySelectable.control;
			flag2 = propertySelectable.isPropertyCheckbox;
			if (control.propertyInfo.canBeDisabled || inspectorPanel.selectedEvent.isFake)
			{
				flag = inspectorPanel.selectedEvent.disabled[control.propertyInfo.name];
			}
			num3++;
			if (num3 > 999)
			{
				num2 = num;
				break;
			}
		}
		while (!control.propertyInfo.CheckIfEnabled(inspectorPanel.selectedEvent, selectedTab) || (flag && !flag2));
		Selectable selectable = propertySelectables[num2].selectable;
		if (flag2)
		{
			selectable.targetGraphic = (flag ? propertySelectable.propertyRef.checkbgImage : propertySelectable.propertyRef.checkImage);
		}
		RectTransform propertyTransform = control.propertyTransform;
		if (!IsControlFullyVisible(propertyTransform))
		{
			float y = content.anchoredPosition.y;
			float y2 = propertyTransform.anchoredPosition.y;
			float num4 = ((y2 > 0f - y) ? y2 : (y2 - propertyTransform.rect.height + viewport.rect.height));
			content.SetAnchorPosY(0f - num4);
		}
		if (selectable.TryGetComponent<InputField>(out var component))
		{
			component.OnPointerClick(new PointerEventData(eventSystem));
		}
		eventSystem.SetSelectedGameObject(selectable.gameObject, new BaseEventData(eventSystem));
	}

	public void Init(InspectorPanel panel, LevelEventInfo levelEventInfo)
	{
		inspectorPanel = panel;
		eventSystem = EventSystem.current;
		Dictionary<string, PropertyInfo> propertiesInfo = levelEventInfo.propertiesInfo;
		Dictionary<string, PropertyInfo>.KeyCollection keys = propertiesInfo.Keys;
		if (levelEventInfo.stretchViewport)
		{
			contentSizeFitter.enabled = false;
			content.anchorMin = Vector2.zero;
			content.anchorMax = Vector2.one;
		}
		foreach (string item in keys)
		{
			if (dontRenderKeys.Contains(item))
			{
				continue;
			}
			PropertyInfo propertyInfo = propertiesInfo[item];
			if (propertyInfo.controlType != ControlType.Hidden)
			{
				bool flag = Application.isEditor && !RDC.hideProEvents;
				if (!propertyInfo.pro || flag || Persistence.enableProEvents)
				{
					RenderControl(item, propertyInfo);
				}
			}
		}
	}

	public void RenderControl(string propertyKey, PropertyInfo propertyInfo)
	{
		LevelEventInfo levelEventInfo = propertyInfo.levelEventInfo;
		GameObject original = null;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		bool flag = false;
		switch (propertyInfo.type)
		{
		case PropertyType.File:
			original = ADOBase.gc.prefab_controlBrowse;
			break;
		case PropertyType.Int:
		case PropertyType.Float:
		case PropertyType.String:
			original = ADOBase.gc.prefab_controlText;
			if (propertyInfo.stringDropdown)
			{
				original = ADOBase.gc.prefab_controlToggle;
				switch (propertyInfo.name)
				{
				case "component":
				{
					Type parentType = typeof(ffxPlusBase);
					foreach (Type item3 in from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsSubclassOf(parentType)
						select t)
					{
						list.Add(item3.ToString());
					}
					break;
				}
				case "filter":
					foreach (Type item4 in from t in typeof(CameraFilterPack_AAA_SuperComputer).Assembly.GetTypes()
						where t.Name.StartsWith("CameraFilterPack_")
						select t)
					{
						string text = item4.ToString();
						if (!ffxSetFilterAdvancedPlus.blacklistedFilterKeywords.Any(text.Contains))
						{
							list.Add(text);
							string text2 = item4.Name.Replace("CameraFilterPack_", "");
							string text3 = text2.UnCamelCase(RDUtils.UnCamelCaseOptions.SpaceBeforeNumbers | RDUtils.UnCamelCaseOptions.UnderbarToSpace);
							bool exists;
							string withCheck = RDString.GetWithCheck("editor.CameraFilterPack." + text2, out exists);
							list2.Add(exists ? withCheck : text3);
						}
					}
					break;
				case "hitsound":
					propertyInfo.enumTypeString = "HitSound";
					flag = true;
					foreach (object value2 in Enum.GetValues(typeof(HitSound)))
					{
						list.Add(value2.ToString());
					}
					break;
				}
			}
			if (propertyInfo.slider)
			{
				original = ADOBase.gc.prefab_controlSlider;
			}
			break;
		case PropertyType.LongString:
			original = ADOBase.gc.prefab_controlLongText;
			break;
		case PropertyType.Enum:
		{
			Type enumType = propertyInfo.enumType;
			original = ADOBase.gc.prefab_controlToggle;
			Array values = Enum.GetValues(enumType);
			if (levelEventInfo == null)
			{
				break;
			}
			foreach (object item5 in values)
			{
				if (enumType == typeof(Ease))
				{
					Ease ease = (Ease)item5;
					if (ease == Ease.Unset || ease == Ease.INTERNAL_Zero || ease == Ease.INTERNAL_Custom)
					{
						continue;
					}
				}
				if ((!levelEventInfo.isDecoration || (DecPlacementType)item5 != DecPlacementType.LastPosition) && (!(enumType == typeof(ObjectDecorationType)) || (ObjectDecorationType)item5 != ObjectDecorationType.PlayerBubble || GCS.isDev) && (propertyInfo.enumExceptions == null || !propertyInfo.enumExceptions.Contains(item5)))
				{
					list.Add(item5.ToString());
				}
			}
			break;
		}
		case PropertyType.Color:
			original = ADOBase.gc.prefab_controlColor;
			break;
		case PropertyType.Bool:
			original = ADOBase.gc.prefab_controlBool;
			break;
		case PropertyType.Vector2:
			original = ADOBase.gc.prefab_controlVector2;
			break;
		case PropertyType.Tile:
			original = ADOBase.gc.prefab_controlTile;
			break;
		case PropertyType.FloatPair:
			original = ADOBase.gc.prefab_floatPair;
			break;
		case PropertyType.MinMaxGradient:
			original = ADOBase.gc.prefab_minMaxGradient;
			break;
		case PropertyType.Vector2Range:
			original = ADOBase.gc.prefab_vector2Range;
			break;
		case PropertyType.Export:
			original = ADOBase.gc.prefab_controlExport;
			break;
		case PropertyType.Rating:
			original = ADOBase.gc.prefab_controlRating;
			break;
		case PropertyType.List:
			if (propertyInfo.name == "decorations")
			{
				original = ADOBase.gc.prefab_controlDecorationsList;
			}
			else if (propertyInfo.name == "events")
			{
				original = ADOBase.gc.prefab_controlEventsList;
			}
			break;
		case PropertyType.FilterProperties:
			original = ADOBase.gc.prefab_controlFilterProperties;
			break;
		case PropertyType.ParticlePlayback:
			original = ADOBase.gc.prefab_controlParticlePlayback;
			break;
		case PropertyType.Note:
			original = ADOBase.gc.prefab_controlNote;
			break;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_property);
		gameObject.transform.SetParent(content, worldPositionStays: false);
		Property property = gameObject.GetComponent<Property>();
		property.gameObject.name = propertyKey;
		property.key = propertyKey;
		property.info = propertyInfo;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(original);
		gameObject2.GetComponent<RectTransform>().SetParent(property.controlContainer, worldPositionStays: false);
		property.control = gameObject2.GetComponent<PropertyControl>();
		property.control.propertyInfo = propertyInfo;
		property.control.propertiesPanel = this;
		property.control.propertyTransform = property.GetComponent<RectTransform>();
		if (propertyInfo.type == PropertyType.Enum || propertyInfo.stringDropdown)
		{
			((PropertyControl_Toggle)property.control).EnumSetup(propertyInfo.enumTypeString, list, propertyInfo.type == PropertyType.Enum || flag, list2, propertyInfo.browsable);
		}
		else if (propertyInfo.type == PropertyType.List && propertyKey == "decorations")
		{
			if (scrollRect.gameObject.TryGetComponent<SmoothScrollRect>(out var component))
			{
				component.enabled = false;
			}
			ScrollRect component2 = gameObject2.GetComponent<ScrollRect>();
			component2.horizontalScrollbar = scrollRect.horizontalScrollbar;
			component2.verticalScrollbar = scrollRect.verticalScrollbar;
			scrollRect.horizontalScrollbar = null;
			scrollRect.verticalScrollbar = null;
			property.label.transform.parent.gameObject.SetActive(value: false);
			ADOBase.editor.decorationsListContent = component2.content;
			scrollRect.content = ADOBase.editor.decorationsListContent;
			scrollRect.vertical = false;
			scrollRect.viewport = gameObject2.GetComponent<ScrollRect>().viewport;
			RectTransform component3 = component2.verticalScrollbar.transform.parent.GetComponent<RectTransform>();
			component3.offsetMin = component3.offsetMin.WithY(60f);
			component3.offsetMax = component3.offsetMax.WithY(-50f);
			ADOBase.editor.propertyControlDecorationsList = (PropertyControl_DecorationsList)property.control;
			((PropertyControl_List)property.control).parentReferenceRT = GetComponent<RectTransform>();
		}
		else if (propertyInfo.type == PropertyType.List && propertyKey == "events")
		{
			ADOBase.editor.eventsListContent = gameObject2.GetComponent<ScrollRect>().content;
			scrollRect.content = ADOBase.editor.eventsListContent;
			scrollRect.vertical = false;
			scrollRect.viewport = gameObject2.GetComponent<ScrollRect>().viewport;
			RectTransform component4 = scrollRect.verticalScrollbar.transform.parent.GetComponent<RectTransform>();
			component4.offsetMin = component4.offsetMin.WithY(60f);
			component4.offsetMax = component4.offsetMax.WithY(-50f);
			ADOBase.editor.propertyControlEventsList = (PropertyControl_EventsList)property.control;
			((PropertyControl_List)property.control).parentReferenceRT = GetComponent<RectTransform>();
		}
		else if (propertyInfo.type == PropertyType.String)
		{
			PropertyControl_Text propertyControl_Text = property.control as PropertyControl_Text;
			string key = propertyInfo.placeholder ?? ("editor." + levelEventInfo.name + "." + propertyInfo.name + ".placeholder");
			bool exists2 = false;
			string withCheck2 = RDString.GetWithCheck(key, out exists2);
			if (exists2)
			{
				(propertyControl_Text.inputField.placeholder as TMP_Text).text = withCheck2;
			}
		}
		else if (propertyInfo.type == PropertyType.Int && propertyKey == "floor")
		{
			PropertyControl_Text obj = property.control as PropertyControl_Text;
			obj.linkFloorIDButton.gameObject.SetActive(value: true);
			obj.goToFloorButton.gameObject.SetActive(value: true);
			obj.inputField.characterLimit = 10;
		}
		if (propertyInfo.type == PropertyType.Vector2)
		{
			PropertyControl_Vector2 obj2 = property.control as PropertyControl_Vector2;
			TMP_Text tMP_Text = obj2.inputX.placeholder as TMP_Text;
			TMP_Text obj3 = obj2.inputY.placeholder as TMP_Text;
			tMP_Text.text = (propertyInfo.vector2_allowEmpty ? "—" : "");
			obj3.text = (propertyInfo.vector2_allowEmpty ? "—" : "");
		}
		if (property.control is PropertyControl_FloatPair propertyControl_FloatPair)
		{
			propertyControl_FloatPair.control.min = propertyInfo.floatPairMin;
			propertyControl_FloatPair.control.max = propertyInfo.floatPairMax;
			propertyControl_FloatPair.control.clamp = true;
			propertyControl_FloatPair.control.isRange = propertyInfo.floatPairIsRange;
		}
		if (property.control is PropertyControl_Vector2Range propertyControl_Vector2Range)
		{
			propertyControl_Vector2Range.controlX.clamp = false;
			propertyControl_Vector2Range.controlY.clamp = false;
			propertyControl_Vector2Range.controlX.isRange = propertyInfo.floatPairIsRange;
			propertyControl_Vector2Range.controlY.isRange = propertyInfo.floatPairIsRange;
		}
		string key2 = "editor." + property.key + ".help";
		bool exists3;
		string helpString = RDString.GetWithCheck(key2, out exists3);
		if (exists3)
		{
			Button helpButton = property.helpButton;
			helpButton.transform.parent.gameObject.SetActive(value: true);
			string buttonText = RDString.GetWithCheck("editor." + property.key + ".help.buttonText", out exists3);
			string buttonURL = RDString.GetWithCheck("editor." + property.key + ".help.buttonURL", out exists3);
			helpButton.onClick.AddListener(delegate
			{
				ADOBase.editor.ShowPropertyHelp(show: true, helpButton.transform, helpString, buttonText, buttonURL);
			});
		}
		property.control.Setup(addListener: true);
		if (property.info.hasRandomValue && levelEventInfo != null)
		{
			string randValueKey = property.info.randValueKey;
			property.control.randomControl.propertyInfo = levelEventInfo.propertiesInfo[randValueKey];
			property.control.randomControl.propertiesPanel = this;
			property.control.randomControl.Setup(addListener: true);
			Button randomButton = property.randomButton;
			randomButton.gameObject.SetActive(value: true);
			randomButton.onClick.AddListener(delegate
			{
				string randModeKey = property.info.randModeKey;
				int num = ((int)inspectorPanel.selectedEvent[randModeKey] + 1) % 3;
				inspectorPanel.selectedEvent[randModeKey] = (RandomMode)num;
				property.control.SetRandomLayout();
			});
		}
		property.enabledButton.onClick.AddListener(delegate
		{
			bool isFake = inspectorPanel.selectedEvent.isFake;
			using (new SaveStateScope(ADOBase.editor))
			{
				bool value;
				bool flag2 = inspectorPanel.selectedEvent.disabled.TryGetValue(propertyKey, out value) && !value;
				inspectorPanel.selectedEvent.disabled[propertyKey] = flag2;
				property.offText.SetActive(flag2);
				property.enabledCheckmark.SetActive(!flag2);
				property.control.gameObject.SetActive(!flag2);
				property.control.OnValueChange();
				UpdateEnabledButton(property, flag2);
				if (isFake)
				{
					property.enabledCheckmark.transform.parent.gameObject.SetActive(value: false);
					property.enabledButton.gameObject.SetActive(value: false);
					inspectorPanel.selectedEvent.ApplyPropertiesToRealEvents();
				}
				property.control.ApplyTileChanges();
			}
		});
		if (property.info.canBeDisabled)
		{
			Button component5 = property.enabledButton.GetComponent<Button>();
			ColorBlock colors = component5.colors;
			colors.selectedColor = InspectorPanel.selectionColor;
			component5.colors = colors;
			PropertySelectable item = new PropertySelectable(component5, property.control, property, isPropertyCheckbox: true);
			propertySelectables.Add(item);
		}
		if (property.control.selectables != null)
		{
			foreach (Selectable selectable in property.control.selectables)
			{
				PropertySelectable item2 = new PropertySelectable(selectable, property.control, property);
				propertySelectables.Add(item2);
			}
		}
		properties.Add(propertyInfo.name, property);
	}

	private void UpdateEnabledButton(Property property, bool disabled)
	{
		RectTransform component = property.enabledButton.GetComponent<RectTransform>();
		if (disabled)
		{
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.offsetMin = Vector2.zero;
			component.offsetMax = Vector2.zero;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
		}
		else
		{
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = new Vector2(0f, 32f);
		}
	}

	public void SetupCheckmark(Property property, PropertyControl control)
	{
		bool flag = inspectorPanel.selectedEvent?.isFake ?? false;
		bool flag2 = control.propertyInfo.canBeDisabled || flag;
		bool value = default(bool);
		bool flag3 = flag2 && inspectorPanel.selectedEvent.disabled.TryGetValue(control.propertyInfo.name, out value) && value;
		bool active = (flag ? flag3 : flag2);
		property.offText.SetActive(flag3);
		property.enabledCheckmark.SetActive(!flag3);
		control.gameObject.SetActive(!flag3);
		property.enabledButton.gameObject.SetActive(value: true);
		UpdateEnabledButton(property, flag3);
		property.enabledCheckmark.transform.parent.gameObject.SetActive(active);
		property.enabledButton.gameObject.SetActive(active);
	}

	public void SetProperties(LevelEvent levelEvent, bool checkIfEnabled = true)
	{
		foreach (string item in levelEvent.GetData().Keys.ToList())
		{
			if (!properties.ContainsKey(item))
			{
				continue;
			}
			Property property = properties[item];
			PropertyControl control = property.control;
			if (control == null)
			{
				continue;
			}
			PropertyType type = control.propertyInfo.type;
			if (type == PropertyType.Export || type == PropertyType.List)
			{
				continue;
			}
			switch (type)
			{
			case PropertyType.Vector2:
				control.text = ((Vector2)levelEvent[item]).ToString("f6");
				if (control.propertyInfo.hasRandomValue)
				{
					control.randomControl.text = ((Vector2)levelEvent[control.propertyInfo.randValueKey]).ToString("f6");
					control.SetRandomLayout();
				}
				break;
			case PropertyType.Tile:
				(control as PropertyControl_Tile).tileValue = levelEvent.GetTile(item);
				break;
			case PropertyType.FilterProperties:
			{
				PropertyControl_FilterProperties propertyControl_FilterProperties = control as PropertyControl_FilterProperties;
				if (levelEvent.TryGet<bool>("isNewlyAdded", out var output))
				{
					propertyControl_FilterProperties.enableProperties = output;
					levelEvent.GetData().Remove("isNewlyAdded");
				}
				break;
			}
			case PropertyType.Bool:
				((PropertyControl_Bool)control).value = levelEvent.GetBool(item);
				break;
			default:
				if (control is PropertyControl_ParticlePlayback propertyControl_ParticlePlayback)
				{
					propertyControl_ParticlePlayback.Decoration = (scrParticleDecoration)scrDecorationManager.GetDecoration(levelEvent);
					Debug.Log(propertyControl_ParticlePlayback.Decoration);
				}
				else if (control is PropertyControl_FloatPair propertyControl_FloatPair)
				{
					MinMaxControl control2 = propertyControl_FloatPair.control;
					Tuple<float, float> tuple = ((Tuple<float, float>)levelEvent[item]) ?? new Tuple<float, float>(0f, 0f);
					control2.start = tuple.Item1;
					control2.end = tuple.Item2;
					control2.RefreshInput();
				}
				else if (control is PropertyControl_Vector2Range propertyControl_Vector2Range)
				{
					Tuple<Vector2, Vector2> tuple2 = ((Tuple<Vector2, Vector2>)levelEvent[item]) ?? new Tuple<Vector2, Vector2>(Vector2.zero, Vector2.zero);
					propertyControl_Vector2Range.controlX.start = tuple2.Item1.x;
					propertyControl_Vector2Range.controlX.end = tuple2.Item2.x;
					propertyControl_Vector2Range.controlY.start = tuple2.Item1.y;
					propertyControl_Vector2Range.controlY.end = tuple2.Item2.y;
					propertyControl_Vector2Range.controlX.RefreshInput();
					propertyControl_Vector2Range.controlY.RefreshInput();
				}
				else if (control is PropertyControl_MinMaxGradient propertyControl_MinMaxGradient)
				{
					propertyControl_MinMaxGradient.SetValue((SerializedMinMaxGradient)levelEvent[item]);
				}
				else if (!(control is PropertyControl_Note))
				{
					string text = levelEvent[item].ToString();
					control.text = ((control.propertyInfo.stringDropdown && string.IsNullOrEmpty(text)) ? ((PropertyControl_Toggle)control).dropdown.items.First().value : text);
					if (control.propertyInfo.hasRandomValue)
					{
						control.randomControl.text = levelEvent[control.propertyInfo.randValueKey].ToString();
						control.SetRandomLayout();
					}
				}
				break;
			}
			if (checkIfEnabled)
			{
				control.ToggleOthersEnabled();
			}
			if (inspectorPanel.selectedEvent != null)
			{
				SetupCheckmark(property, control);
			}
			property.control?.OnSelectedEventChanged(levelEvent);
		}
		if (properties.ContainsKey("floor"))
		{
			int num = Mathf.Clamp(levelEvent.floor, 0, ADOBase.editor.floors.Count - 1);
			Property property2 = properties["floor"];
			PropertyControl control3 = property2.control;
			control3.text = num.ToString();
			SetupCheckmark(property2, control3);
		}
	}

	private bool IsControlFullyVisible(RectTransform rt)
	{
		float y = content.anchoredPosition.y;
		float y2 = rt.anchoredPosition.y;
		float height = rt.rect.height;
		float height2 = viewport.rect.height;
		if (y2 <= 0f - y)
		{
			return y2 - height > 0f - height2 - y;
		}
		return false;
	}

	public void SelectTab(string targetTab)
	{
		selectedTab = targetTab;
		foreach (PropertiesSubTabButton value in tabButtons.Values)
		{
			value.SetSelected(targetTab == value.groupName);
		}
		foreach (Property value2 in properties.Values)
		{
			value2.gameObject.SetActive(value2.info.groupName == targetTab);
			value2.control.UpdateEnabled();
		}
	}
}
