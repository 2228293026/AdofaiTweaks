using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired.Glyphs;
using Rewired.Glyphs.UnityUI;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class ControlMapper : MonoBehaviour
{
	private abstract class GUIElement
	{
		public readonly GameObject gameObject;

		protected readonly Text text;

		public readonly Selectable selectable;

		protected readonly UIElementInfo uiElementInfo;

		protected bool permanentStateSet;

		protected readonly List<GUIElement> children;

		public RectTransform rectTransform { get; private set; }

		public GUIElement(GameObject gameObject)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: gameObject is null!");
				return;
			}
			selectable = gameObject.GetComponent<Selectable>();
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: Selectable is null!");
				return;
			}
			this.gameObject = gameObject;
			rectTransform = gameObject.GetComponent<RectTransform>();
			text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
			uiElementInfo = gameObject.GetComponent<UIElementInfo>();
			children = new List<GUIElement>();
		}

		public GUIElement(Selectable selectable, Text label)
		{
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: Selectable is null!");
				return;
			}
			this.selectable = selectable;
			gameObject = selectable.gameObject;
			rectTransform = gameObject.GetComponent<RectTransform>();
			text = label;
			uiElementInfo = gameObject.GetComponent<UIElementInfo>();
			children = new List<GUIElement>();
		}

		public virtual void SetInteractible(bool state, bool playTransition)
		{
			SetInteractible(state, playTransition, permanent: false);
		}

		public virtual void SetInteractible(bool state, bool playTransition, bool permanent)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] != null)
				{
					children[i].SetInteractible(state, playTransition, permanent);
				}
			}
			if (!permanentStateSet && !(selectable == null))
			{
				if (permanent)
				{
					permanentStateSet = true;
				}
				if (selectable.interactable != state)
				{
					UITools.SetInteractable(selectable, state, playTransition);
				}
			}
		}

		public virtual void SetTextWidth(int value)
		{
			if (!(text == null))
			{
				LayoutElement layoutElement = text.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = text.gameObject.AddComponent<LayoutElement>();
				}
				layoutElement.preferredWidth = value;
			}
		}

		public virtual void SetFirstChildObjectWidth(LayoutElementSizeType type, int value)
		{
			if (rectTransform.childCount != 0)
			{
				Transform child = rectTransform.GetChild(0);
				LayoutElement layoutElement = child.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = child.gameObject.AddComponent<LayoutElement>();
				}
				switch (type)
				{
				case LayoutElementSizeType.MinSize:
					layoutElement.minWidth = value;
					break;
				case LayoutElementSizeType.PreferredSize:
					layoutElement.preferredWidth = value;
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		public virtual void SetLabel(string label)
		{
			if (!(text == null))
			{
				text.text = label;
			}
		}

		public virtual string GetLabel()
		{
			if (text == null)
			{
				return string.Empty;
			}
			return text.text;
		}

		public virtual void AddChild(GUIElement child)
		{
			children.Add(child);
		}

		public void SetElementInfoData(string identifier, int intData)
		{
			if (!(uiElementInfo == null))
			{
				uiElementInfo.identifier = identifier;
				uiElementInfo.intData = intData;
			}
		}

		public virtual void SetActive(bool state)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(state);
			}
		}

		protected virtual bool Init()
		{
			bool result = true;
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] != null && !children[i].Init())
				{
					result = false;
				}
			}
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
				result = false;
			}
			if (rectTransform == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
				result = false;
			}
			if (uiElementInfo == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
				result = false;
			}
			return result;
		}
	}

	private class GUIButton : GUIElement
	{
		protected Button button => selectable as Button;

		public ButtonInfo buttonInfo => uiElementInfo as ButtonInfo;

		public GUIButton(GameObject gameObject)
			: base(gameObject)
		{
			Init();
		}

		public GUIButton(Button button, Text label)
			: base(button, label)
		{
			Init();
		}

		public void SetButtonInfoData(string identifier, int intData)
		{
			SetElementInfoData(identifier, intData);
		}

		public void SetOnClickCallback(Action<ButtonInfo> callback)
		{
			if (!(button == null))
			{
				button.onClick.AddListener(delegate
				{
					callback(buttonInfo);
				});
			}
		}
	}

	private class GUIInputField : GUIElement
	{
		protected Button button => selectable as Button;

		public InputFieldInfo fieldInfo => uiElementInfo as InputFieldInfo;

		public bool hasToggle => toggle != null;

		public GUIToggle toggle { get; private set; }

		public int actionElementMapId
		{
			get
			{
				if (fieldInfo == null)
				{
					return -1;
				}
				return fieldInfo.actionElementMapId;
			}
			set
			{
				if (!(fieldInfo == null))
				{
					fieldInfo.actionElementMapId = value;
				}
			}
		}

		public int controllerId
		{
			get
			{
				if (fieldInfo == null)
				{
					return -1;
				}
				return fieldInfo.controllerId;
			}
			set
			{
				if (!(fieldInfo == null))
				{
					fieldInfo.controllerId = value;
				}
			}
		}

		public GUIInputField(GameObject gameObject)
			: base(gameObject)
		{
			if (Init())
			{
				fieldInfo.glyphOrText = UnityTools.GetComponentInSelfOrChildren<UnityUIControllerElementGlyph>(gameObject);
			}
		}

		public GUIInputField(Button button, Text label)
			: base(button, label)
		{
			if (Init())
			{
				fieldInfo.glyphOrText = UnityTools.GetComponentInSelfOrChildren<UnityUIControllerElementGlyph>(button.gameObject);
			}
		}

		public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			SetElementInfoData(string.Empty, intData);
			if (!(fieldInfo == null))
			{
				fieldInfo.actionId = actionId;
				fieldInfo.axisRange = axisRange;
				fieldInfo.controllerType = controllerType;
			}
		}

		public void SetOnClickCallback(Action<InputFieldInfo> callback)
		{
			if (!(button == null))
			{
				button.onClick.AddListener(delegate
				{
					callback(fieldInfo);
				});
			}
		}

		public virtual void SetInteractable(bool state, bool playTransition, bool permanent)
		{
			if (!permanentStateSet)
			{
				if (hasToggle && !state)
				{
					toggle.SetInteractible(state, playTransition, permanent);
				}
				base.SetInteractible(state, playTransition, permanent);
			}
		}

		public void AddToggle(GUIToggle toggle)
		{
			if (toggle != null)
			{
				this.toggle = toggle;
			}
		}
	}

	private class GUIToggle : GUIElement
	{
		protected Toggle toggle => selectable as Toggle;

		public ToggleInfo toggleInfo => uiElementInfo as ToggleInfo;

		public int actionElementMapId
		{
			get
			{
				if (toggleInfo == null)
				{
					return -1;
				}
				return toggleInfo.actionElementMapId;
			}
			set
			{
				if (!(toggleInfo == null))
				{
					toggleInfo.actionElementMapId = value;
				}
			}
		}

		public GUIToggle(GameObject gameObject)
			: base(gameObject)
		{
			Init();
		}

		public GUIToggle(Toggle toggle, Text label)
			: base(toggle, label)
		{
			Init();
		}

		public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			SetElementInfoData(string.Empty, intData);
			if (!(toggleInfo == null))
			{
				toggleInfo.actionId = actionId;
				toggleInfo.axisRange = axisRange;
				toggleInfo.controllerType = controllerType;
			}
		}

		public void SetOnSubmitCallback(Action<ToggleInfo, bool> callback)
		{
			if (toggle == null)
			{
				return;
			}
			EventTrigger eventTrigger = toggle.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = toggle.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener(delegate(BaseEventData data)
			{
				if (!(data is PointerEventData { button: not PointerEventData.InputButton.Left }))
				{
					callback(toggleInfo, toggle.isOn);
				}
			});
			EventTrigger.Entry item = new EventTrigger.Entry
			{
				callback = triggerEvent,
				eventID = EventTriggerType.Submit
			};
			EventTrigger.Entry item2 = new EventTrigger.Entry
			{
				callback = triggerEvent,
				eventID = EventTriggerType.PointerClick
			};
			if (eventTrigger.triggers != null)
			{
				eventTrigger.triggers.Clear();
			}
			else
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			eventTrigger.triggers.Add(item);
			eventTrigger.triggers.Add(item2);
		}

		public void SetToggleState(bool state)
		{
			if (!(toggle == null))
			{
				toggle.isOn = state;
			}
		}
	}

	private class GUILabel
	{
		public GameObject gameObject { get; private set; }

		private Text text { get; set; }

		public RectTransform rectTransform { get; private set; }

		public GUILabel(GameObject gameObject)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: gameObject is null!");
				return;
			}
			text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
			Check();
		}

		public GUILabel(Text label)
		{
			text = label;
			Check();
		}

		public void SetSize(int width, int height)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
		}

		public void SetWidth(int width)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}

		public void SetHeight(int height)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
		}

		public void SetLabel(string label)
		{
			if (!(text == null))
			{
				text.text = label;
			}
		}

		public void SetFontStyle(FontStyle style)
		{
			if (!(text == null))
			{
				text.fontStyle = style;
			}
		}

		public void SetTextAlignment(TextAnchor alignment)
		{
			if (!(text == null))
			{
				text.alignment = alignment;
			}
		}

		public void SetActive(bool state)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(state);
			}
		}

		private bool Check()
		{
			bool result = true;
			if (text == null)
			{
				Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
				result = false;
			}
			gameObject = text.gameObject;
			rectTransform = text.GetComponent<RectTransform>();
			return result;
		}
	}

	[Serializable]
	public class MappingSet
	{
		public enum ActionListMode
		{
			ActionCategory,
			Action
		}

		[SerializeField]
		[Tooltip("The Map Category that will be displayed to the user for remapping.")]
		private int _mapCategoryId;

		[SerializeField]
		[Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
		private ActionListMode _actionListMode;

		[SerializeField]
		private int[] _actionCategoryIds;

		[SerializeField]
		private int[] _actionIds;

		private IList<int> _actionCategoryIdsReadOnly;

		private IList<int> _actionIdsReadOnly;

		public int mapCategoryId => _mapCategoryId;

		public ActionListMode actionListMode => _actionListMode;

		public IList<int> actionCategoryIds
		{
			get
			{
				if (_actionCategoryIds == null)
				{
					return null;
				}
				if (_actionCategoryIdsReadOnly == null)
				{
					_actionCategoryIdsReadOnly = new ReadOnlyCollection<int>(_actionCategoryIds);
				}
				return _actionCategoryIdsReadOnly;
			}
		}

		public IList<int> actionIds
		{
			get
			{
				if (_actionIds == null)
				{
					return null;
				}
				if (_actionIdsReadOnly == null)
				{
					_actionIdsReadOnly = new ReadOnlyCollection<int>(_actionIds);
				}
				return _actionIds;
			}
		}

		public bool isValid
		{
			get
			{
				if (_mapCategoryId < 0)
				{
					return false;
				}
				InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(_mapCategoryId);
				if (mapCategory == null)
				{
					return false;
				}
				if (!((InputCategory)mapCategory).userAssignable)
				{
					return false;
				}
				return true;
			}
		}

		public static MappingSet Default => new MappingSet(0, ActionListMode.ActionCategory, new int[1], new int[0]);

		public MappingSet()
		{
			_mapCategoryId = -1;
			_actionCategoryIds = new int[0];
			_actionIds = new int[0];
			_actionListMode = ActionListMode.ActionCategory;
		}

		private MappingSet(int mapCategoryId, ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds)
		{
			_mapCategoryId = mapCategoryId;
			_actionListMode = actionListMode;
			_actionCategoryIds = actionCategoryIds;
			_actionIds = actionIds;
		}
	}

	[Serializable]
	public class InputBehaviorSettings
	{
		[SerializeField]
		[Tooltip("The Input Behavior that will be displayed to the user for modification.")]
		private int _inputBehaviorId = -1;

		[SerializeField]
		[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
		private bool _showJoystickAxisSensitivity = true;

		[SerializeField]
		[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
		private bool _showMouseXYAxisSensitivity = true;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
		private string _labelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
		private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
		private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
		private Sprite _joystickAxisSensitivityIcon;

		[SerializeField]
		[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
		private Sprite _mouseXYAxisSensitivityIcon;

		[SerializeField]
		[Tooltip("Minimum value the user is allowed to set for this property.")]
		private float _joystickAxisSensitivityMin;

		[SerializeField]
		[Tooltip("Maximum value the user is allowed to set for this property.")]
		private float _joystickAxisSensitivityMax = 2f;

		[SerializeField]
		[Tooltip("Minimum value the user is allowed to set for this property.")]
		private float _mouseXYAxisSensitivityMin;

		[SerializeField]
		[Tooltip("Maximum value the user is allowed to set for this property.")]
		private float _mouseXYAxisSensitivityMax = 2f;

		public int inputBehaviorId => _inputBehaviorId;

		public bool showJoystickAxisSensitivity => _showJoystickAxisSensitivity;

		public bool showMouseXYAxisSensitivity => _showMouseXYAxisSensitivity;

		public string labelLanguageKey => _labelLanguageKey;

		public string joystickAxisSensitivityLabelLanguageKey => _joystickAxisSensitivityLabelLanguageKey;

		public string mouseXYAxisSensitivityLabelLanguageKey => _mouseXYAxisSensitivityLabelLanguageKey;

		public Sprite joystickAxisSensitivityIcon => _joystickAxisSensitivityIcon;

		public Sprite mouseXYAxisSensitivityIcon => _mouseXYAxisSensitivityIcon;

		public float joystickAxisSensitivityMin => _joystickAxisSensitivityMin;

		public float joystickAxisSensitivityMax => _joystickAxisSensitivityMax;

		public float mouseXYAxisSensitivityMin => _mouseXYAxisSensitivityMin;

		public float mouseXYAxisSensitivityMax => _mouseXYAxisSensitivityMax;

		public bool isValid
		{
			get
			{
				if (_inputBehaviorId >= 0)
				{
					if (!_showJoystickAxisSensitivity)
					{
						return _showMouseXYAxisSensitivity;
					}
					return true;
				}
				return false;
			}
		}
	}

	[Serializable]
	private class Prefabs
	{
		[SerializeField]
		private GameObject _button;

		[SerializeField]
		private GameObject _fitButton;

		[SerializeField]
		private GameObject _inputGridLabel;

		[SerializeField]
		private GameObject _inputGridHeaderLabel;

		[SerializeField]
		private GameObject _inputGridFieldButton;

		[SerializeField]
		private GameObject _inputGridFieldInvertToggle;

		[SerializeField]
		private GameObject _window;

		[SerializeField]
		private GameObject _windowTitleText;

		[SerializeField]
		private GameObject _windowContentText;

		[SerializeField]
		private GameObject _fader;

		[SerializeField]
		private GameObject _calibrationWindow;

		[SerializeField]
		private GameObject _inputBehaviorsWindow;

		[SerializeField]
		private GameObject _centerStickGraphic;

		[SerializeField]
		private GameObject _moveStickGraphic;

		public GameObject button => _button;

		public GameObject fitButton => _fitButton;

		public GameObject inputGridLabel => _inputGridLabel;

		public GameObject inputGridHeaderLabel => _inputGridHeaderLabel;

		public GameObject inputGridFieldButton => _inputGridFieldButton;

		public GameObject inputGridFieldInvertToggle => _inputGridFieldInvertToggle;

		public GameObject window => _window;

		public GameObject windowTitleText => _windowTitleText;

		public GameObject windowContentText => _windowContentText;

		public GameObject fader => _fader;

		public GameObject calibrationWindow => _calibrationWindow;

		public GameObject inputBehaviorsWindow => _inputBehaviorsWindow;

		public GameObject centerStickGraphic => _centerStickGraphic;

		public GameObject moveStickGraphic => _moveStickGraphic;

		public bool Check()
		{
			if (_button == null || _fitButton == null || _inputGridLabel == null || _inputGridHeaderLabel == null || _inputGridFieldButton == null || _inputGridFieldInvertToggle == null || _window == null || _windowTitleText == null || _windowContentText == null || _fader == null || _calibrationWindow == null || _inputBehaviorsWindow == null)
			{
				return false;
			}
			return true;
		}
	}

	[Serializable]
	private class References
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private CanvasGroup _mainCanvasGroup;

		[SerializeField]
		private Transform _mainContent;

		[SerializeField]
		private Transform _mainContentInner;

		[SerializeField]
		private UIGroup _playersGroup;

		[SerializeField]
		private Transform _controllerGroup;

		[SerializeField]
		private Transform _controllerGroupLabelGroup;

		[SerializeField]
		private UIGroup _controllerSettingsGroup;

		[SerializeField]
		private UIGroup _assignedControllersGroup;

		[SerializeField]
		private Transform _settingsAndMapCategoriesGroup;

		[SerializeField]
		private UIGroup _settingsGroup;

		[SerializeField]
		private UIGroup _mapCategoriesGroup;

		[SerializeField]
		private Transform _inputGridGroup;

		[SerializeField]
		private Transform _inputGridContainer;

		[SerializeField]
		private Transform _inputGridHeadersGroup;

		[SerializeField]
		private Scrollbar _inputGridVScrollbar;

		[SerializeField]
		private ScrollRect _inputGridScrollRect;

		[SerializeField]
		private Transform _inputGridInnerGroup;

		[SerializeField]
		private Text _controllerNameLabel;

		[SerializeField]
		private Button _removeControllerButton;

		[SerializeField]
		private Button _assignControllerButton;

		[SerializeField]
		private Button _calibrateControllerButton;

		[SerializeField]
		private Button _doneButton;

		[SerializeField]
		private Button _restoreDefaultsButton;

		[SerializeField]
		private Selectable _defaultSelection;

		[SerializeField]
		private GameObject[] _fixedSelectableUIElements;

		[SerializeField]
		private Image _mainBackgroundImage;

		public Canvas canvas => _canvas;

		public CanvasGroup mainCanvasGroup => _mainCanvasGroup;

		public Transform mainContent => _mainContent;

		public Transform mainContentInner => _mainContentInner;

		public UIGroup playersGroup => _playersGroup;

		public Transform controllerGroup => _controllerGroup;

		public Transform controllerGroupLabelGroup => _controllerGroupLabelGroup;

		public UIGroup controllerSettingsGroup => _controllerSettingsGroup;

		public UIGroup assignedControllersGroup => _assignedControllersGroup;

		public Transform settingsAndMapCategoriesGroup => _settingsAndMapCategoriesGroup;

		public UIGroup settingsGroup => _settingsGroup;

		public UIGroup mapCategoriesGroup => _mapCategoriesGroup;

		public Transform inputGridGroup => _inputGridGroup;

		public Transform inputGridContainer => _inputGridContainer;

		public Transform inputGridHeadersGroup => _inputGridHeadersGroup;

		public Scrollbar inputGridVScrollbar => _inputGridVScrollbar;

		public ScrollRect inputGridScrollRect => _inputGridScrollRect;

		public Transform inputGridInnerGroup => _inputGridInnerGroup;

		public Text controllerNameLabel => _controllerNameLabel;

		public Button removeControllerButton => _removeControllerButton;

		public Button assignControllerButton => _assignControllerButton;

		public Button calibrateControllerButton => _calibrateControllerButton;

		public Button doneButton => _doneButton;

		public Button restoreDefaultsButton => _restoreDefaultsButton;

		public Selectable defaultSelection => _defaultSelection;

		public GameObject[] fixedSelectableUIElements => _fixedSelectableUIElements;

		public Image mainBackgroundImage => _mainBackgroundImage;

		public LayoutElement inputGridLayoutElement { get; set; }

		public Transform inputGridActionColumn { get; set; }

		public Transform inputGridKeyboardColumn { get; set; }

		public Transform inputGridMouseColumn { get; set; }

		public Transform inputGridControllerColumn { get; set; }

		public Transform inputGridHeader1 { get; set; }

		public Transform inputGridHeader2 { get; set; }

		public Transform inputGridHeader3 { get; set; }

		public Transform inputGridHeader4 { get; set; }

		public bool Check()
		{
			if (_canvas == null || _mainCanvasGroup == null || _mainContent == null || _mainContentInner == null || _playersGroup == null || _controllerGroup == null || _controllerGroupLabelGroup == null || _controllerSettingsGroup == null || _assignedControllersGroup == null || _settingsAndMapCategoriesGroup == null || _settingsGroup == null || _mapCategoriesGroup == null || _inputGridGroup == null || _inputGridContainer == null || _inputGridHeadersGroup == null || _inputGridVScrollbar == null || _inputGridScrollRect == null || _inputGridInnerGroup == null || _controllerNameLabel == null || _removeControllerButton == null || _assignControllerButton == null || _calibrateControllerButton == null || _doneButton == null || _restoreDefaultsButton == null || _defaultSelection == null)
			{
				return false;
			}
			return true;
		}
	}

	private class InputActionSet
	{
		private int _actionId;

		private AxisRange _axisRange;

		public int actionId => _actionId;

		public AxisRange axisRange => _axisRange;

		public InputActionSet(int actionId, AxisRange axisRange)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			_actionId = actionId;
			_axisRange = axisRange;
		}
	}

	private class InputMapping
	{
		public string actionName { get; private set; }

		public InputFieldInfo fieldInfo { get; private set; }

		public ControllerMap map { get; private set; }

		public ActionElementMap aem { get; private set; }

		public ControllerType controllerType { get; private set; }

		public int controllerId { get; private set; }

		public ControllerPollingInfo pollingInfo { get; set; }

		public ModifierKeyFlags modifierKeyFlags { get; set; }

		public AxisRange axisRange
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				AxisRange result = (AxisRange)1;
				ControllerPollingInfo val = pollingInfo;
				if ((int)((ControllerPollingInfo)(ref val)).elementType == 0)
				{
					if ((int)fieldInfo.axisRange == 0)
					{
						result = (AxisRange)0;
					}
					else
					{
						val = pollingInfo;
						result = (AxisRange)(((int)((ControllerPollingInfo)(ref val)).axisPole == 0) ? 1 : 2);
					}
				}
				return result;
			}
		}

		public string elementName
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				ControllerPollingInfo val;
				if ((int)controllerType == 0)
				{
					LanguageDataBase language = GetLanguage();
					val = pollingInfo;
					return language.GetElementIdentifierName(((ControllerPollingInfo)(ref val)).keyboardKey, modifierKeyFlags);
				}
				LanguageDataBase language2 = GetLanguage();
				val = pollingInfo;
				Controller controller = ((ControllerPollingInfo)(ref val)).controller;
				val = pollingInfo;
				int elementIdentifierId = ((ControllerPollingInfo)(ref val)).elementIdentifierId;
				val = pollingInfo;
				return language2.GetElementIdentifierName(controller, elementIdentifierId, (AxisRange)(((int)((ControllerPollingInfo)(ref val)).axisPole == 0) ? 1 : 2));
			}
		}

		public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			this.actionName = actionName;
			this.fieldInfo = fieldInfo;
			this.map = map;
			this.aem = aem;
			this.controllerType = controllerType;
			this.controllerId = controllerId;
		}

		public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			this.pollingInfo = pollingInfo;
			return ToElementAssignment();
		}

		public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			this.pollingInfo = pollingInfo;
			this.modifierKeyFlags = modifierKeyFlags;
			return ToElementAssignment();
		}

		public ElementAssignment ToElementAssignment()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			ControllerType val = controllerType;
			ControllerPollingInfo val2 = pollingInfo;
			ControllerElementType elementType = ((ControllerPollingInfo)(ref val2)).elementType;
			val2 = pollingInfo;
			int elementIdentifierId = ((ControllerPollingInfo)(ref val2)).elementIdentifierId;
			AxisRange val3 = axisRange;
			val2 = pollingInfo;
			return new ElementAssignment(val, elementType, elementIdentifierId, val3, ((ControllerPollingInfo)(ref val2)).keyboardKey, modifierKeyFlags, fieldInfo.actionId, (Pole)((int)fieldInfo.axisRange == 2), false, (aem != null) ? aem.id : (-1));
		}
	}

	private class AxisCalibrator
	{
		public AxisCalibrationData data;

		public readonly Joystick joystick;

		public readonly int axisIndex;

		private Axis axis;

		private bool firstRun;

		public bool isValid => axis != null;

		public AxisCalibrator(Joystick joystick, int axisIndex)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			data = default(AxisCalibrationData);
			this.joystick = joystick;
			this.axisIndex = axisIndex;
			if (joystick != null && axisIndex >= 0 && ((ControllerWithAxes)joystick).axisCount > axisIndex)
			{
				axis = ((ControllerWithAxes)joystick).Axes[axisIndex];
				data = ((ControllerWithAxes)joystick).calibrationMap.GetAxis(axisIndex).GetData();
			}
			firstRun = true;
		}

		public void RecordMinMax()
		{
			if (axis != null)
			{
				float valueRaw = axis.valueRaw;
				if (firstRun || valueRaw < data.min)
				{
					data.min = valueRaw;
				}
				if (firstRun || valueRaw > data.max)
				{
					data.max = valueRaw;
				}
				firstRun = false;
			}
		}

		public void RecordZero()
		{
			if (axis != null)
			{
				data.zero = axis.valueRaw;
			}
		}

		public void Commit()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (axis != null)
			{
				AxisCalibration val = ((ControllerWithAxes)joystick).calibrationMap.GetAxis(axisIndex);
				if (val != null && !((double)Mathf.Abs(data.max - data.min) < 0.1))
				{
					val.SetData(data);
				}
			}
		}
	}

	private class IndexedDictionary<TKey, TValue>
	{
		private class Entry
		{
			public TKey key;

			public TValue value;

			public Entry(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}
		}

		private List<Entry> list;

		public int Count => list.Count;

		public TValue this[int index] => list[index].value;

		public IndexedDictionary()
		{
			list = new List<Entry>();
		}

		public TValue Get(TKey key)
		{
			int num = IndexOfKey(key);
			if (num < 0)
			{
				throw new Exception("Key does not exist!");
			}
			return list[num].value;
		}

		public bool TryGet(TKey key, out TValue value)
		{
			value = default(TValue);
			int num = IndexOfKey(key);
			if (num < 0)
			{
				return false;
			}
			value = list[num].value;
			return true;
		}

		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				throw new Exception("Key " + key.ToString() + " is already in use!");
			}
			list.Add(new Entry(key, value));
		}

		public int IndexOfKey(TKey key)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
				{
					return i;
				}
			}
			return -1;
		}

		public bool ContainsKey(TKey key)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			list.Clear();
		}
	}

	private enum LayoutElementSizeType
	{
		MinSize,
		PreferredSize
	}

	private enum WindowType
	{
		None,
		ChooseJoystick,
		JoystickAssignmentConflict,
		ElementAssignment,
		ElementAssignmentPrePolling,
		ElementAssignmentPolling,
		ElementAssignmentResult,
		ElementAssignmentConflict,
		Calibration,
		CalibrateStep1,
		CalibrateStep2
	}

	private class InputGrid
	{
		private InputGridEntryList list;

		private List<GameObject> groups;

		public InputGrid()
		{
			list = new InputGridEntryList();
			groups = new List<GameObject>();
		}

		public void AddMapCategory(int mapCategoryId)
		{
			list.AddMapCategory(mapCategoryId);
		}

		public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			list.AddAction(mapCategoryId, action, axisRange);
		}

		public void AddActionCategory(int mapCategoryId, int actionCategoryId)
		{
			list.AddActionCategory(mapCategoryId, actionCategoryId);
		}

		public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
		}

		public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
		}

		public void AddGroup(GameObject group)
		{
			groups.Add(group);
		}

		public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
		}

		public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
		{
			list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
		}

		public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
		}

		public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
		{
			return list.GetActionSets(mapCategoryId);
		}

		public void SetColumnHeight(int mapCategoryId, float height)
		{
			list.SetColumnHeight(mapCategoryId, height);
		}

		public float GetColumnHeight(int mapCategoryId)
		{
			return list.GetColumnHeight(mapCategoryId);
		}

		public void SetFieldsActive(int mapCategoryId, bool state)
		{
			list.SetFieldsActive(mapCategoryId, state);
		}

		public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
		}

		public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
		}

		public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
		}

		public void InitializeFields(int mapCategoryId)
		{
			list.InitializeFields(mapCategoryId);
		}

		public void Show(int mapCategoryId)
		{
			list.Show(mapCategoryId);
		}

		public void HideAll()
		{
			list.HideAll();
		}

		public void ClearLabels(int mapCategoryId)
		{
			list.ClearLabels(mapCategoryId);
		}

		private void ClearGroups()
		{
			for (int i = 0; i < groups.Count; i++)
			{
				if (!(groups[i] == null))
				{
					UnityEngine.Object.Destroy(groups[i]);
				}
			}
		}

		public void ClearAll()
		{
			ClearGroups();
			list.Clear();
		}
	}

	private class InputGridEntryList
	{
		private class MapCategoryEntry
		{
			private List<ActionEntry> _actionList;

			private IndexedDictionary<int, ActionCategoryEntry> _actionCategoryList;

			private float _columnHeight;

			public List<ActionEntry> actionList => _actionList;

			public IndexedDictionary<int, ActionCategoryEntry> actionCategoryList => _actionCategoryList;

			public float columnHeight
			{
				get
				{
					return _columnHeight;
				}
				set
				{
					_columnHeight = value;
				}
			}

			public MapCategoryEntry()
			{
				_actionList = new List<ActionEntry>();
				_actionCategoryList = new IndexedDictionary<int, ActionCategoryEntry>();
			}

			public ActionEntry GetActionEntry(int actionId, AxisRange axisRange)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				int num = IndexOfActionEntry(actionId, axisRange);
				if (num < 0)
				{
					return null;
				}
				return _actionList[num];
			}

			public int IndexOfActionEntry(int actionId, AxisRange axisRange)
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				int count = _actionList.Count;
				for (int i = 0; i < count; i++)
				{
					if (_actionList[i].Matches(actionId, axisRange))
					{
						return i;
					}
				}
				return -1;
			}

			public bool ContainsActionEntry(int actionId, AxisRange axisRange)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return IndexOfActionEntry(actionId, axisRange) >= 0;
			}

			public ActionEntry AddAction(InputAction action, AxisRange axisRange)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (action == null)
				{
					return null;
				}
				if (ContainsActionEntry(action.id, axisRange))
				{
					return null;
				}
				_actionList.Add(new ActionEntry(action, axisRange));
				return _actionList[_actionList.Count - 1];
			}

			public ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId)
			{
				if (!_actionCategoryList.ContainsKey(actionCategoryId))
				{
					return null;
				}
				return _actionCategoryList.Get(actionCategoryId);
			}

			public ActionCategoryEntry AddActionCategory(int actionCategoryId)
			{
				if (actionCategoryId < 0)
				{
					return null;
				}
				if (_actionCategoryList.ContainsKey(actionCategoryId))
				{
					return null;
				}
				_actionCategoryList.Add(actionCategoryId, new ActionCategoryEntry(actionCategoryId));
				return _actionCategoryList.Get(actionCategoryId);
			}

			public void SetAllActive(bool state)
			{
				for (int i = 0; i < _actionCategoryList.Count; i++)
				{
					_actionCategoryList[i].SetActive(state);
				}
				for (int j = 0; j < _actionList.Count; j++)
				{
					_actionList[j].SetActive(state);
				}
			}
		}

		private class ActionEntry
		{
			private IndexedDictionary<int, FieldSet> fieldSets;

			public GUILabel label;

			public readonly InputAction action;

			public readonly AxisRange axisRange;

			public readonly InputActionSet actionSet;

			public ActionEntry(InputAction action, AxisRange axisRange)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				this.action = action;
				this.axisRange = axisRange;
				actionSet = new InputActionSet(action.id, axisRange);
				fieldSets = new IndexedDictionary<int, FieldSet>();
			}

			public void SetLabel(GUILabel label)
			{
				this.label = label;
			}

			public bool Matches(int actionId, AxisRange axisRange)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				if (action.id != actionId)
				{
					return false;
				}
				if (this.axisRange != axisRange)
				{
					return false;
				}
				return true;
			}

			public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Expected I4, but got Unknown
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					fieldSets.Add((int)controllerType, new FieldSet(fieldSetContainer));
				}
			}

			public void AddInputField(ControllerType controllerType, int fieldIndex, GUIInputField inputField)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Expected I4, but got Unknown
				if (fieldSets.ContainsKey((int)controllerType))
				{
					FieldSet fieldSet = fieldSets.Get((int)controllerType);
					if (!fieldSet.fields.ContainsKey(fieldIndex))
					{
						fieldSet.fields.Add(fieldIndex, inputField);
					}
				}
			}

			public GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected I4, but got Unknown
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Expected I4, but got Unknown
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					return null;
				}
				if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldIndex))
				{
					return null;
				}
				return fieldSets.Get((int)controllerType).fields.Get(fieldIndex);
			}

			public bool Contains(ControllerType controllerType, int fieldId)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected I4, but got Unknown
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					return false;
				}
				if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldId))
				{
					return false;
				}
				return true;
			}

			public void SetFieldLabel(ControllerType controllerType, int index, string label)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Expected I4, but got Unknown
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Expected I4, but got Unknown
				if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
				{
					fieldSets.Get((int)controllerType).fields.Get(index).SetLabel(label);
				}
			}

			public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Expected I4, but got Unknown
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Expected I4, but got Unknown
				if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
				{
					GUIInputField gUIInputField = fieldSets.Get((int)controllerType).fields.Get(index);
					gUIInputField.SetLabel(label);
					gUIInputField.actionElementMapId = actionElementMapId;
					gUIInputField.controllerId = controllerId;
					if (gUIInputField.hasToggle)
					{
						gUIInputField.toggle.SetInteractible(state: true, playTransition: false);
						gUIInputField.toggle.SetToggleState(invert);
						gUIInputField.toggle.actionElementMapId = actionElementMapId;
					}
				}
			}

			public void SetFixedFieldData(ControllerType controllerType, int controllerId)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Expected I4, but got Unknown
				if (fieldSets.ContainsKey((int)controllerType))
				{
					FieldSet fieldSet = fieldSets.Get((int)controllerType);
					int count = fieldSet.fields.Count;
					for (int i = 0; i < count; i++)
					{
						fieldSet.fields[i].controllerId = controllerId;
					}
				}
			}

			public void Initialize()
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						GUIInputField gUIInputField = fieldSet.fields[j];
						if (gUIInputField.hasToggle)
						{
							gUIInputField.toggle.SetInteractible(state: false, playTransition: false);
							gUIInputField.toggle.SetToggleState(state: false);
							gUIInputField.toggle.actionElementMapId = -1;
						}
						gUIInputField.SetLabel("");
						gUIInputField.actionElementMapId = -1;
						gUIInputField.controllerId = -1;
					}
				}
			}

			public void SetActive(bool state)
			{
				if (label != null)
				{
					label.SetActive(state);
				}
				int count = fieldSets.Count;
				for (int i = 0; i < count; i++)
				{
					fieldSets[i].groupContainer.SetActive(state);
				}
			}

			public void ClearLabels()
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						fieldSet.fields[j].SetLabel("");
					}
				}
			}

			public void SetFieldsActive(bool state)
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						GUIInputField gUIInputField = fieldSet.fields[j];
						gUIInputField.SetInteractible(state, playTransition: false);
						if (gUIInputField.hasToggle && (!state || gUIInputField.toggle.actionElementMapId >= 0))
						{
							gUIInputField.toggle.SetInteractible(state, playTransition: false);
						}
					}
				}
			}
		}

		private class FieldSet
		{
			public readonly GameObject groupContainer;

			public readonly IndexedDictionary<int, GUIInputField> fields;

			public FieldSet(GameObject groupContainer)
			{
				this.groupContainer = groupContainer;
				fields = new IndexedDictionary<int, GUIInputField>();
			}
		}

		private class ActionCategoryEntry
		{
			public readonly int actionCategoryId;

			public GUILabel label;

			public ActionCategoryEntry(int actionCategoryId)
			{
				this.actionCategoryId = actionCategoryId;
			}

			public void SetLabel(GUILabel label)
			{
				this.label = label;
			}

			public void SetActive(bool state)
			{
				if (label != null)
				{
					label.SetActive(state);
				}
			}
		}

		private IndexedDictionary<int, MapCategoryEntry> entries;

		public InputGridEntryList()
		{
			entries = new IndexedDictionary<int, MapCategoryEntry>();
		}

		public void AddMapCategory(int mapCategoryId)
		{
			if (mapCategoryId >= 0 && !entries.ContainsKey(mapCategoryId))
			{
				entries.Add(mapCategoryId, new MapCategoryEntry());
			}
		}

		public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			AddActionEntry(mapCategoryId, action, axisRange);
		}

		private ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (action == null)
			{
				return null;
			}
			if (!entries.TryGet(mapCategoryId, out var value))
			{
				return null;
			}
			return value.AddAction(action, axisRange);
		}

		public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (entries.TryGet(mapCategoryId, out var value))
			{
				value.GetActionEntry(actionId, axisRange)?.SetLabel(label);
			}
		}

		public void AddActionCategory(int mapCategoryId, int actionCategoryId)
		{
			AddActionCategoryEntry(mapCategoryId, actionCategoryId);
		}

		private ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId)
		{
			if (!entries.TryGet(mapCategoryId, out var value))
			{
				return null;
			}
			return value.AddActionCategory(actionCategoryId);
		}

		public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				value.GetActionCategoryEntry(actionCategoryId)?.SetLabel(label);
			}
		}

		public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GetActionEntry(mapCategoryId, action, axisRange)?.AddInputFieldSet(controllerType, fieldSetContainer);
		}

		public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GetActionEntry(mapCategoryId, action, axisRange)?.AddInputField(controllerType, fieldIndex, inputField);
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			return GetActionEntry(mapCategoryId, actionId, axisRange) != null;
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return GetActionEntry(mapCategoryId, actionId, axisRange)?.Contains(controllerType, fieldIndex) ?? false;
		}

		public void SetColumnHeight(int mapCategoryId, float height)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				value.columnHeight = height;
			}
		}

		public float GetColumnHeight(int mapCategoryId)
		{
			if (!entries.TryGet(mapCategoryId, out var value))
			{
				return 0f;
			}
			return value.columnHeight;
		}

		public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return GetActionEntry(mapCategoryId, actionId, axisRange)?.GetGUIInputField(controllerType, fieldIndex);
		}

		private ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (actionId < 0)
			{
				return null;
			}
			if (!entries.TryGet(mapCategoryId, out var value))
			{
				return null;
			}
			return value.GetActionEntry(actionId, axisRange);
		}

		private ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (action == null)
			{
				return null;
			}
			return GetActionEntry(mapCategoryId, action.id, axisRange);
		}

		public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				List<ActionEntry> list = value.actionList;
				int count = list?.Count ?? 0;
				for (int i = 0; i < count; i++)
				{
					yield return list[i].actionSet;
				}
			}
		}

		public void SetFieldsActive(int mapCategoryId, bool state)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				List<ActionEntry> actionList = value.actionList;
				int num = actionList?.Count ?? 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].SetFieldsActive(state);
				}
			}
		}

		public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GetActionEntry(mapCategoryId, actionId, axisRange)?.SetFieldLabel(controllerType, index, label);
		}

		public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GetActionEntry(mapCategoryId, actionId, axisRange)?.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
		}

		public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GetActionEntry(mapCategoryId, actionId, axisRange)?.SetFixedFieldData(controllerType, controllerId);
		}

		public void InitializeFields(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				List<ActionEntry> actionList = value.actionList;
				int num = actionList?.Count ?? 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].Initialize();
				}
			}
		}

		public void Show(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				value.SetAllActive(state: true);
			}
		}

		public void HideAll()
		{
			for (int i = 0; i < entries.Count; i++)
			{
				entries[i].SetAllActive(state: false);
			}
		}

		public void ClearLabels(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var value))
			{
				List<ActionEntry> actionList = value.actionList;
				int num = actionList?.Count ?? 0;
				for (int i = 0; i < num; i++)
				{
					actionList[i].ClearLabels();
				}
			}
		}

		public void Clear()
		{
			entries.Clear();
		}
	}

	private class WindowManager
	{
		private List<Window> windows;

		private GameObject windowPrefab;

		private Transform parent;

		private GameObject fader;

		private int idCounter;

		public bool isWindowOpen
		{
			get
			{
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (!(windows[num] == null))
					{
						return true;
					}
				}
				return false;
			}
		}

		public Window topWindow
		{
			get
			{
				for (int num = windows.Count - 1; num >= 0; num--)
				{
					if (!(windows[num] == null))
					{
						return windows[num];
					}
				}
				return null;
			}
		}

		public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent)
		{
			this.windowPrefab = windowPrefab;
			this.parent = parent;
			windows = new List<Window>();
			fader = UnityEngine.Object.Instantiate(faderPrefab);
			fader.transform.SetParent(parent, worldPositionStays: false);
			fader.GetComponent<RectTransform>().localScale = Vector2.one;
			SetFaderActive(state: false);
		}

		public Window OpenWindow(string name, int width, int height)
		{
			Window result = InstantiateWindow(name, width, height);
			UpdateFader();
			return result;
		}

		public Window OpenWindow(GameObject windowPrefab, string name)
		{
			if (windowPrefab == null)
			{
				Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
				return null;
			}
			Window result = InstantiateWindow(name, windowPrefab);
			UpdateFader();
			return result;
		}

		public void CloseTop()
		{
			int num = windows.Count - 1;
			while (num >= 0)
			{
				if (windows[num] == null)
				{
					windows.RemoveAt(num);
					num--;
					continue;
				}
				DestroyWindow(windows[num]);
				windows.RemoveAt(num);
				break;
			}
			UpdateFader();
		}

		public void CloseWindow(int windowId)
		{
			CloseWindow(GetWindow(windowId));
		}

		public void CloseWindow(Window window)
		{
			if (window == null)
			{
				return;
			}
			for (int num = windows.Count - 1; num >= 0; num--)
			{
				if (windows[num] == null)
				{
					windows.RemoveAt(num);
				}
				else if (!(windows[num] != window))
				{
					DestroyWindow(windows[num]);
					windows.RemoveAt(num);
					break;
				}
			}
			UpdateFader();
			FocusTopWindow();
		}

		public void CloseAll()
		{
			SetFaderActive(state: false);
			for (int num = windows.Count - 1; num >= 0; num--)
			{
				if (windows[num] == null)
				{
					windows.RemoveAt(num);
				}
				else
				{
					DestroyWindow(windows[num]);
					windows.RemoveAt(num);
				}
			}
			UpdateFader();
		}

		public void CancelAll()
		{
			if (!isWindowOpen)
			{
				return;
			}
			for (int num = windows.Count - 1; num >= 0; num--)
			{
				if (!(windows[num] == null))
				{
					windows[num].Cancel();
				}
			}
			CloseAll();
		}

		public Window GetWindow(int windowId)
		{
			if (windowId < 0)
			{
				return null;
			}
			for (int num = windows.Count - 1; num >= 0; num--)
			{
				if (!(windows[num] == null) && windows[num].id == windowId)
				{
					return windows[num];
				}
			}
			return null;
		}

		public bool IsFocused(int windowId)
		{
			if (windowId < 0)
			{
				return false;
			}
			if (topWindow == null)
			{
				return false;
			}
			return topWindow.id == windowId;
		}

		public void Focus(int windowId)
		{
			Focus(GetWindow(windowId));
		}

		public void Focus(Window window)
		{
			if (!(window == null))
			{
				window.TakeInputFocus();
				DefocusOtherWindows(window.id);
			}
		}

		private void DefocusOtherWindows(int focusedWindowId)
		{
			if (focusedWindowId < 0)
			{
				return;
			}
			for (int num = windows.Count - 1; num >= 0; num--)
			{
				if (!(windows[num] == null) && windows[num].id != focusedWindowId)
				{
					windows[num].Disable();
				}
			}
		}

		private void UpdateFader()
		{
			if (!isWindowOpen)
			{
				SetFaderActive(state: false);
			}
			else if (!(topWindow.transform.parent == null))
			{
				SetFaderActive(state: true);
				fader.transform.SetAsLastSibling();
				int siblingIndex = topWindow.transform.GetSiblingIndex();
				fader.transform.SetSiblingIndex(siblingIndex);
			}
		}

		private void FocusTopWindow()
		{
			if (!(topWindow == null))
			{
				topWindow.TakeInputFocus();
			}
		}

		private void SetFaderActive(bool state)
		{
			fader.SetActive(state);
		}

		private Window InstantiateWindow(string name, int width, int height)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "Window";
			}
			GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
			if (gameObject == null)
			{
				return null;
			}
			Window component = gameObject.GetComponent<Window>();
			if (component != null)
			{
				component.Initialize(GetNewId(), IsFocused);
				windows.Add(component);
				component.SetSize(width, height);
			}
			return component;
		}

		private Window InstantiateWindow(string name, GameObject windowPrefab)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "Window";
			}
			if (windowPrefab == null)
			{
				return null;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
			if (gameObject == null)
			{
				return null;
			}
			Window component = gameObject.GetComponent<Window>();
			if (component != null)
			{
				component.Initialize(GetNewId(), IsFocused);
				windows.Add(component);
			}
			return component;
		}

		private void DestroyWindow(Window window)
		{
			if (!(window == null))
			{
				UnityEngine.Object.Destroy(window.gameObject);
			}
		}

		private int GetNewId()
		{
			int result = idCounter;
			idCounter++;
			return result;
		}

		public void ClearCompletely()
		{
			CloseAll();
			if (fader != null)
			{
				UnityEngine.Object.Destroy(fader);
			}
		}
	}

	public const int versionMajor = 1;

	public const int versionMinor = 1;

	public const bool usesTMPro = false;

	private const float blockInputOnFocusTimeout = 0.1f;

	private const string buttonIdentifier_playerSelection = "PlayerSelection";

	private const string buttonIdentifier_removeController = "RemoveController";

	private const string buttonIdentifier_assignController = "AssignController";

	private const string buttonIdentifier_calibrateController = "CalibrateController";

	private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";

	private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";

	private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";

	private const string buttonIdentifier_done = "Done";

	private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

	[SerializeField]
	[Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
	private InputManager _rewiredInputManager;

	[SerializeField]
	[Tooltip("Set to True to prevent the Game Object from being destroyed when a new scene is loaded.\n\nNOTE: Changing this value from True to False at runtime will have no effect because Object.DontDestroyOnLoad cannot be undone once set.")]
	private bool _dontDestroyOnLoad;

	[SerializeField]
	[Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
	private bool _openOnStart;

	[SerializeField]
	[Tooltip("The Layout of the Keyboard Maps to be displayed.")]
	private int _keyboardMapDefaultLayout;

	[SerializeField]
	[Tooltip("The Layout of the Mouse Maps to be displayed.")]
	private int _mouseMapDefaultLayout;

	[SerializeField]
	[Tooltip("The Layout of the Mouse Maps to be displayed.")]
	private int _joystickMapDefaultLayout;

	[SerializeField]
	private MappingSet[] _mappingSets = new MappingSet[1] { MappingSet.Default };

	[SerializeField]
	[Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
	private bool _showPlayers = true;

	[SerializeField]
	[Tooltip("Display the Controller column for input mapping.")]
	private bool _showControllers = true;

	[SerializeField]
	[Tooltip("Display the Keyboard column for input mapping.")]
	private bool _showKeyboard = true;

	[SerializeField]
	[Tooltip("Display the Mouse column for input mapping.")]
	private bool _showMouse = true;

	[SerializeField]
	[Tooltip("The maximum number of controllers allowed to be assigned to a Player. If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. [0 = infinite]")]
	private int _maxControllersPerPlayer = 1;

	[SerializeField]
	[Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
	private bool _showActionCategoryLabels;

	[SerializeField]
	[Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _keyboardInputFieldCount = 2;

	[SerializeField]
	[Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _mouseInputFieldCount = 1;

	[SerializeField]
	[Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _controllerInputFieldCount = 1;

	[SerializeField]
	[Tooltip("Display a full-axis input assignment field for every axis-type Action in the input field grid. Also displays an invert toggle for the user  to invert the full-axis assignment direction.\n\n*IMPORTANT*: This field is required if you have made any full-axis assignments in the Rewired Input Manager or in saved XML user data. Disabling this field when you have full-axis assignments will result in the inability for the user to view, remove, or modify these full-axis assignments. In addition, these assignments may cause conflicts when trying to remap the same axes to Actions.")]
	private bool _showFullAxisInputFields = true;

	[SerializeField]
	[Tooltip("Display a positive and negative input assignment field for every axis-type Action in the input field grid.\n\n*IMPORTANT*: These fields are required to assign buttons, keyboard keys, and hat or D-Pad directions to axis-type Actions. If you have made any split-axis assignments or button/key/D-pad assignments to axis-type Actions in the Rewired Input Manager or in saved XML user data, disabling these fields will result in the inability for the user to view, remove, or modify these assignments. In addition, these assignments may cause conflicts when trying to remap the same elements to Actions.")]
	private bool _showSplitAxisInputFields = true;

	[SerializeField]
	[Tooltip("Show glyphs if available. Glyph Provider must be configured for glyphs to be displayed. See Glyphs documentation for more information.")]
	private bool _showGlyphs = true;

	[SerializeField]
	[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
	private bool _allowElementAssignmentConflicts;

	[SerializeField]
	[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to swap conflicting assignments. This only applies to the first conflicting assignment found. This option will not be displayed if allowElementAssignmentConflicts is true.")]
	private bool _allowElementAssignmentSwap;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Action label column.")]
	private int _actionLabelWidth = 200;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Keyboard column.")]
	private int _keyboardColMaxWidth = 360;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Mouse column.")]
	private int _mouseColMaxWidth = 200;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Controller column.")]
	private int _controllerColMaxWidth = 200;

	[SerializeField]
	[Tooltip("The height in relative pixels of the input grid button rows.")]
	private int _inputRowHeight = 40;

	[SerializeField]
	[Tooltip("The padding of the input grid button rows.")]
	private RectOffset _inputRowPadding = new RectOffset();

	[SerializeField]
	[Tooltip("The width in relative pixels of spacing between input fields in a single column.")]
	private int _inputRowFieldSpacing;

	[SerializeField]
	[Tooltip("The width in relative pixels of spacing between columns.")]
	private int _inputColumnSpacing = 40;

	[SerializeField]
	[Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
	private int _inputRowCategorySpacing = 20;

	[SerializeField]
	[Tooltip("The width in relative pixels of the invert toggle buttons.")]
	private int _invertToggleWidth = 40;

	[SerializeField]
	[Tooltip("The width in relative pixels of generated popup windows.")]
	private int _defaultWindowWidth = 500;

	[SerializeField]
	[Tooltip("The height in relative pixels of generated popup windows.")]
	private int _defaultWindowHeight = 400;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
	private float _controllerAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
	private float _preInputAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
	private float _inputAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
	private float _axisCalibrationTimeout = 5f;

	[SerializeField]
	[Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
	private bool _ignoreMouseXAxisAssignment = true;

	[SerializeField]
	[Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
	private bool _ignoreMouseYAxisAssignment = true;

	[SerializeField]
	[Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
	private int _screenToggleAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will open the main screen if it is closed.")]
	private int _screenOpenAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
	private int _screenCloseAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
	private int _universalCancelAction = -1;

	[SerializeField]
	[Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
	private bool _universalCancelClosesScreen = true;

	[SerializeField]
	[Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
	private bool _showInputBehaviorSettings;

	[SerializeField]
	[Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
	private InputBehaviorSettings[] _inputBehaviorSettings;

	[SerializeField]
	[Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
	private bool _useThemeSettings = true;

	[SerializeField]
	[Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
	private ThemeSettings _themeSettings;

	[SerializeField]
	[Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
	private LanguageDataBase _language;

	[SerializeField]
	[Tooltip("A list of prefabs. You should not have to modify this.")]
	private Prefabs prefabs;

	[SerializeField]
	[Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
	private References references;

	[SerializeField]
	[Tooltip("Show the label for the Players button group?")]
	private bool _showPlayersGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Controller button group?")]
	private bool _showControllerGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Assigned Controllers button group?")]
	private bool _showAssignedControllersGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Settings button group?")]
	private bool _showSettingsGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Map Categories button group?")]
	private bool _showMapCategoriesGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the current controller name?")]
	private bool _showControllerNameLabel = true;

	[SerializeField]
	[Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
	private bool _showAssignedControllers = true;

	private Action _ScreenClosedEvent;

	private Action _ScreenOpenedEvent;

	private Action _PopupWindowOpenedEvent;

	private Action _PopupWindowClosedEvent;

	private Action _InputPollingStartedEvent;

	private Action _InputPollingEndedEvent;

	private Action _ThemeAppliedEvent;

	[SerializeField]
	[Tooltip("Event sent when the UI is closed.")]
	private UnityEvent _onScreenClosed;

	[SerializeField]
	[Tooltip("Event sent when the UI is opened.")]
	private UnityEvent _onScreenOpened;

	[SerializeField]
	[Tooltip("Event sent when a popup window is closed.")]
	private UnityEvent _onPopupWindowClosed;

	[SerializeField]
	[Tooltip("Event sent when a popup window is opened.")]
	private UnityEvent _onPopupWindowOpened;

	[SerializeField]
	[Tooltip("Event sent when polling for input has started.")]
	private UnityEvent _onInputPollingStarted;

	[SerializeField]
	[Tooltip("Event sent when polling for input has ended.")]
	private UnityEvent _onInputPollingEnded;

	private static ControlMapper Instance;

	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private int playerCount;

	private InputGrid inputGrid;

	private WindowManager windowManager;

	[NonSerialized]
	private int currentPlayerId;

	[NonSerialized]
	private int currentMapCategoryId;

	[NonSerialized]
	private List<GUIButton> playerButtons;

	[NonSerialized]
	private List<GUIButton> mapCategoryButtons;

	[NonSerialized]
	private List<GUIButton> assignedControllerButtons;

	private GUIButton assignedControllerButtonsPlaceholder;

	[NonSerialized]
	private List<GameObject> miscInstantiatedObjects;

	[NonSerialized]
	private GameObject canvas;

	[NonSerialized]
	private GameObject lastUISelection;

	[NonSerialized]
	private int currentJoystickId = -1;

	[NonSerialized]
	private float blockInputOnFocusEndTime;

	[NonSerialized]
	private bool isPollingForInput;

	[NonSerialized]
	private List<ThemedElement> themedElements = new List<ThemedElement>();

	[NonSerialized]
	private InputMapping pendingInputMapping;

	[NonSerialized]
	private AxisCalibrator pendingAxisCalibration;

	private Action<InputFieldInfo> inputFieldActivatedDelegate;

	private Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

	private Action _restoreDefaultsDelegate;

	public InputManager rewiredInputManager
	{
		get
		{
			return _rewiredInputManager;
		}
		set
		{
			_rewiredInputManager = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool dontDestroyOnLoad
	{
		get
		{
			return _dontDestroyOnLoad;
		}
		set
		{
			if (value != _dontDestroyOnLoad && value)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			}
			_dontDestroyOnLoad = value;
		}
	}

	public int keyboardMapDefaultLayout
	{
		get
		{
			return _keyboardMapDefaultLayout;
		}
		set
		{
			_keyboardMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseMapDefaultLayout
	{
		get
		{
			return _mouseMapDefaultLayout;
		}
		set
		{
			_mouseMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int joystickMapDefaultLayout
	{
		get
		{
			return _joystickMapDefaultLayout;
		}
		set
		{
			_joystickMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showPlayers
	{
		get
		{
			if (_showPlayers)
			{
				return ReInput.players.playerCount > 1;
			}
			return false;
		}
		set
		{
			_showPlayers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllers
	{
		get
		{
			return _showControllers;
		}
		set
		{
			_showControllers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showKeyboard
	{
		get
		{
			return _showKeyboard;
		}
		set
		{
			_showKeyboard = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showMouse
	{
		get
		{
			return _showMouse;
		}
		set
		{
			_showMouse = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int maxControllersPerPlayer
	{
		get
		{
			return _maxControllersPerPlayer;
		}
		set
		{
			_maxControllersPerPlayer = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showActionCategoryLabels
	{
		get
		{
			return _showActionCategoryLabels;
		}
		set
		{
			_showActionCategoryLabels = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int keyboardInputFieldCount
	{
		get
		{
			return _keyboardInputFieldCount;
		}
		set
		{
			_keyboardInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseInputFieldCount
	{
		get
		{
			return _mouseInputFieldCount;
		}
		set
		{
			_mouseInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int controllerInputFieldCount
	{
		get
		{
			return _controllerInputFieldCount;
		}
		set
		{
			_controllerInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showFullAxisInputFields
	{
		get
		{
			return _showFullAxisInputFields;
		}
		set
		{
			_showFullAxisInputFields = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showSplitAxisInputFields
	{
		get
		{
			return _showSplitAxisInputFields;
		}
		set
		{
			_showSplitAxisInputFields = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showGlyphs
	{
		get
		{
			return _showGlyphs;
		}
		set
		{
			_showGlyphs = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool allowElementAssignmentConflicts
	{
		get
		{
			return _allowElementAssignmentConflicts;
		}
		set
		{
			_allowElementAssignmentConflicts = value;
			InspectorPropertyChanged();
		}
	}

	public bool allowElementAssignmentSwap
	{
		get
		{
			return _allowElementAssignmentSwap;
		}
		set
		{
			_allowElementAssignmentSwap = value;
			InspectorPropertyChanged();
		}
	}

	public int actionLabelWidth
	{
		get
		{
			return _actionLabelWidth;
		}
		set
		{
			_actionLabelWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int keyboardColMaxWidth
	{
		get
		{
			return _keyboardColMaxWidth;
		}
		set
		{
			_keyboardColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseColMaxWidth
	{
		get
		{
			return _mouseColMaxWidth;
		}
		set
		{
			_mouseColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int controllerColMaxWidth
	{
		get
		{
			return _controllerColMaxWidth;
		}
		set
		{
			_controllerColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputRowHeight
	{
		get
		{
			return _inputRowHeight;
		}
		set
		{
			_inputRowHeight = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputColumnSpacing
	{
		get
		{
			return _inputColumnSpacing;
		}
		set
		{
			_inputColumnSpacing = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputRowCategorySpacing
	{
		get
		{
			return _inputRowCategorySpacing;
		}
		set
		{
			_inputRowCategorySpacing = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int invertToggleWidth
	{
		get
		{
			return _invertToggleWidth;
		}
		set
		{
			_invertToggleWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int defaultWindowWidth
	{
		get
		{
			return _defaultWindowWidth;
		}
		set
		{
			_defaultWindowWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int defaultWindowHeight
	{
		get
		{
			return _defaultWindowHeight;
		}
		set
		{
			_defaultWindowHeight = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public float controllerAssignmentTimeout
	{
		get
		{
			return _controllerAssignmentTimeout;
		}
		set
		{
			_controllerAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float preInputAssignmentTimeout
	{
		get
		{
			return _preInputAssignmentTimeout;
		}
		set
		{
			_preInputAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float inputAssignmentTimeout
	{
		get
		{
			return _inputAssignmentTimeout;
		}
		set
		{
			_inputAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float axisCalibrationTimeout
	{
		get
		{
			return _axisCalibrationTimeout;
		}
		set
		{
			_axisCalibrationTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public bool ignoreMouseXAxisAssignment
	{
		get
		{
			return _ignoreMouseXAxisAssignment;
		}
		set
		{
			_ignoreMouseXAxisAssignment = value;
			InspectorPropertyChanged();
		}
	}

	public bool ignoreMouseYAxisAssignment
	{
		get
		{
			return _ignoreMouseYAxisAssignment;
		}
		set
		{
			_ignoreMouseYAxisAssignment = value;
			InspectorPropertyChanged();
		}
	}

	public bool universalCancelClosesScreen
	{
		get
		{
			return _universalCancelClosesScreen;
		}
		set
		{
			_universalCancelClosesScreen = value;
			InspectorPropertyChanged();
		}
	}

	public bool showInputBehaviorSettings
	{
		get
		{
			return _showInputBehaviorSettings;
		}
		set
		{
			_showInputBehaviorSettings = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool useThemeSettings
	{
		get
		{
			return _useThemeSettings;
		}
		set
		{
			_useThemeSettings = value;
			InspectorPropertyChanged(reset: true);
			if (value)
			{
				ApplyTheme();
			}
		}
	}

	public ThemeSettings themeSettings
	{
		get
		{
			return _themeSettings;
		}
		set
		{
			_themeSettings = value;
			InspectorPropertyChanged(reset: true);
			ApplyTheme();
		}
	}

	public LanguageDataBase language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
			if (_language != null)
			{
				_language.Initialize();
			}
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showPlayersGroupLabel
	{
		get
		{
			return _showPlayersGroupLabel;
		}
		set
		{
			_showPlayersGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllerGroupLabel
	{
		get
		{
			return _showControllerGroupLabel;
		}
		set
		{
			_showControllerGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showAssignedControllersGroupLabel
	{
		get
		{
			return _showAssignedControllersGroupLabel;
		}
		set
		{
			_showAssignedControllersGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showSettingsGroupLabel
	{
		get
		{
			return _showSettingsGroupLabel;
		}
		set
		{
			_showSettingsGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showMapCategoriesGroupLabel
	{
		get
		{
			return _showMapCategoriesGroupLabel;
		}
		set
		{
			_showMapCategoriesGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllerNameLabel
	{
		get
		{
			return _showControllerNameLabel;
		}
		set
		{
			_showControllerNameLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showAssignedControllers
	{
		get
		{
			return _showAssignedControllers;
		}
		set
		{
			_showAssignedControllers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public Action restoreDefaultsDelegate
	{
		get
		{
			return _restoreDefaultsDelegate;
		}
		set
		{
			_restoreDefaultsDelegate = value;
		}
	}

	public bool isOpen
	{
		get
		{
			if (!initialized)
			{
				if (!(references.canvas != null))
				{
					return false;
				}
				return references.canvas.gameObject.activeInHierarchy;
			}
			return canvas.activeInHierarchy;
		}
	}

	private bool isFocused
	{
		get
		{
			if (!initialized)
			{
				return false;
			}
			return !windowManager.isWindowOpen;
		}
	}

	private bool inputAllowed
	{
		get
		{
			if (blockInputOnFocusEndTime > Time.unscaledTime)
			{
				return false;
			}
			return true;
		}
	}

	private int inputGridColumnCount
	{
		get
		{
			int num = 1;
			if (_showKeyboard)
			{
				num++;
			}
			if (_showMouse)
			{
				num++;
			}
			if (_showControllers)
			{
				num++;
			}
			return num;
		}
	}

	private int inputGridWidth => _actionLabelWidth + (_showKeyboard ? _keyboardColMaxWidth : 0) + (_showMouse ? _mouseColMaxWidth : 0) + (_showControllers ? _controllerColMaxWidth : 0) + (inputGridColumnCount - 1) * _inputColumnSpacing;

	private Player currentPlayer => ReInput.players.GetPlayer(currentPlayerId);

	private InputCategory currentMapCategory => (InputCategory)(object)ReInput.mapping.GetMapCategory(currentMapCategoryId);

	private MappingSet currentMappingSet
	{
		get
		{
			if (currentMapCategoryId < 0)
			{
				return null;
			}
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				if (_mappingSets[i].mapCategoryId == currentMapCategoryId)
				{
					return _mappingSets[i];
				}
			}
			return null;
		}
	}

	private Joystick currentJoystick => ReInput.controllers.GetJoystick(currentJoystickId);

	private bool isJoystickSelected => currentJoystickId >= 0;

	private GameObject currentUISelection
	{
		get
		{
			if (!(EventSystem.current != null))
			{
				return null;
			}
			return EventSystem.current.currentSelectedGameObject;
		}
	}

	private bool showSettings
	{
		get
		{
			if (_showInputBehaviorSettings)
			{
				return _inputBehaviorSettings.Length != 0;
			}
			return false;
		}
	}

	private bool showMapCategories
	{
		get
		{
			if (_mappingSets == null)
			{
				return false;
			}
			if (_mappingSets.Length <= 1)
			{
				return false;
			}
			return true;
		}
	}

	public static ControlMapper current => Instance;

	public event Action ScreenClosedEvent
	{
		add
		{
			_ScreenClosedEvent = (Action)Delegate.Combine(_ScreenClosedEvent, value);
		}
		remove
		{
			_ScreenClosedEvent = (Action)Delegate.Remove(_ScreenClosedEvent, value);
		}
	}

	public event Action ScreenOpenedEvent
	{
		add
		{
			_ScreenOpenedEvent = (Action)Delegate.Combine(_ScreenOpenedEvent, value);
		}
		remove
		{
			_ScreenOpenedEvent = (Action)Delegate.Remove(_ScreenOpenedEvent, value);
		}
	}

	public event Action PopupWindowClosedEvent
	{
		add
		{
			_PopupWindowClosedEvent = (Action)Delegate.Combine(_PopupWindowClosedEvent, value);
		}
		remove
		{
			_PopupWindowClosedEvent = (Action)Delegate.Remove(_PopupWindowClosedEvent, value);
		}
	}

	public event Action PopupWindowOpenedEvent
	{
		add
		{
			_PopupWindowOpenedEvent = (Action)Delegate.Combine(_PopupWindowOpenedEvent, value);
		}
		remove
		{
			_PopupWindowOpenedEvent = (Action)Delegate.Remove(_PopupWindowOpenedEvent, value);
		}
	}

	public event Action InputPollingStartedEvent
	{
		add
		{
			_InputPollingStartedEvent = (Action)Delegate.Combine(_InputPollingStartedEvent, value);
		}
		remove
		{
			_InputPollingStartedEvent = (Action)Delegate.Remove(_InputPollingStartedEvent, value);
		}
	}

	public event Action InputPollingEndedEvent
	{
		add
		{
			_InputPollingEndedEvent = (Action)Delegate.Combine(_InputPollingEndedEvent, value);
		}
		remove
		{
			_InputPollingEndedEvent = (Action)Delegate.Remove(_InputPollingEndedEvent, value);
		}
	}

	public event UnityAction onScreenClosed
	{
		add
		{
			_onScreenClosed.AddListener(value);
		}
		remove
		{
			_onScreenClosed.RemoveListener(value);
		}
	}

	public event UnityAction onScreenOpened
	{
		add
		{
			_onScreenOpened.AddListener(value);
		}
		remove
		{
			_onScreenOpened.RemoveListener(value);
		}
	}

	public event UnityAction onPopupWindowClosed
	{
		add
		{
			_onPopupWindowClosed.AddListener(value);
		}
		remove
		{
			_onPopupWindowClosed.RemoveListener(value);
		}
	}

	public event UnityAction onPopupWindowOpened
	{
		add
		{
			_onPopupWindowOpened.AddListener(value);
		}
		remove
		{
			_onPopupWindowOpened.RemoveListener(value);
		}
	}

	public event UnityAction onInputPollingStarted
	{
		add
		{
			_onInputPollingStarted.AddListener(value);
		}
		remove
		{
			_onInputPollingStarted.RemoveListener(value);
		}
	}

	public event UnityAction onInputPollingEnded
	{
		add
		{
			_onInputPollingEnded.AddListener(value);
		}
		remove
		{
			_onInputPollingEnded.RemoveListener(value);
		}
	}

	private void Awake()
	{
		if (_dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		}
		PreInitialize();
		if (isOpen)
		{
			Initialize();
			Open(force: true);
		}
	}

	private void Start()
	{
		if (_openOnStart)
		{
			Open(force: false);
		}
	}

	private void Update()
	{
		if (isOpen && initialized)
		{
			CheckUISelection();
		}
	}

	private void OnDestroy()
	{
		ReInput.ControllerConnectedEvent -= OnJoystickConnected;
		ReInput.ControllerDisconnectedEvent -= OnJoystickDisconnected;
		ReInput.ControllerPreDisconnectEvent -= OnJoystickPreDisconnect;
		UnsubscribeMenuControlInputEvents();
	}

	private void PreInitialize()
	{
		if (!ReInput.isReady)
		{
			Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
		}
		else
		{
			SubscribeMenuControlInputEvents();
		}
	}

	private void Initialize()
	{
		if (initialized || !ReInput.isReady)
		{
			return;
		}
		if ((UnityEngine.Object)(object)_rewiredInputManager == null)
		{
			_rewiredInputManager = UnityEngine.Object.FindFirstObjectByType<InputManager>();
			if ((UnityEngine.Object)(object)_rewiredInputManager == null)
			{
				Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
				return;
			}
		}
		if (Instance != null)
		{
			Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
			return;
		}
		Instance = this;
		if (prefabs == null || !prefabs.Check())
		{
			Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
			return;
		}
		if (references == null || !references.Check())
		{
			Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
			return;
		}
		references.inputGridLayoutElement = references.inputGridContainer.GetComponent<LayoutElement>();
		if (references.inputGridLayoutElement == null)
		{
			Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
			return;
		}
		if (_showKeyboard && _keyboardInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
			_keyboardInputFieldCount = 1;
		}
		if (_showMouse && _mouseInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
			_mouseInputFieldCount = 1;
		}
		if (_showControllers && _controllerInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
			_controllerInputFieldCount = 1;
		}
		if (_maxControllersPerPlayer < 0)
		{
			Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
			_maxControllersPerPlayer = 0;
		}
		if (_useThemeSettings && _themeSettings == null)
		{
			Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
			_useThemeSettings = false;
		}
		if (_language == null)
		{
			Debug.LogError("Rawired UI: Language must be set in the inspector!");
			return;
		}
		_language.Initialize();
		inputFieldActivatedDelegate = OnInputFieldActivated;
		inputFieldInvertToggleStateChangedDelegate = OnInputFieldInvertToggleStateChanged;
		ReInput.ControllerConnectedEvent += OnJoystickConnected;
		ReInput.ControllerDisconnectedEvent += OnJoystickDisconnected;
		ReInput.ControllerPreDisconnectEvent += OnJoystickPreDisconnect;
		playerCount = ReInput.players.playerCount;
		canvas = references.canvas.gameObject;
		windowManager = new WindowManager(prefabs.window, prefabs.fader, references.canvas.transform);
		playerButtons = new List<GUIButton>();
		mapCategoryButtons = new List<GUIButton>();
		assignedControllerButtons = new List<GUIButton>();
		miscInstantiatedObjects = new List<GameObject>();
		List<MappingSet> list = new List<MappingSet>(_mappingSets.Length);
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet mappingSet = _mappingSets[i];
			if (mappingSet != null && mappingSet.isValid)
			{
				list.Add(mappingSet);
			}
		}
		_mappingSets = list.ToArray();
		currentMapCategoryId = _mappingSets[0].mapCategoryId;
		Draw();
		CreateInputGrid();
		CreateLayout();
		SubscribeFixedUISelectionEvents();
		initialized = true;
	}

	private void OnJoystickConnected(ControllerStatusChangedEventArgs args)
	{
		if (initialized && _showControllers)
		{
			ClearVarsOnJoystickChange();
			ForceRefresh();
		}
	}

	private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (initialized && _showControllers)
		{
			ClearVarsOnJoystickChange();
			ForceRefresh();
		}
	}

	private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args)
	{
		if (initialized)
		{
			_ = _showControllers;
		}
	}

	public void OnButtonActivated(ButtonInfo buttonInfo)
	{
		if (initialized && inputAllowed)
		{
			switch (buttonInfo.identifier)
			{
			case "PlayerSelection":
				OnPlayerSelected(buttonInfo.intData, redraw: true);
				break;
			case "AssignedControllerSelection":
				OnControllerSelected(buttonInfo.intData);
				break;
			case "RemoveController":
				OnRemoveCurrentController();
				break;
			case "AssignController":
				ShowAssignControllerWindow();
				break;
			case "CalibrateController":
				ShowCalibrateControllerWindow();
				break;
			case "EditInputBehaviors":
				ShowEditInputBehaviorsWindow();
				break;
			case "MapCategorySelection":
				OnMapCategorySelected(buttonInfo.intData, redraw: true);
				break;
			case "Done":
				Close(save: true);
				break;
			case "RestoreDefaults":
				OnRestoreDefaults();
				break;
			}
		}
	}

	public void OnInputFieldActivated(InputFieldInfo fieldInfo)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized || !inputAllowed || currentPlayer == null)
		{
			return;
		}
		InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
		if (action == null)
		{
			return;
		}
		AxisRange axisRange = (AxisRange)(((int)action.type == 0) ? ((int)fieldInfo.axisRange) : 0);
		string actionName = _language.GetActionName(action.id, axisRange);
		ControllerMap controllerMap = GetControllerMap(fieldInfo.controllerType);
		if (controllerMap != null)
		{
			ActionElementMap val = ((fieldInfo.actionElementMapId >= 0) ? controllerMap.GetElementMap(fieldInfo.actionElementMapId) : null);
			if (val != null)
			{
				ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, controllerMap, val, actionName);
			}
			else
			{
				ShowCreateNewElementAssignmentWindow(fieldInfo, action, controllerMap, actionName);
			}
		}
	}

	public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (initialized && inputAllowed)
		{
			SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
		}
	}

	private void OnPlayerSelected(int playerId, bool redraw)
	{
		if (initialized)
		{
			currentPlayerId = playerId;
			ClearVarsOnPlayerChange();
			if (redraw)
			{
				Redraw(listsChanged: true, playTransitions: true);
			}
		}
	}

	private void OnControllerSelected(int joystickId)
	{
		if (initialized)
		{
			currentJoystickId = joystickId;
			Redraw(listsChanged: true, playTransitions: true);
		}
	}

	private void OnRemoveCurrentController()
	{
		if (currentPlayer != null && currentJoystickId >= 0)
		{
			RemoveController(currentPlayer, currentJoystickId);
			ClearVarsOnJoystickChange();
			Redraw(listsChanged: false, playTransitions: false);
		}
	}

	private void OnMapCategorySelected(int id, bool redraw)
	{
		if (initialized)
		{
			currentMapCategoryId = id;
			if (redraw)
			{
				Redraw(listsChanged: true, playTransitions: true);
			}
		}
	}

	private void OnRestoreDefaults()
	{
		if (initialized)
		{
			ShowRestoreDefaultsWindow();
		}
	}

	private void OnScreenToggleActionPressed(InputActionEventData data)
	{
		if (!isOpen)
		{
			Open();
		}
		else if (initialized && isFocused)
		{
			Close(save: true);
		}
	}

	private void OnScreenOpenActionPressed(InputActionEventData data)
	{
		Open();
	}

	private void OnScreenCloseActionPressed(InputActionEventData data)
	{
		if (initialized && isOpen && isFocused)
		{
			Close(save: true);
		}
	}

	private void OnUniversalCancelActionPressed(InputActionEventData data)
	{
		if (!initialized || !isOpen)
		{
			return;
		}
		if (_universalCancelClosesScreen)
		{
			if (isFocused)
			{
				Close(save: true);
				return;
			}
		}
		else if (isFocused)
		{
			return;
		}
		CloseAllWindows();
	}

	private void OnWindowCancel(int windowId)
	{
		if (initialized && windowId >= 0)
		{
			CloseWindow(windowId);
		}
	}

	private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem)
	{
		if (map != null && aem != null)
		{
			map.DeleteElementMap(aem.id);
			CloseWindow(windowId);
		}
	}

	private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected I4, but got Unknown
		if (!(fieldInfo == null) && map != null)
		{
			pendingInputMapping = new InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);
			ControllerType controllerType = fieldInfo.controllerType;
			switch ((int)controllerType)
			{
			case 2:
				ShowElementAssignmentPrePollingWindow();
				break;
			case 0:
				ShowElementAssignmentPollingWindow();
				break;
			case 1:
				ShowElementAssignmentPollingWindow();
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}

	private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId)
	{
		if (windowId >= 0 && player != null && controllerId >= 0)
		{
			AssignController(player, controllerId);
			CloseWindow(windowId);
		}
	}

	private void OnMouseAssignmentConfirmed(int windowId, Player player)
	{
		if (windowId < 0 || player == null)
		{
			return;
		}
		IList<Player> players = ReInput.players.Players;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != player)
			{
				players[i].controllers.hasMouse = false;
			}
		}
		player.controllers.hasMouse = true;
		CloseWindow(windowId);
	}

	private void OnElementAssignmentConflictReplaceConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers, bool allowSwap)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null || mapping == null)
		{
			return;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
			CloseWindow(windowId);
			return;
		}
		ElementAssignmentConflictInfo conflict = default(ElementAssignmentConflictInfo);
		ActionElementMap val = null;
		ActionElementMap val2 = null;
		bool flag = false;
		if (allowSwap && mapping.aem != null && GetFirstElementAssignmentConflict(conflictCheck, out conflict, skipOtherPlayers))
		{
			flag = true;
			val2 = new ActionElementMap(mapping.aem);
			val = new ActionElementMap(((ElementAssignmentConflictInfo)(ref conflict)).elementMap);
		}
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			Player val3 = allPlayers[i];
			if (!skipOtherPlayers || val3 == currentPlayer || val3 == ReInput.players.SystemPlayer)
			{
				val3.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
		}
		mapping.map.ReplaceOrCreateElementMap(assignment);
		if (allowSwap && flag)
		{
			int actionId = val.actionId;
			Pole axisContribution = val.axisContribution;
			bool flag2 = val.invert;
			AxisRange val4 = val2.axisRange;
			ControllerElementType elementType = val2.elementType;
			int elementIdentifierId = val2.elementIdentifierId;
			KeyCode keyCode = val2.keyCode;
			ModifierKeyFlags modifierKeyFlags = val2.modifierKeyFlags;
			if (elementType == val.elementType && (int)elementType == 0)
			{
				if (val4 != val.axisRange)
				{
					if ((int)val4 == 0)
					{
						val4 = (AxisRange)1;
					}
					else if ((int)val.axisRange != 0)
					{
					}
				}
			}
			else if ((int)elementType == 0 && ((int)val.elementType == 1 || ((int)val.elementType == 0 && (int)val.axisRange != 0)) && (int)val4 == 0)
			{
				val4 = (AxisRange)1;
			}
			if ((int)elementType != 0 || (int)val4 != 0)
			{
				flag2 = false;
			}
			int num = 0;
			foreach (ActionElementMap item in ((ElementAssignmentConflictInfo)(ref conflict)).controllerMap.ElementMapsWithAction(actionId))
			{
				if (SwapIsSameInputRange(elementType, val4, axisContribution, item.elementType, item.axisRange, item.axisContribution))
				{
					num++;
				}
			}
			if (num < GetControllerInputFieldCount(mapping.controllerType))
			{
				((ElementAssignmentConflictInfo)(ref conflict)).controllerMap.ReplaceOrCreateElementMap(ElementAssignment.CompleteAssignment(mapping.controllerType, elementType, elementIdentifierId, val4, keyCode, modifierKeyFlags, actionId, axisContribution, flag2));
			}
		}
		CloseWindow(windowId);
	}

	private void OnElementAssignmentAddConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer != null && mapping != null)
		{
			mapping.map.ReplaceOrCreateElementMap(assignment);
			CloseWindow(windowId);
		}
	}

	private void OnRestoreDefaultsConfirmed(int windowId)
	{
		if (_restoreDefaultsDelegate == null)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player val = players[i];
				if (_showControllers)
				{
					val.controllers.maps.LoadDefaultMaps((ControllerType)2);
				}
				if (_showKeyboard)
				{
					val.controllers.maps.LoadDefaultMaps((ControllerType)0);
				}
				if (_showMouse)
				{
					val.controllers.maps.LoadDefaultMaps((ControllerType)1);
				}
			}
		}
		CloseWindow(windowId);
		if (_restoreDefaultsDelegate != null)
		{
			_restoreDefaultsDelegate();
		}
	}

	private void OnAssignControllerWindowUpdate(int windowId)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		ControllerPollingInfo val = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown((ControllerType)2);
		if (((ControllerPollingInfo)(ref val)).success)
		{
			InputPollingStopped();
			if (ReInput.controllers.IsControllerAssigned((ControllerType)2, ((ControllerPollingInfo)(ref val)).controllerId) && !currentPlayer.controllers.ContainsController((ControllerType)2, ((ControllerPollingInfo)(ref val)).controllerId))
			{
				ShowControllerAssignmentConflictWindow(((ControllerPollingInfo)(ref val)).controllerId);
			}
			else
			{
				OnControllerAssignmentConfirmed(windowId, currentPlayer, ((ControllerPollingInfo)(ref val)).controllerId);
			}
		}
		else
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		}
	}

	private void OnElementAssignmentPrePollingWindowUpdate(int windowId)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			ControllerType controllerType = pendingInputMapping.controllerType;
			ControllerPollingInfo val;
			if ((int)controllerType > 1)
			{
				if ((int)controllerType != 2)
				{
					throw new NotImplementedException();
				}
				if (currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				val = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, ((Controller)currentJoystick).id);
			}
			else
			{
				val = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, 0);
			}
			if (!((ControllerPollingInfo)(ref val)).success)
			{
				return;
			}
		}
		ShowElementAssignmentPollingWindow();
	}

	private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		if (currentPlayer.controllers.joystickCount == 0)
		{
			return;
		}
		ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown((ControllerType)2, ((Controller)currentJoystick).id);
		if (((ControllerPollingInfo)(ref pollingInfo)).success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment val = pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, val, skipOtherPlayers: false))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(val);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(val, skipOtherPlayers: false);
			}
		}
	}

	private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		PollKeyboardForAssignment(out var pollingInfo, out var modifierKeyPressed, out var modifierFlags, out var label);
		if (modifierKeyPressed)
		{
			window.timer.Start(_inputAssignmentTimeout);
		}
		window.SetContentText(modifierKeyPressed ? string.Empty : Mathf.CeilToInt(window.timer.remaining).ToString(), 2);
		window.SetContentText(label, 1);
		if (((ControllerPollingInfo)(ref pollingInfo)).success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment val = pendingInputMapping.ToElementAssignment(pollingInfo, modifierFlags);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, val, skipOtherPlayers: false))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(val);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(val, skipOtherPlayers: false);
			}
		}
	}

	private void OnMouseElementAssignmentPollingWindowUpdate(int windowId)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		ControllerPollingInfo pollingInfo;
		if (_ignoreMouseXAxisAssignment || _ignoreMouseYAxisAssignment)
		{
			pollingInfo = default(ControllerPollingInfo);
			foreach (ControllerPollingInfo item in ReInput.controllers.polling.PollControllerForAllElementsDown((ControllerType)1, 0))
			{
				ControllerPollingInfo val = item;
				if ((int)((ControllerPollingInfo)(ref val)).elementType != 0 || ((!_ignoreMouseXAxisAssignment || ((ControllerPollingInfo)(ref val)).elementIndex != 0) && (!_ignoreMouseYAxisAssignment || ((ControllerPollingInfo)(ref val)).elementIndex != 1)))
				{
					pollingInfo = val;
					break;
				}
			}
		}
		else
		{
			pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown((ControllerType)1, 0);
		}
		if (((ControllerPollingInfo)(ref pollingInfo)).success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment val2 = pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, val2, skipOtherPlayers: true))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(val2);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(val2, skipOtherPlayers: true);
			}
		}
	}

	private void OnCalibrateAxisStep1WindowUpdate(int windowId)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
		{
			return;
		}
		InputPollingStarted();
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			if (currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			ControllerPollingInfo val = ((Controller)pendingAxisCalibration.joystick).PollForFirstButtonDown();
			if (!((ControllerPollingInfo)(ref val)).success)
			{
				return;
			}
		}
		pendingAxisCalibration.RecordZero();
		CloseWindow(windowId);
		ShowCalibrateAxisStep2Window();
	}

	private void OnCalibrateAxisStep2WindowUpdate(int windowId)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
		{
			return;
		}
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			pendingAxisCalibration.RecordMinMax();
			if (currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			ControllerPollingInfo val = ((Controller)pendingAxisCalibration.joystick).PollForFirstButtonDown();
			if (!((ControllerPollingInfo)(ref val)).success)
			{
				return;
			}
		}
		EndAxisCalibration();
		InputPollingStopped();
		CloseWindow(windowId);
	}

	private void ShowAssignControllerWindow()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer != null && ReInput.controllers.joystickCount != 0)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				window.SetUpdateCallback(OnAssignControllerWindowUpdate);
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.assignControllerWindowTitle);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.assignControllerWindowMessage);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.timer.Start(_controllerAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowControllerAssignmentConflictWindow(int controllerId)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null || ReInput.controllers.joystickCount == 0)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		string otherPlayerName = string.Empty;
		IList<Player> players = ReInput.players.Players;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != currentPlayer && players[i].controllers.ContainsController((ControllerType)2, controllerId))
			{
				otherPlayerName = _language.GetPlayerName(players[i].id);
				break;
			}
		}
		Joystick joystick = ReInput.controllers.GetJoystick(controllerId);
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.controllerAssignmentConflictWindowTitle);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetControllerAssignmentConflictWindowMessage(_language.GetControllerName((Controller)(object)joystick), otherPlayerName, _language.GetPlayerName(currentPlayer.id)));
		UnityAction unityAction = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = unityAction;
		window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
		{
			OnControllerAssignmentConfirmed(window.id, currentPlayer, controllerId);
		}, unityAction, setDefault: true);
		window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, unityAction, unityAction, setDefault: false);
		windowManager.Focus(window);
	}

	private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		GUIInputField gUIInputField = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
		if (gUIInputField == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, actionName);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), gUIInputField.GetLabel());
			UnityAction unityAction = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
			{
				OnBeginElementAssignment(fieldInfo, map, aem, actionName);
			}, unityAction, setDefault: true);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.remove, delegate
			{
				OnRemoveElementAssignment(window.id, map, aem);
			}, unityAction, setDefault: false);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, unityAction, unityAction, setDefault: false);
			windowManager.Focus(window);
		}
	}

	private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData) != null)
		{
			OnBeginElementAssignment(fieldInfo, map, null, actionName);
		}
	}

	private void ShowElementAssignmentPrePollingWindow()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (pendingInputMapping == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.elementAssignmentPrePollingWindowMessage);
			if (prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnElementAssignmentPrePollingWindowUpdate);
			window.timer.Start(_preInputAssignmentTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowElementAssignmentPollingWindow()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		if (pendingInputMapping == null)
		{
			return;
		}
		ControllerType controllerType = pendingInputMapping.controllerType;
		switch ((int)controllerType)
		{
		case 2:
			ShowJoystickElementAssignmentPollingWindow();
			break;
		case 0:
			ShowKeyboardElementAssignmentPollingWindow();
			break;
		case 1:
			if (currentPlayer.controllers.hasMouse)
			{
				ShowMouseElementAssignmentPollingWindow();
			}
			else
			{
				ShowMouseAssignmentConflictWindow();
			}
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private void ShowJoystickElementAssignmentPollingWindow()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				string text = (((int)pendingInputMapping.axisRange == 0 && _showFullAxisInputFields && !_showSplitAxisInputFields) ? _language.GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) : _language.GetJoystickElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnJoystickElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowKeyboardElementAssignmentPollingWindow()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetKeyboardElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, 0f - (window.GetContentTextHeight(0) + 50f)), "");
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnKeyboardElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowMouseElementAssignmentPollingWindow()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				string text = (((int)pendingInputMapping.axisRange == 0 && _showFullAxisInputFields && !_showSplitAxisInputFields) ? _language.GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) : _language.GetMouseElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnMouseElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if (pendingInputMapping == null)
		{
			return;
		}
		bool flag = IsBlockingAssignmentConflict(pendingInputMapping, assignment, skipOtherPlayers);
		string text = (flag ? _language.GetElementAlreadyInUseBlocked(pendingInputMapping.elementName) : _language.GetElementAlreadyInUseCanReplace(pendingInputMapping.elementName, _allowElementAssignmentConflicts));
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.elementAssignmentConflictWindowMessage);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
		UnityAction unityAction = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = unityAction;
		if (flag)
		{
			window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.okay, unityAction, unityAction, setDefault: true);
		}
		else
		{
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, allowSwap: false);
			}, unityAction, setDefault: true);
			if (_allowElementAssignmentConflicts)
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.add, delegate
				{
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					OnElementAssignmentAddConfirmed(window.id, pendingInputMapping, assignment);
				}, unityAction, setDefault: false);
			}
			else if (ShowSwapButton(window.id, pendingInputMapping, assignment, skipOtherPlayers))
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.swap, delegate
				{
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, allowSwap: true);
				}, unityAction, setDefault: false);
			}
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, unityAction, unityAction, setDefault: false);
		}
		windowManager.Focus(window);
	}

	private void ShowMouseAssignmentConflictWindow()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		string otherPlayerName = string.Empty;
		IList<Player> players = ReInput.players.Players;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != currentPlayer && players[i].controllers.hasMouse)
			{
				otherPlayerName = _language.GetPlayerName(players[i].id);
				break;
			}
		}
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.mouseAssignmentConflictWindowTitle);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetMouseAssignmentConflictWindowMessage(otherPlayerName, _language.GetPlayerName(currentPlayer.id)));
		UnityAction unityAction = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = unityAction;
		window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
		{
			OnMouseAssignmentConfirmed(window.id, currentPlayer);
		}, unityAction, setDefault: true);
		window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, unityAction, unityAction, setDefault: false);
		windowManager.Focus(window);
	}

	private void ShowCalibrateControllerWindow()
	{
		if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
		{
			CalibrationWindow calibrationWindow = OpenWindow(prefabs.calibrationWindow, "CalibrationWindow", closeOthers: true) as CalibrationWindow;
			if (!(calibrationWindow == null))
			{
				Joystick joystick = currentJoystick;
				calibrationWindow.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateControllerWindowTitle);
				calibrationWindow.SetJoystick(currentPlayer.id, joystick);
				calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, CloseWindow);
				calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, StartAxisCalibration);
				calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, CloseWindow);
				windowManager.Focus(calibrationWindow);
			}
		}
	}

	private void ShowCalibrateAxisStep1Window()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: false);
		if (window == null || pendingAxisCalibration == null)
		{
			return;
		}
		Joystick joystick = pendingAxisCalibration.joystick;
		if (((ControllerWithAxes)joystick).axisCount == 0)
		{
			return;
		}
		int axisIndex = pendingAxisCalibration.axisIndex;
		if (axisIndex >= 0 && axisIndex < ((ControllerWithAxes)joystick).axisCount)
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep1WindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep1WindowMessage(_language.GetElementIdentifierName((Controller)(object)joystick, ((ControllerWithAxes)joystick).AxisElementIdentifiers[axisIndex].id, (AxisRange)0)));
			if (prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnCalibrateAxisStep1WindowUpdate);
			window.timer.Start(_axisCalibrationTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowCalibrateAxisStep2Window()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: false);
		if (window == null || pendingAxisCalibration == null)
		{
			return;
		}
		Joystick joystick = pendingAxisCalibration.joystick;
		if (((ControllerWithAxes)joystick).axisCount == 0)
		{
			return;
		}
		int axisIndex = pendingAxisCalibration.axisIndex;
		if (axisIndex >= 0 && axisIndex < ((ControllerWithAxes)joystick).axisCount)
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep2WindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep2WindowMessage(_language.GetElementIdentifierName((Controller)(object)joystick, ((ControllerWithAxes)joystick).AxisElementIdentifiers[axisIndex].id, (AxisRange)0)));
			if (prefabs.moveStickGraphic != null)
			{
				window.AddContentImage(prefabs.moveStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnCalibrateAxisStep2WindowUpdate);
			window.timer.Start(_axisCalibrationTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowEditInputBehaviorsWindow()
	{
		if (currentPlayer != null && _inputBehaviorSettings != null)
		{
			InputBehaviorWindow inputBehaviorWindow = OpenWindow(prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", closeOthers: true) as InputBehaviorWindow;
			if (!(inputBehaviorWindow == null))
			{
				inputBehaviorWindow.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.inputBehaviorSettingsWindowTitle);
				inputBehaviorWindow.SetData(currentPlayer.id, _inputBehaviorSettings);
				inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, CloseWindow);
				inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, CloseWindow);
				windowManager.Focus(inputBehaviorWindow);
			}
		}
	}

	private void ShowRestoreDefaultsWindow()
	{
		if (currentPlayer != null)
		{
			OpenModal(_language.restoreDefaultsWindowTitle, _language.restoreDefaultsWindowMessage, _language.yes, OnRestoreDefaultsConfirmed, _language.no, OnWindowCancel, closeOthers: true);
		}
	}

	private void CreateInputGrid()
	{
		InitializeInputGrid();
		CreateHeaderLabels();
		CreateActionLabelColumn();
		CreateKeyboardInputFieldColumn();
		CreateMouseInputFieldColumn();
		CreateControllerInputFieldColumn();
		CreateInputActionLabels();
		CreateInputFields();
		inputGrid.HideAll();
		ResetInputGridScrollBar();
	}

	private void InitializeInputGrid()
	{
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Invalid comparison between Unknown and I4
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Invalid comparison between Unknown and I4
		if (inputGrid == null)
		{
			inputGrid = new InputGrid();
		}
		else
		{
			inputGrid.ClearAll();
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet mappingSet = _mappingSets[i];
			if (mappingSet == null || !mappingSet.isValid)
			{
				continue;
			}
			InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
			if (mapCategory == null || !((InputCategory)mapCategory).userAssignable)
			{
				continue;
			}
			inputGrid.AddMapCategory(mappingSet.mapCategoryId);
			if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
				for (int j = 0; j < actionCategoryIds.Count; j++)
				{
					int num = actionCategoryIds[j];
					InputCategory actionCategory = ReInput.mapping.GetActionCategory(num);
					if (actionCategory == null || !actionCategory.userAssignable)
					{
						continue;
					}
					inputGrid.AddActionCategory(mappingSet.mapCategoryId, num);
					foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(num))
					{
						if ((int)item.type == 0)
						{
							if (_showFullAxisInputFields)
							{
								inputGrid.AddAction(mappingSet.mapCategoryId, item, (AxisRange)0);
							}
							if (_showSplitAxisInputFields)
							{
								inputGrid.AddAction(mappingSet.mapCategoryId, item, (AxisRange)1);
								inputGrid.AddAction(mappingSet.mapCategoryId, item, (AxisRange)2);
							}
						}
						else if ((int)item.type == 1)
						{
							inputGrid.AddAction(mappingSet.mapCategoryId, item, (AxisRange)1);
						}
					}
				}
				continue;
			}
			IList<int> actionIds = mappingSet.actionIds;
			for (int k = 0; k < actionIds.Count; k++)
			{
				InputAction action = ReInput.mapping.GetAction(actionIds[k]);
				if (action == null)
				{
					continue;
				}
				if ((int)action.type == 0)
				{
					if (_showFullAxisInputFields)
					{
						inputGrid.AddAction(mappingSet.mapCategoryId, action, (AxisRange)0);
					}
					if (_showSplitAxisInputFields)
					{
						inputGrid.AddAction(mappingSet.mapCategoryId, action, (AxisRange)1);
						inputGrid.AddAction(mappingSet.mapCategoryId, action, (AxisRange)2);
					}
				}
				else if ((int)action.type == 1)
				{
					inputGrid.AddAction(mappingSet.mapCategoryId, action, (AxisRange)1);
				}
			}
		}
		references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = _inputColumnSpacing;
		references.inputGridLayoutElement.flexibleWidth = 0f;
		references.inputGridLayoutElement.preferredWidth = inputGridWidth;
	}

	private void RefreshInputGridStructure()
	{
		if (currentMappingSet != null)
		{
			inputGrid.HideAll();
			inputGrid.Show(currentMappingSet.mapCategoryId);
			references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inputGrid.GetColumnHeight(currentMappingSet.mapCategoryId));
		}
	}

	private void CreateHeaderLabels()
	{
		references.inputGridHeader1 = CreateNewColumnGroup("ActionsHeader", references.inputGridHeadersGroup, _actionLabelWidth).transform;
		CreateLabel(prefabs.inputGridHeaderLabel, _language.actionColumnLabel, references.inputGridHeader1, Vector2.zero);
		if (_showKeyboard)
		{
			references.inputGridHeader2 = CreateNewColumnGroup("KeybordHeader", references.inputGridHeadersGroup, _keyboardColMaxWidth).transform;
			CreateLabel(prefabs.inputGridHeaderLabel, _language.keyboardColumnLabel, references.inputGridHeader2, Vector2.zero).SetTextAlignment(TextAnchor.MiddleCenter);
		}
		if (_showMouse)
		{
			references.inputGridHeader3 = CreateNewColumnGroup("MouseHeader", references.inputGridHeadersGroup, _mouseColMaxWidth).transform;
			CreateLabel(prefabs.inputGridHeaderLabel, _language.mouseColumnLabel, references.inputGridHeader3, Vector2.zero).SetTextAlignment(TextAnchor.MiddleCenter);
		}
		if (_showControllers)
		{
			references.inputGridHeader4 = CreateNewColumnGroup("ControllerHeader", references.inputGridHeadersGroup, _controllerColMaxWidth).transform;
			CreateLabel(prefabs.inputGridHeaderLabel, _language.controllerColumnLabel, references.inputGridHeader4, Vector2.zero).SetTextAlignment(TextAnchor.MiddleCenter);
		}
	}

	private void CreateActionLabelColumn()
	{
		Transform inputGridActionColumn = CreateNewColumnGroup("ActionLabelColumn", references.inputGridInnerGroup, _actionLabelWidth).transform;
		references.inputGridActionColumn = inputGridActionColumn;
	}

	private void CreateKeyboardInputFieldColumn()
	{
		if (_showKeyboard)
		{
			CreateInputFieldColumn("KeyboardColumn", (ControllerType)0, _keyboardColMaxWidth, _keyboardInputFieldCount, disableFullAxis: true);
		}
	}

	private void CreateMouseInputFieldColumn()
	{
		if (_showMouse)
		{
			CreateInputFieldColumn("MouseColumn", (ControllerType)1, _mouseColMaxWidth, _mouseInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateControllerInputFieldColumn()
	{
		if (_showControllers)
		{
			CreateInputFieldColumn("ControllerColumn", (ControllerType)2, _controllerColMaxWidth, _controllerInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected I4, but got Unknown
		Transform transform = CreateNewColumnGroup(name, references.inputGridInnerGroup, maxWidth).transform;
		switch ((int)controllerType)
		{
		case 2:
			references.inputGridControllerColumn = transform;
			break;
		case 0:
			references.inputGridKeyboardColumn = transform;
			break;
		case 1:
			references.inputGridMouseColumn = transform;
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private void CreateInputActionLabels()
	{
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Invalid comparison between Unknown and I4
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Invalid comparison between Unknown and I4
		Transform inputGridActionColumn = references.inputGridActionColumn;
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet mappingSet = _mappingSets[i];
			if (mappingSet == null || !mappingSet.isValid)
			{
				continue;
			}
			int num = 0;
			if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				int num2 = 0;
				IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
				for (int j = 0; j < actionCategoryIds.Count; j++)
				{
					InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
					if (actionCategory == null || !actionCategory.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) == 0)
					{
						continue;
					}
					if (_showActionCategoryLabels)
					{
						if (num2 > 0)
						{
							num -= _inputRowCategorySpacing;
						}
						GUILabel gUILabel = CreateLabel(_language.GetActionCategoryName(actionCategory.id), inputGridActionColumn, new Vector2(0f, num));
						gUILabel.SetFontStyle(FontStyle.Bold);
						gUILabel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
						inputGrid.AddActionCategoryLabel(mappingSet.mapCategoryId, actionCategory.id, gUILabel);
						num -= _inputRowHeight;
					}
					foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
					{
						if ((int)item.type == 0)
						{
							if (_showFullAxisInputFields)
							{
								GUILabel gUILabel2 = CreateLabel(_language.GetActionName(item.id, (AxisRange)0), inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, (AxisRange)0, gUILabel2);
								num -= _inputRowHeight;
							}
							if (_showSplitAxisInputFields)
							{
								string actionName = _language.GetActionName(item.id, (AxisRange)1);
								GUILabel gUILabel2 = CreateLabel(actionName, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, (AxisRange)1, gUILabel2);
								num -= _inputRowHeight;
								string actionName2 = _language.GetActionName(item.id, (AxisRange)2);
								gUILabel2 = CreateLabel(actionName2, inputGridActionColumn, new Vector2(0f, num));
								gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, (AxisRange)2, gUILabel2);
								num -= _inputRowHeight;
							}
						}
						else if ((int)item.type == 1)
						{
							GUILabel gUILabel2 = CreateLabel(_language.GetActionName(item.id), inputGridActionColumn, new Vector2(0f, num));
							gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(mappingSet.mapCategoryId, item.id, (AxisRange)1, gUILabel2);
							num -= _inputRowHeight;
						}
					}
					num2++;
				}
			}
			else
			{
				IList<int> actionIds = mappingSet.actionIds;
				for (int k = 0; k < actionIds.Count; k++)
				{
					InputAction action = ReInput.mapping.GetAction(actionIds[k]);
					if (action == null || !action.userAssignable)
					{
						continue;
					}
					InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
					if (actionCategory2 == null || !actionCategory2.userAssignable)
					{
						continue;
					}
					if ((int)action.type == 0)
					{
						if (_showFullAxisInputFields)
						{
							GUILabel gUILabel3 = CreateLabel(_language.GetActionName(action.id, (AxisRange)0), inputGridActionColumn, new Vector2(0f, num));
							gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, (AxisRange)0, gUILabel3);
							num -= _inputRowHeight;
						}
						if (_showSplitAxisInputFields)
						{
							GUILabel gUILabel3 = CreateLabel(_language.GetActionName(action.id, (AxisRange)1), inputGridActionColumn, new Vector2(0f, num));
							gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, (AxisRange)1, gUILabel3);
							num -= _inputRowHeight;
							gUILabel3 = CreateLabel(_language.GetActionName(action.id, (AxisRange)2), inputGridActionColumn, new Vector2(0f, num));
							gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, (AxisRange)2, gUILabel3);
							num -= _inputRowHeight;
						}
					}
					else if ((int)action.type == 1)
					{
						GUILabel gUILabel3 = CreateLabel(_language.GetActionName(action.id), inputGridActionColumn, new Vector2(0f, num));
						gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
						inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, (AxisRange)1, gUILabel3);
						num -= _inputRowHeight;
					}
				}
			}
			inputGrid.SetColumnHeight(mappingSet.mapCategoryId, -num);
		}
	}

	private void CreateInputFields()
	{
		if (_showControllers)
		{
			CreateInputFields(references.inputGridControllerColumn, (ControllerType)2, _controllerColMaxWidth, _controllerInputFieldCount, disableFullAxis: false);
		}
		if (_showKeyboard)
		{
			CreateInputFields(references.inputGridKeyboardColumn, (ControllerType)0, _keyboardColMaxWidth, _keyboardInputFieldCount, disableFullAxis: true);
		}
		if (_showMouse)
		{
			CreateInputFields(references.inputGridMouseColumn, (ControllerType)1, _mouseColMaxWidth, _mouseInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
	{
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Invalid comparison between Unknown and I4
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet mappingSet = _mappingSets[i];
			if (mappingSet == null || !mappingSet.isValid)
			{
				continue;
			}
			int fieldWidth = maxWidth / cols;
			int yPos = 0;
			int num = 0;
			if (mappingSet.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
				for (int j = 0; j < actionCategoryIds.Count; j++)
				{
					InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
					if (actionCategory == null || !actionCategory.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) == 0)
					{
						continue;
					}
					if (_showActionCategoryLabels)
					{
						yPos -= ((num > 0) ? (_inputRowHeight + _inputRowCategorySpacing) : _inputRowHeight);
					}
					foreach (InputAction item in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
					{
						if ((int)item.type == 0)
						{
							if (_showFullAxisInputFields)
							{
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, (AxisRange)0, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
							}
							if (_showSplitAxisInputFields)
							{
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, (AxisRange)1, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
								CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, (AxisRange)2, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
							}
						}
						else if ((int)item.type == 1)
						{
							CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, item, (AxisRange)1, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
						}
						num++;
					}
				}
				continue;
			}
			IList<int> actionIds = mappingSet.actionIds;
			for (int k = 0; k < actionIds.Count; k++)
			{
				InputAction action = ReInput.mapping.GetAction(actionIds[k]);
				if (action == null || !action.userAssignable)
				{
					continue;
				}
				InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
				if (actionCategory2 == null || !actionCategory2.userAssignable)
				{
					continue;
				}
				if ((int)action.type == 0)
				{
					if (_showFullAxisInputFields)
					{
						CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, (AxisRange)0, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
					}
					if (_showSplitAxisInputFields)
					{
						CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, (AxisRange)1, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
						CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, (AxisRange)2, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
					}
				}
				else if ((int)action.type == 1)
				{
					CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, (AxisRange)1, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
				}
			}
		}
	}

	private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0f, yPos));
		HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.padding = _inputRowPadding;
		horizontalLayoutGroup.spacing = _inputRowFieldSpacing;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.sizeDelta = Vector2.zero;
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
		inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, gameObject);
		for (int i = 0; i < cols; i++)
		{
			int num = (((int)axisRange == 0) ? _invertToggleWidth : 0);
			GUIInputField gUIInputField = CreateInputField(horizontalLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, i);
			gUIInputField.SetFirstChildObjectWidth(LayoutElementSizeType.PreferredSize, fieldWidth - num);
			gUIInputField.SetFirstChildObjectWidth(LayoutElementSizeType.MinSize, 0);
			inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, i, gUIInputField);
			if ((int)axisRange == 0)
			{
				if (!disableFullAxis)
				{
					GUIToggle gUIToggle = CreateToggle(prefabs.inputGridFieldInvertToggle, horizontalLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, i);
					gUIToggle.SetFirstChildObjectWidth(LayoutElementSizeType.MinSize, num);
					gUIInputField.AddToggle(gUIToggle);
				}
				else
				{
					gUIInputField.SetInteractible(state: false, playTransition: false, permanent: true);
				}
			}
		}
		yPos -= _inputRowHeight;
	}

	private void PopulateInputFields()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		inputGrid.InitializeFields(currentMapCategoryId);
		if (currentPlayer == null)
		{
			return;
		}
		inputGrid.SetFieldsActive(currentMapCategoryId, state: true);
		foreach (InputActionSet actionSet in inputGrid.GetActionSets(currentMapCategoryId))
		{
			if (_showKeyboard)
			{
				ControllerType controllerType = (ControllerType)0;
				int controllerId = 0;
				int layoutId = _keyboardMapDefaultLayout;
				int maxFields = _keyboardInputFieldCount;
				ControllerMap controllerMapOrCreateNew = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew, controllerType, controllerId, maxFields);
			}
			if (_showMouse)
			{
				ControllerType controllerType = (ControllerType)1;
				int controllerId = 0;
				int layoutId = _mouseMapDefaultLayout;
				int maxFields = _mouseInputFieldCount;
				ControllerMap controllerMapOrCreateNew2 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				if (currentPlayer.controllers.hasMouse)
				{
					PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew2, controllerType, controllerId, maxFields);
				}
			}
			if (isJoystickSelected && currentPlayer.controllers.joystickCount > 0)
			{
				ControllerType controllerType = (ControllerType)2;
				int controllerId = ((Controller)currentJoystick).id;
				int layoutId = _joystickMapDefaultLayout;
				int maxFields = _controllerInputFieldCount;
				ControllerMap controllerMapOrCreateNew3 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				PopulateInputFieldGroup(actionSet, controllerMapOrCreateNew3, controllerType, controllerId, maxFields);
			}
			else
			{
				DisableInputFieldGroup(actionSet, (ControllerType)2, _controllerInputFieldCount);
			}
		}
	}

	private void PopulateInputFieldGroup(InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I4
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Invalid comparison between Unknown and I4
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Invalid comparison between Unknown and I4
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Invalid comparison between Unknown and I4
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		if (controllerMap == null)
		{
			return;
		}
		int num = 0;
		inputGrid.SetFixedFieldData(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);
		foreach (ActionElementMap item in controllerMap.ElementMapsWithAction(actionSet.actionId))
		{
			if ((int)item.elementType == 1)
			{
				if ((int)actionSet.axisRange == 0)
				{
					continue;
				}
				if ((int)actionSet.axisRange == 1)
				{
					if ((int)item.axisContribution == 1)
					{
						continue;
					}
				}
				else if ((int)actionSet.axisRange == 2 && (int)item.axisContribution == 0)
				{
					continue;
				}
				inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, _language.GetElementIdentifierName(item), invert: false);
			}
			else if ((int)item.elementType == 0)
			{
				if ((int)actionSet.axisRange == 0)
				{
					if ((int)item.axisRange != 0)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, _language.GetElementIdentifierName(item), item.invert);
				}
				else if ((int)actionSet.axisRange == 1)
				{
					if (((int)item.axisRange == 0 && (int)ReInput.mapping.GetAction(actionSet.actionId).type != 1) || (int)item.axisContribution == 1)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, _language.GetElementIdentifierName(item), invert: false);
				}
				else if ((int)actionSet.axisRange == 2)
				{
					if ((int)item.axisRange == 0 || (int)item.axisContribution == 0)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, item.id, _language.GetElementIdentifierName(item), invert: false);
				}
			}
			num++;
			if (num > maxFields)
			{
				break;
			}
		}
	}

	private void DisableInputFieldGroup(InputActionSet actionSet, ControllerType controllerType, int fieldCount)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < fieldCount; i++)
		{
			inputGrid.GetGUIInputField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i)?.SetInteractible(state: false, playTransition: false);
		}
	}

	private void ResetInputGridScrollBar()
	{
		references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		references.inputGridVScrollbar.value = 1f;
		references.inputGridScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
	}

	private void CreateLayout()
	{
		references.playersGroup.gameObject.SetActive(showPlayers);
		references.controllerGroup.gameObject.SetActive(_showControllers);
		references.assignedControllersGroup.gameObject.SetActive(_showControllers && ShowAssignedControllers());
		references.settingsAndMapCategoriesGroup.gameObject.SetActive(showSettings || showMapCategories);
		references.settingsGroup.gameObject.SetActive(showSettings);
		references.mapCategoriesGroup.gameObject.SetActive(showMapCategories);
	}

	private void Draw()
	{
		DrawPlayersGroup();
		DrawControllersGroup();
		DrawSettingsGroup();
		DrawMapCategoriesGroup();
		DrawWindowButtonsGroup();
	}

	private void DrawPlayersGroup()
	{
		if (!showPlayers)
		{
			return;
		}
		references.playersGroup.labelText = _language.playersGroupLabel;
		references.playersGroup.SetLabelActive(_showPlayersGroupLabel);
		for (int i = 0; i < playerCount; i++)
		{
			Player player = ReInput.players.GetPlayer(i);
			if (player != null)
			{
				GUIButton gUIButton = new GUIButton(UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.playersGroup.content, "Player" + i + "Button"));
				gUIButton.SetLabel(_language.GetPlayerName(player.id));
				gUIButton.SetButtonInfoData("PlayerSelection", player.id);
				gUIButton.SetOnClickCallback(OnButtonActivated);
				gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
				playerButtons.Add(gUIButton);
			}
		}
	}

	private void DrawControllersGroup()
	{
		if (_showControllers)
		{
			references.controllerSettingsGroup.labelText = _language.controllerSettingsGroupLabel;
			references.controllerSettingsGroup.SetLabelActive(_showControllerGroupLabel);
			references.controllerNameLabel.gameObject.SetActive(_showControllerNameLabel);
			references.controllerGroupLabelGroup.gameObject.SetActive(_showControllerGroupLabel || _showControllerNameLabel);
			if (ShowAssignedControllers())
			{
				references.assignedControllersGroup.labelText = _language.assignedControllersGroupLabel;
				references.assignedControllersGroup.SetLabelActive(_showAssignedControllersGroupLabel);
			}
			references.removeControllerButton.GetComponent<ButtonInfo>().text.text = _language.removeControllerButtonLabel;
			references.calibrateControllerButton.GetComponent<ButtonInfo>().text.text = _language.calibrateControllerButtonLabel;
			references.assignControllerButton.GetComponent<ButtonInfo>().text.text = _language.assignControllerButtonLabel;
			GUIButton gUIButton = CreateButton(_language.none, references.assignedControllersGroup.content, Vector2.zero);
			gUIButton.SetInteractible(state: false, playTransition: false, permanent: true);
			assignedControllerButtonsPlaceholder = gUIButton;
		}
	}

	private void DrawSettingsGroup()
	{
		if (showSettings)
		{
			references.settingsGroup.labelText = _language.settingsGroupLabel;
			references.settingsGroup.SetLabelActive(_showSettingsGroupLabel);
			GUIButton gUIButton = CreateButton(_language.inputBehaviorSettingsButtonLabel, references.settingsGroup.content, Vector2.zero);
			miscInstantiatedObjects.Add(gUIButton.gameObject);
			gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
			gUIButton.SetButtonInfoData("EditInputBehaviors", 0);
			gUIButton.SetOnClickCallback(OnButtonActivated);
		}
	}

	private void DrawMapCategoriesGroup()
	{
		if (!showMapCategories || _mappingSets == null)
		{
			return;
		}
		references.mapCategoriesGroup.labelText = _language.mapCategoriesGroupLabel;
		references.mapCategoriesGroup.SetLabelActive(_showMapCategoriesGroupLabel);
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet mappingSet = _mappingSets[i];
			if (mappingSet != null && mappingSet.isValid)
			{
				InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
				if (mapCategory != null)
				{
					GUIButton gUIButton = new GUIButton(UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.mapCategoriesGroup.content, ((InputCategory)mapCategory).name + "Button"));
					gUIButton.SetLabel(_language.GetMapCategoryName(((InputCategory)mapCategory).id));
					gUIButton.SetButtonInfoData("MapCategorySelection", ((InputCategory)mapCategory).id);
					gUIButton.SetOnClickCallback(OnButtonActivated);
					gUIButton.buttonInfo.OnSelectedEvent += OnUIElementSelected;
					mapCategoryButtons.Add(gUIButton);
				}
			}
		}
	}

	private void DrawWindowButtonsGroup()
	{
		references.doneButton.GetComponent<ButtonInfo>().text.text = _language.doneButtonLabel;
		references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = _language.restoreDefaultsButtonLabel;
	}

	private void Redraw(bool listsChanged, bool playTransitions)
	{
		RedrawPlayerGroup(playTransitions);
		RedrawControllerGroup();
		RedrawMapCategoriesGroup(playTransitions);
		RedrawInputGrid(listsChanged);
		if (currentUISelection == null || !currentUISelection.activeInHierarchy)
		{
			RestoreLastUISelection();
		}
	}

	private void RedrawPlayerGroup(bool playTransitions)
	{
		if (showPlayers)
		{
			for (int i = 0; i < playerButtons.Count; i++)
			{
				bool state = currentPlayerId != playerButtons[i].buttonInfo.intData;
				playerButtons[i].SetInteractible(state, playTransitions);
			}
		}
	}

	private void RedrawControllerGroup()
	{
		int num = -1;
		references.controllerNameLabel.text = _language.none;
		UITools.SetInteractable(references.removeControllerButton, state: false, playTransition: false);
		UITools.SetInteractable(references.assignControllerButton, state: false, playTransition: false);
		UITools.SetInteractable(references.calibrateControllerButton, state: false, playTransition: false);
		if (ShowAssignedControllers())
		{
			foreach (GUIButton assignedControllerButton in assignedControllerButtons)
			{
				if (!(assignedControllerButton.gameObject == null))
				{
					if (currentUISelection == assignedControllerButton.gameObject)
					{
						num = assignedControllerButton.buttonInfo.intData;
					}
					UnityEngine.Object.Destroy(assignedControllerButton.gameObject);
				}
			}
			assignedControllerButtons.Clear();
			assignedControllerButtonsPlaceholder.SetActive(state: true);
		}
		Player player = ReInput.players.GetPlayer(currentPlayerId);
		if (player == null)
		{
			return;
		}
		if (ShowAssignedControllers())
		{
			if (player.controllers.joystickCount > 0)
			{
				assignedControllerButtonsPlaceholder.SetActive(state: false);
			}
			foreach (Joystick joystick in player.controllers.Joysticks)
			{
				GUIButton gUIButton2 = CreateButton(_language.GetControllerName((Controller)(object)joystick), references.assignedControllersGroup.content, Vector2.zero);
				gUIButton2.SetButtonInfoData("AssignedControllerSelection", ((Controller)joystick).id);
				gUIButton2.SetOnClickCallback(OnButtonActivated);
				gUIButton2.buttonInfo.OnSelectedEvent += OnUIElementSelected;
				assignedControllerButtons.Add(gUIButton2);
				if (((Controller)joystick).id == currentJoystickId)
				{
					gUIButton2.SetInteractible(state: false, playTransition: true);
				}
			}
			if (player.controllers.joystickCount > 0 && !isJoystickSelected)
			{
				currentJoystickId = ((Controller)player.controllers.Joysticks[0]).id;
				assignedControllerButtons[0].SetInteractible(state: false, playTransition: false);
			}
			if (num >= 0)
			{
				foreach (GUIButton assignedControllerButton2 in assignedControllerButtons)
				{
					if (assignedControllerButton2.buttonInfo.intData == num)
					{
						SetUISelection(assignedControllerButton2.gameObject);
						break;
					}
				}
			}
		}
		else if (player.controllers.joystickCount > 0 && !isJoystickSelected)
		{
			currentJoystickId = ((Controller)player.controllers.Joysticks[0]).id;
		}
		if (isJoystickSelected && player.controllers.joystickCount > 0)
		{
			references.removeControllerButton.interactable = true;
			references.controllerNameLabel.text = _language.GetControllerName((Controller)(object)currentJoystick);
			if (((ControllerWithAxes)currentJoystick).axisCount > 0)
			{
				references.calibrateControllerButton.interactable = true;
			}
		}
		int joystickCount = player.controllers.joystickCount;
		int joystickCount2 = ReInput.controllers.joystickCount;
		int num2 = GetMaxControllersPerPlayer();
		bool flag = num2 == 0;
		if (joystickCount2 > 0 && joystickCount < joystickCount2 && (num2 == 1 || flag || joystickCount < num2))
		{
			UITools.SetInteractable(references.assignControllerButton, state: true, playTransition: false);
		}
	}

	private void RedrawMapCategoriesGroup(bool playTransitions)
	{
		if (showMapCategories)
		{
			for (int i = 0; i < mapCategoryButtons.Count; i++)
			{
				bool state = currentMapCategoryId != mapCategoryButtons[i].buttonInfo.intData;
				mapCategoryButtons[i].SetInteractible(state, playTransitions);
			}
		}
	}

	private void RedrawInputGrid(bool listsChanged)
	{
		if (listsChanged)
		{
			RefreshInputGridStructure();
		}
		PopulateInputFields();
		if (listsChanged)
		{
			ResetInputGridScrollBar();
		}
	}

	private void ForceRefresh()
	{
		if (windowManager.isWindowOpen)
		{
			CloseAllWindows();
		}
		else
		{
			Redraw(listsChanged: false, playTransitions: false);
		}
	}

	private void CreateInputCategoryRow(ref int rowCount, InputCategory category)
	{
		CreateLabel(_language.GetMapCategoryName(category.id), references.inputGridActionColumn, new Vector2(0f, (float)(rowCount * _inputRowHeight) * -1f));
		rowCount++;
	}

	private GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset)
	{
		return CreateLabel(prefabs.inputGridLabel, labelText, parent, offset);
	}

	private GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset)
	{
		GameObject gameObject = InstantiateGUIObject(prefab, parent, offset);
		Text componentInSelfOrChildren = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
		if (componentInSelfOrChildren == null)
		{
			Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
			return null;
		}
		componentInSelfOrChildren.text = labelText;
		return new GUILabel(gameObject);
	}

	private GUIButton CreateButton(string labelText, Transform parent, Vector2 offset)
	{
		GUIButton gUIButton = new GUIButton(InstantiateGUIObject(prefabs.button, parent, offset));
		gUIButton.SetLabel(labelText);
		return gUIButton;
	}

	private GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset)
	{
		GUIButton gUIButton = new GUIButton(InstantiateGUIObject(prefabs.fitButton, parent, offset));
		gUIButton.SetLabel(labelText);
		return gUIButton;
	}

	private GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		GUIInputField gUIInputField = CreateInputField(parent, offset);
		gUIInputField.SetLabel("");
		gUIInputField.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
		gUIInputField.SetOnClickCallback(inputFieldActivatedDelegate);
		gUIInputField.fieldInfo.OnSelectedEvent += OnUIElementSelected;
		if (gUIInputField.fieldInfo.glyphOrText != null)
		{
			gUIInputField.fieldInfo.glyphOrText.allowedTypes = ((!_showGlyphs) ? ControllerElementGlyphBase.AllowedTypes.Text : ControllerElementGlyphBase.AllowedTypes.All);
		}
		return gUIInputField;
	}

	private GUIInputField CreateInputField(Transform parent, Vector2 offset)
	{
		return new GUIInputField(InstantiateGUIObject(prefabs.inputGridFieldButton, parent, offset));
	}

	private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GUIToggle gUIToggle = CreateToggle(prefab, parent, offset);
		gUIToggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
		gUIToggle.SetOnSubmitCallback(inputFieldInvertToggleStateChangedDelegate);
		gUIToggle.toggleInfo.OnSelectedEvent += OnUIElementSelected;
		return gUIToggle;
	}

	private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset)
	{
		return new GUIToggle(InstantiateGUIObject(prefab, parent, offset));
	}

	private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset)
	{
		if (prefab == null)
		{
			Debug.LogError("Rewired Control Mapper: Prefab is null!");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		return InitializeNewGUIGameObject(gameObject, parent, offset);
	}

	private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = name;
		gameObject.AddComponent<RectTransform>();
		return InitializeNewGUIGameObject(gameObject, parent, offset);
	}

	private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset)
	{
		if (gameObject == null)
		{
			Debug.LogError("Rewired Control Mapper: GameObject is null!");
			return null;
		}
		RectTransform component = gameObject.GetComponent<RectTransform>();
		if (component == null)
		{
			Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
			return gameObject;
		}
		if (parent != null)
		{
			component.SetParent(parent, worldPositionStays: false);
		}
		component.anchoredPosition = offset;
		return gameObject;
	}

	private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth)
	{
		GameObject gameObject = CreateNewGUIObject(name, parent, Vector2.zero);
		inputGrid.AddGroup(gameObject);
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		if (maxWidth >= 0)
		{
			layoutElement.preferredWidth = maxWidth;
		}
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, 0f);
		return gameObject;
	}

	private Window OpenWindow(bool closeOthers)
	{
		return OpenWindow(string.Empty, closeOthers);
	}

	private Window OpenWindow(string name, bool closeOthers)
	{
		if (closeOthers)
		{
			windowManager.CancelAll();
		}
		Window window = windowManager.OpenWindow(name, _defaultWindowWidth, _defaultWindowHeight);
		if (window == null)
		{
			return null;
		}
		ChildWindowOpened();
		return window;
	}

	private Window OpenWindow(GameObject windowPrefab, bool closeOthers)
	{
		return OpenWindow(windowPrefab, string.Empty, closeOthers);
	}

	private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers)
	{
		if (closeOthers)
		{
			windowManager.CancelAll();
		}
		Window window = windowManager.OpenWindow(windowPrefab, name);
		if (window == null)
		{
			return null;
		}
		ChildWindowOpened();
		return window;
	}

	private void OpenModal(string title, string message, string confirmText, Action<int> confirmAction, string cancelText, Action<int> cancelAction, bool closeOthers)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		Window window = OpenWindow(closeOthers);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, title);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
			UnityAction unityAction = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, confirmText, delegate
			{
				OnRestoreDefaultsConfirmed(window.id);
			}, unityAction, setDefault: false);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, cancelText, unityAction, unityAction, setDefault: true);
			windowManager.Focus(window);
		}
	}

	private void CloseWindow(int windowId)
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CloseWindow(windowId);
			ChildWindowClosed();
		}
	}

	private void CloseTopWindow()
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CloseTop();
			ChildWindowClosed();
		}
	}

	private void CloseAllWindows()
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CancelAll();
			ChildWindowClosed();
			InputPollingStopped();
		}
	}

	private void ChildWindowOpened()
	{
		if (windowManager.isWindowOpen)
		{
			SetIsFocused(state: false);
			if (_PopupWindowOpenedEvent != null)
			{
				_PopupWindowOpenedEvent();
			}
			if (_onPopupWindowOpened != null)
			{
				_onPopupWindowOpened.Invoke();
			}
		}
	}

	private void ChildWindowClosed()
	{
		if (!windowManager.isWindowOpen)
		{
			SetIsFocused(state: true);
			if (_PopupWindowClosedEvent != null)
			{
				_PopupWindowClosedEvent();
			}
			if (_onPopupWindowClosed != null)
			{
				_onPopupWindowClosed.Invoke();
			}
		}
	}

	private bool HasElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (player == null || mapping == null)
		{
			return false;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			return false;
		}
		if (skipOtherPlayers)
		{
			if (ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
			{
				return true;
			}
			if (player.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
			{
				return true;
			}
			return false;
		}
		return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
	}

	private bool IsBlockingAssignmentConflict(InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			return false;
		}
		if (skipOtherPlayers)
		{
			foreach (ElementAssignmentConflictInfo item in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				ElementAssignmentConflictInfo val = item;
				if (!((ElementAssignmentConflictInfo)(ref val)).isUserAssignable)
				{
					return true;
				}
			}
			foreach (ElementAssignmentConflictInfo item2 in currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				ElementAssignmentConflictInfo val2 = item2;
				if (!((ElementAssignmentConflictInfo)(ref val2)).isUserAssignable)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (ElementAssignmentConflictInfo item3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				ElementAssignmentConflictInfo val3 = item3;
				if (!((ElementAssignmentConflictInfo)(ref val3)).isUserAssignable)
				{
					return true;
				}
			}
		}
		return false;
	}

	private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (player == null || mapping == null || !CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			yield break;
		}
		if (skipOtherPlayers)
		{
			foreach (ElementAssignmentConflictInfo item in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				ElementAssignmentConflictInfo val = item;
				if (!((ElementAssignmentConflictInfo)(ref val)).isUserAssignable)
				{
					yield return val;
				}
			}
			foreach (ElementAssignmentConflictInfo item2 in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				ElementAssignmentConflictInfo val2 = item2;
				if (!((ElementAssignmentConflictInfo)(ref val2)).isUserAssignable)
				{
					yield return val2;
				}
			}
			yield break;
		}
		foreach (ElementAssignmentConflictInfo item3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
		{
			ElementAssignmentConflictInfo val3 = item3;
			if (!((ElementAssignmentConflictInfo)(ref val3)).isUserAssignable)
			{
				yield return val3;
			}
		}
	}

	private bool CreateConflictCheck(InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (mapping == null || currentPlayer == null)
		{
			conflictCheck = default(ElementAssignmentConflictCheck);
			return false;
		}
		conflictCheck = ((ElementAssignment)(ref assignment)).ToElementAssignmentConflictCheck();
		((ElementAssignmentConflictCheck)(ref conflictCheck)).playerId = currentPlayer.id;
		((ElementAssignmentConflictCheck)(ref conflictCheck)).controllerType = mapping.controllerType;
		((ElementAssignmentConflictCheck)(ref conflictCheck)).controllerId = mapping.controllerId;
		((ElementAssignmentConflictCheck)(ref conflictCheck)).controllerMapId = mapping.map.id;
		((ElementAssignmentConflictCheck)(ref conflictCheck)).controllerMapCategoryId = mapping.map.categoryId;
		if (mapping.aem != null)
		{
			((ElementAssignmentConflictCheck)(ref conflictCheck)).elementMapId = mapping.aem.id;
		}
		return true;
	}

	private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected I4, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		pollingInfo = default(ControllerPollingInfo);
		label = string.Empty;
		modifierKeyPressed = false;
		modifierFlags = (ModifierKeyFlags)0;
		int num = 0;
		ControllerPollingInfo val = default(ControllerPollingInfo);
		ControllerPollingInfo val2 = default(ControllerPollingInfo);
		ModifierKeyFlags val3 = (ModifierKeyFlags)0;
		foreach (ControllerPollingInfo item in ReInput.controllers.Keyboard.PollForAllKeys())
		{
			ControllerPollingInfo val4 = item;
			KeyCode keyboardKey = ((ControllerPollingInfo)(ref val4)).keyboardKey;
			if (keyboardKey == KeyCode.AltGr)
			{
				continue;
			}
			if (Keyboard.IsModifierKey(((ControllerPollingInfo)(ref val4)).keyboardKey))
			{
				if (num == 0)
				{
					val2 = val4;
				}
				val3 |= Keyboard.KeyCodeToModifierKeyFlags(keyboardKey);
				num++;
			}
			else if (((ControllerPollingInfo)(ref val)).keyboardKey == KeyCode.None)
			{
				val = val4;
			}
		}
		if (((ControllerPollingInfo)(ref val)).keyboardKey != KeyCode.None)
		{
			if (ReInput.controllers.Keyboard.GetKeyDown(((ControllerPollingInfo)(ref val)).keyboardKey))
			{
				if (num == 0)
				{
					pollingInfo = val;
					return;
				}
				pollingInfo = val;
				modifierFlags = (ModifierKeyFlags)(int)val3;
			}
		}
		else
		{
			if (num <= 0)
			{
				return;
			}
			modifierKeyPressed = true;
			if (num == 1)
			{
				if (ReInput.controllers.Keyboard.GetKeyTimePressed(((ControllerPollingInfo)(ref val2)).keyboardKey) > 1.0)
				{
					pollingInfo = val2;
				}
				else
				{
					label = Keyboard.GetKeyName(((ControllerPollingInfo)(ref val2)).keyboardKey);
				}
			}
			else
			{
				label = _language.ModifierKeyFlagsToString(val3);
			}
		}
	}

	private bool GetFirstElementAssignmentConflict(ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict, bool skipOtherPlayers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (GetFirstElementAssignmentConflict(currentPlayer, conflictCheck, out conflict))
		{
			return true;
		}
		if (GetFirstElementAssignmentConflict(ReInput.players.SystemPlayer, conflictCheck, out conflict))
		{
			return true;
		}
		if (!skipOtherPlayers)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player val = players[i];
				if (val != currentPlayer && GetFirstElementAssignmentConflict(val, conflictCheck, out conflict))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool GetFirstElementAssignmentConflict(Player player, ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		using (IEnumerator<ElementAssignmentConflictInfo> enumerator = player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				ElementAssignmentConflictInfo val = enumerator.Current;
				conflict = val;
				return true;
			}
		}
		conflict = default(ElementAssignmentConflictInfo);
		return false;
	}

	private void StartAxisCalibration(int axisIndex)
	{
		if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
		{
			Joystick val = currentJoystick;
			if (axisIndex >= 0 && axisIndex < ((ControllerWithAxes)val).axisCount)
			{
				pendingAxisCalibration = new AxisCalibrator(val, axisIndex);
				ShowCalibrateAxisStep1Window();
			}
		}
	}

	private void EndAxisCalibration()
	{
		if (pendingAxisCalibration != null)
		{
			pendingAxisCalibration.Commit();
			pendingAxisCalibration = null;
		}
	}

	private void SetUISelection(GameObject selection)
	{
		if (!(EventSystem.current == null))
		{
			EventSystem.current.SetSelectedGameObject(selection);
		}
	}

	private void RestoreLastUISelection()
	{
		if (lastUISelection == null || !lastUISelection.activeInHierarchy)
		{
			SetDefaultUISelection();
		}
		else
		{
			SetUISelection(lastUISelection);
		}
	}

	private void SetDefaultUISelection()
	{
		if (isOpen)
		{
			if (references.defaultSelection == null)
			{
				SetUISelection(null);
			}
			else
			{
				SetUISelection(references.defaultSelection.gameObject);
			}
		}
	}

	private void SelectDefaultMapCategory(bool redraw)
	{
		currentMapCategoryId = GetDefaultMapCategoryId();
		OnMapCategorySelected(currentMapCategoryId, redraw);
		if (!showMapCategories)
		{
			return;
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			if (ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId) != null)
			{
				currentMapCategoryId = _mappingSets[i].mapCategoryId;
				break;
			}
		}
		if (currentMapCategoryId >= 0)
		{
			for (int j = 0; j < _mappingSets.Length; j++)
			{
				bool state = ((_mappingSets[j].mapCategoryId != currentMapCategoryId) ? true : false);
				mapCategoryButtons[j].SetInteractible(state, playTransition: false);
			}
		}
	}

	private void CheckUISelection()
	{
		if (isFocused && currentUISelection == null)
		{
			RestoreLastUISelection();
		}
	}

	private void OnUIElementSelected(GameObject selectedObject)
	{
		lastUISelection = selectedObject;
	}

	private void SetIsFocused(bool state)
	{
		references.mainCanvasGroup.interactable = state;
		if (state)
		{
			Redraw(listsChanged: false, playTransitions: false);
			RestoreLastUISelection();
			blockInputOnFocusEndTime = Time.unscaledTime + 0.1f;
		}
	}

	public void Toggle()
	{
		if (isOpen)
		{
			Close(save: true);
		}
		else
		{
			Open();
		}
	}

	public void Open()
	{
		Open(force: false);
	}

	private void Open(bool force)
	{
		if (!initialized)
		{
			Initialize();
		}
		if (initialized && (force || !isOpen))
		{
			Clear();
			if (Instance == null)
			{
				Instance = this;
			}
			canvas.SetActive(value: true);
			OnPlayerSelected(0, redraw: false);
			SelectDefaultMapCategory(redraw: false);
			SetDefaultUISelection();
			Redraw(listsChanged: true, playTransitions: false);
			if (_ScreenOpenedEvent != null)
			{
				_ScreenOpenedEvent();
			}
			if (_onScreenOpened != null)
			{
				_onScreenOpened.Invoke();
			}
		}
	}

	public void Close(bool save)
	{
		if (initialized && isOpen)
		{
			if (save && ReInput.userDataStore != null)
			{
				ReInput.userDataStore.Save();
			}
			Clear();
			canvas.SetActive(value: false);
			SetUISelection(null);
			if (Instance == this)
			{
				Instance = null;
			}
			if (_ScreenClosedEvent != null)
			{
				_ScreenClosedEvent();
			}
			if (_onScreenClosed != null)
			{
				_onScreenClosed.Invoke();
			}
		}
	}

	private void Clear()
	{
		CloseAllWindows();
		lastUISelection = null;
		pendingInputMapping = null;
		pendingAxisCalibration = null;
		InputPollingStopped();
	}

	private void ClearCompletely()
	{
		Clear();
		ClearSpawnedObjects();
		ClearAllVars();
	}

	private void ClearSpawnedObjects()
	{
		windowManager.ClearCompletely();
		inputGrid.ClearAll();
		foreach (GUIButton playerButton in playerButtons)
		{
			UnityEngine.Object.Destroy(playerButton.gameObject);
		}
		playerButtons.Clear();
		foreach (GUIButton mapCategoryButton in mapCategoryButtons)
		{
			UnityEngine.Object.Destroy(mapCategoryButton.gameObject);
		}
		mapCategoryButtons.Clear();
		foreach (GUIButton assignedControllerButton in assignedControllerButtons)
		{
			UnityEngine.Object.Destroy(assignedControllerButton.gameObject);
		}
		assignedControllerButtons.Clear();
		if (assignedControllerButtonsPlaceholder != null)
		{
			UnityEngine.Object.Destroy(assignedControllerButtonsPlaceholder.gameObject);
			assignedControllerButtonsPlaceholder = null;
		}
		foreach (GameObject miscInstantiatedObject in miscInstantiatedObjects)
		{
			UnityEngine.Object.Destroy(miscInstantiatedObject);
		}
		miscInstantiatedObjects.Clear();
	}

	private void ClearVarsOnPlayerChange()
	{
		currentJoystickId = -1;
	}

	private void ClearVarsOnJoystickChange()
	{
		currentJoystickId = -1;
	}

	private void ClearAllVars()
	{
		initialized = false;
		if (Instance == this)
		{
			Instance = null;
		}
		playerCount = 0;
		inputGrid = null;
		windowManager = null;
		currentPlayerId = -1;
		currentMapCategoryId = -1;
		playerButtons = null;
		mapCategoryButtons = null;
		miscInstantiatedObjects = null;
		canvas = null;
		lastUISelection = null;
		currentJoystickId = -1;
		pendingInputMapping = null;
		pendingAxisCalibration = null;
		inputFieldActivatedDelegate = null;
		inputFieldInvertToggleStateChangedDelegate = null;
		isPollingForInput = false;
	}

	public void Reset()
	{
		if (initialized)
		{
			bool num = isOpen;
			Close(save: false);
			ClearCompletely();
			Initialize();
			if (num)
			{
				Open(force: true);
			}
		}
	}

	private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return;
		}
		ControllerMap controllerMap = GetControllerMap(controllerType);
		ControllerMapWithAxes val = (ControllerMapWithAxes)(object)((controllerMap is ControllerMapWithAxes) ? controllerMap : null);
		if (val != null)
		{
			ActionElementMap elementMap = ((ControllerMap)val).GetElementMap(actionElementMapId);
			if (elementMap != null)
			{
				elementMap.invert = state;
			}
		}
	}

	private ControllerMap GetControllerMap(ControllerType type)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected I4, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return null;
		}
		int num = 0;
		switch ((int)type)
		{
		case 2:
			if (currentPlayer.controllers.joystickCount > 0)
			{
				num = ((Controller)currentJoystick).id;
				break;
			}
			return null;
		default:
			throw new NotImplementedException();
		case 0:
		case 1:
			break;
		}
		return currentPlayer.controllers.maps.GetFirstMapInCategory(type, num, currentMapCategoryId);
	}

	private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ControllerMap val = GetControllerMap(controllerType);
		if (val == null)
		{
			currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, currentMapCategoryId, layoutId);
			val = currentPlayer.controllers.maps.GetMap(controllerType, controllerId, currentMapCategoryId, layoutId);
		}
		return val;
	}

	private int CountIEnumerable<T>(IEnumerable<T> enumerable)
	{
		if (enumerable == null)
		{
			return 0;
		}
		IEnumerator<T> enumerator = enumerable.GetEnumerator();
		if (enumerator == null)
		{
			return 0;
		}
		int num = 0;
		while (enumerator.MoveNext())
		{
			num++;
		}
		return num;
	}

	private int GetDefaultMapCategoryId()
	{
		if (_mappingSets.Length == 0)
		{
			return 0;
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			if (ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId) != null)
			{
				return _mappingSets[i].mapCategoryId;
			}
		}
		return 0;
	}

	private void SubscribeFixedUISelectionEvents()
	{
		if (references.fixedSelectableUIElements == null)
		{
			return;
		}
		GameObject[] fixedSelectableUIElements = references.fixedSelectableUIElements;
		for (int i = 0; i < fixedSelectableUIElements.Length; i++)
		{
			UIElementInfo component = UnityTools.GetComponent<UIElementInfo>(fixedSelectableUIElements[i]);
			if (!(component == null))
			{
				component.OnSelectedEvent += OnUIElementSelected;
			}
		}
	}

	private void SubscribeMenuControlInputEvents()
	{
		SubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
		SubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
		SubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
		SubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
	}

	private void UnsubscribeMenuControlInputEvents()
	{
		UnsubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
	}

	private void SubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
	{
		if (actionId < 0 || callback == null)
		{
			return;
		}
		if (ReInput.mapping.GetAction(actionId) == null)
		{
			Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
			return;
		}
		foreach (Player allPlayer in ReInput.players.AllPlayers)
		{
			allPlayer.AddInputEventDelegate(callback, (UpdateLoopType)0, (InputActionEventType)3, actionId);
		}
	}

	private void UnsubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
	{
		if (actionId < 0 || callback == null || !ReInput.isReady)
		{
			return;
		}
		if (ReInput.mapping.GetAction(actionId) == null)
		{
			Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
			return;
		}
		foreach (Player allPlayer in ReInput.players.AllPlayers)
		{
			allPlayer.RemoveInputEventDelegate(callback, (UpdateLoopType)0, (InputActionEventType)3, actionId);
		}
	}

	private int GetMaxControllersPerPlayer()
	{
		if (((InputManager_Base)_rewiredInputManager).userData.ConfigVars.autoAssignJoysticks)
		{
			return ((InputManager_Base)_rewiredInputManager).userData.ConfigVars.maxJoysticksPerPlayer;
		}
		return _maxControllersPerPlayer;
	}

	private bool ShowAssignedControllers()
	{
		if (!_showControllers)
		{
			return false;
		}
		if (_showAssignedControllers)
		{
			return true;
		}
		if (GetMaxControllersPerPlayer() != 1)
		{
			return true;
		}
		return false;
	}

	private void InspectorPropertyChanged(bool reset = false)
	{
		if (reset)
		{
			Reset();
		}
	}

	private void AssignController(Player player, int controllerId)
	{
		if (player == null || player.controllers.ContainsController((ControllerType)2, controllerId))
		{
			return;
		}
		if (GetMaxControllersPerPlayer() == 1)
		{
			RemoveAllControllers(player);
			ClearVarsOnJoystickChange();
		}
		foreach (Player player2 in ReInput.players.Players)
		{
			if (player2 != player)
			{
				RemoveController(player2, controllerId);
			}
		}
		player.controllers.AddController((ControllerType)2, controllerId, false);
		if (ReInput.userDataStore != null)
		{
			ReInput.userDataStore.LoadControllerData(player.id, (ControllerType)2, controllerId);
		}
	}

	private void RemoveAllControllers(Player player)
	{
		if (player != null)
		{
			IList<Joystick> joysticks = player.controllers.Joysticks;
			for (int num = joysticks.Count - 1; num >= 0; num--)
			{
				RemoveController(player, ((Controller)joysticks[num]).id);
			}
		}
	}

	private void RemoveController(Player player, int controllerId)
	{
		if (player != null && player.controllers.ContainsController((ControllerType)2, controllerId))
		{
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.SaveControllerData(player.id, (ControllerType)2, controllerId);
			}
			player.controllers.RemoveController((ControllerType)2, controllerId);
		}
	}

	private bool IsAllowedAssignment(InputMapping pendingInputMapping, ControllerPollingInfo pollingInfo)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		if (pendingInputMapping == null)
		{
			return false;
		}
		if ((int)pendingInputMapping.axisRange == 0 && !_showSplitAxisInputFields && (int)((ControllerPollingInfo)(ref pollingInfo)).elementType == 1)
		{
			return false;
		}
		return true;
	}

	private void InputPollingStarted()
	{
		bool num = isPollingForInput;
		isPollingForInput = true;
		if (!num)
		{
			if (_InputPollingStartedEvent != null)
			{
				_InputPollingStartedEvent();
			}
			if (_onInputPollingStarted != null)
			{
				_onInputPollingStarted.Invoke();
			}
		}
	}

	private void InputPollingStopped()
	{
		bool num = isPollingForInput;
		isPollingForInput = false;
		if (num)
		{
			if (_InputPollingEndedEvent != null)
			{
				_InputPollingEndedEvent();
			}
			if (_onInputPollingEnded != null)
			{
				_onInputPollingEnded.Invoke();
			}
		}
	}

	private int GetControllerInputFieldCount(ControllerType controllerType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected I4, but got Unknown
		return (int)controllerType switch
		{
			0 => _keyboardInputFieldCount, 
			1 => _mouseInputFieldCount, 
			2 => _controllerInputFieldCount, 
			_ => throw new NotImplementedException(), 
		};
	}

	private bool ShowSwapButton(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Invalid comparison between Unknown and I4
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		if (currentPlayer == null)
		{
			return false;
		}
		if (!_allowElementAssignmentSwap)
		{
			return false;
		}
		if (mapping == null || mapping.aem == null)
		{
			return false;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
			return false;
		}
		List<ElementAssignmentConflictInfo> list = new List<ElementAssignmentConflictInfo>();
		list.AddRange(currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
		list.AddRange(ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
		if (list.Count == 0)
		{
			return false;
		}
		ActionElementMap aem = mapping.aem;
		ElementAssignmentConflictInfo val = list[0];
		int actionId = ((ElementAssignmentConflictInfo)(ref val)).elementMap.actionId;
		Pole axisContribution = ((ElementAssignmentConflictInfo)(ref val)).elementMap.axisContribution;
		AxisRange val2 = aem.axisRange;
		ControllerElementType elementType = aem.elementType;
		if (elementType == ((ElementAssignmentConflictInfo)(ref val)).elementMap.elementType && (int)elementType == 0)
		{
			if (val2 != ((ElementAssignmentConflictInfo)(ref val)).elementMap.axisRange)
			{
				if ((int)val2 == 0)
				{
					val2 = (AxisRange)1;
				}
				else if ((int)((ElementAssignmentConflictInfo)(ref val)).elementMap.axisRange != 0)
				{
				}
			}
		}
		else if ((int)elementType == 0 && ((int)((ElementAssignmentConflictInfo)(ref val)).elementMap.elementType == 1 || ((int)((ElementAssignmentConflictInfo)(ref val)).elementMap.elementType == 0 && (int)((ElementAssignmentConflictInfo)(ref val)).elementMap.axisRange != 0)) && (int)val2 == 0)
		{
			val2 = (AxisRange)1;
		}
		int num = 0;
		if (assignment.actionId == ((ElementAssignmentConflictInfo)(ref val)).actionId && mapping.map == ((ElementAssignmentConflictInfo)(ref val)).controllerMap)
		{
			Controller controller = ReInput.controllers.GetController(mapping.controllerType, mapping.controllerId);
			if (SwapIsSameInputRange(elementType, val2, axisContribution, controller.GetElementById(assignment.elementIdentifierId).type, assignment.axisRange, assignment.axisContribution))
			{
				num++;
			}
		}
		foreach (ActionElementMap aem2 in ((ElementAssignmentConflictInfo)(ref val)).controllerMap.ElementMapsWithAction(actionId))
		{
			if (aem2.id != aem.id && list.FindIndex((ElementAssignmentConflictInfo x) => ((ElementAssignmentConflictInfo)(ref x)).elementMapId == aem2.id) < 0 && SwapIsSameInputRange(elementType, val2, axisContribution, aem2.elementType, aem2.axisRange, aem2.axisContribution))
			{
				num++;
			}
		}
		return num < GetControllerInputFieldCount(mapping.controllerType);
	}

	private bool SwapIsSameInputRange(ControllerElementType origElementType, AxisRange origAxisRange, Pole origAxisContribution, ControllerElementType conflictElementType, AxisRange conflictAxisRange, Pole conflictAxisContribution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((int)origElementType == 1 || ((int)origElementType == 0 && (int)origAxisRange != 0)) && ((int)conflictElementType == 1 || ((int)conflictElementType == 0 && (int)conflictAxisRange != 0)) && conflictAxisContribution == origAxisContribution)
		{
			return true;
		}
		if ((int)origElementType == 0 && (int)origAxisRange == 0 && (int)conflictElementType == 0 && (int)conflictAxisRange == 0)
		{
			return true;
		}
		return false;
	}

	private void ApplyTheme()
	{
		for (int i = 0; i < themedElements.Count; i++)
		{
			if (themedElements != null)
			{
				themedElements[i].ApplyTheme();
			}
		}
	}

	public static void Register(ThemedElement themedElement)
	{
		if (!(themedElement == null) && !(Instance == null) && !Instance.themedElements.Contains(themedElement))
		{
			Instance.themedElements.Add(themedElement);
		}
	}

	public static void Unregister(ThemedElement themedElement)
	{
		if (!(themedElement == null) && !(Instance == null))
		{
			Instance.themedElements.Remove(themedElement);
		}
	}

	public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo)
	{
		if (!(Instance == null) && !(Instance._themeSettings == null) && Instance._useThemeSettings)
		{
			Instance._themeSettings.Apply(elementInfo);
		}
	}

	public static LanguageDataBase GetLanguage()
	{
		if (Instance == null)
		{
			return null;
		}
		return Instance._language;
	}
}
