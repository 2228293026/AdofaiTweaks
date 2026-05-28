using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rewired.Glyphs;

[Serializable]
public class ControllerElementGlyphSelectorOptions
{
	[Serializable]
	public struct ControllerSelector
	{
		[Tooltip("The controller type of the Controller.")]
		[SerializeField]
		private ControllerType _controllerType;

		[Tooltip("The hardware type GUID of the Controller. This value is used to identify recognized Controllers.")]
		[SerializeField]
		private string _hardwareTypeGuid;

		[Tooltip("The hardware identifier of the Controller. This is used primarily by Unknown Controllers to identify the controller using various information gathered from the controller. This value varies depending on the platform, input source in use, and the device.")]
		[SerializeField]
		private string _hardwareIdentifier;

		[Tooltip("The list of Controller Map selectors. This provides necessary information about the Controller Maps to load.")]
		[SerializeField]
		private List<ControllerMapSelector> _controllerMapSelectors;

		[NonSerialized]
		private bool _isHardwareTypeGuidCached;

		[NonSerialized]
		private Guid _hardwareTypeGuidCache;

		public ControllerType controllerType
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _controllerType;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_controllerType = value;
			}
		}

		public Guid hardwareTypeGuid
		{
			get
			{
				if (_isHardwareTypeGuidCached)
				{
					return _hardwareTypeGuidCache;
				}
				UpdateHardwareTypeGuidCache();
				return _hardwareTypeGuidCache;
			}
			set
			{
				_hardwareTypeGuid = value.ToString();
				UpdateHardwareTypeGuidCache();
			}
		}

		public string hardwareIdentifier
		{
			get
			{
				return _hardwareIdentifier;
			}
			set
			{
				_hardwareIdentifier = value;
			}
		}

		public List<ControllerMapSelector> controllerMapSelectors
		{
			get
			{
				return _controllerMapSelectors;
			}
			set
			{
				_controllerMapSelectors = value;
			}
		}

		private void UpdateHardwareTypeGuidCache()
		{
			try
			{
				_hardwareTypeGuidCache = new Guid(_hardwareTypeGuid);
			}
			catch
			{
			}
			_isHardwareTypeGuidCached = true;
		}
	}

	[Serializable]
	public struct ControllerMapSelector
	{
		[Tooltip("The Controller Map Category name.")]
		[SerializeField]
		private string _mapCategoryName;

		[Tooltip("The Controller Map Layout name.")]
		[SerializeField]
		private string _layoutName;

		private const string errorMessage_notInitializedMemberAccess = "Rewired: Rewired must be initialized before accessing ";

		public string mapCategoryName
		{
			get
			{
				return _mapCategoryName;
			}
			set
			{
				_mapCategoryName = value;
			}
		}

		public string layoutName
		{
			get
			{
				return _layoutName;
			}
			set
			{
				_layoutName = value;
			}
		}

		public int mapCategoryId
		{
			get
			{
				if (!ReInput.isReady)
				{
					Debug.LogError(errorMessage_notInitialized_mapCategoryId);
					return -1;
				}
				return ReInput.mapping.GetMapCategoryId(_mapCategoryName);
			}
			set
			{
				if (!ReInput.isReady)
				{
					Debug.LogError(errorMessage_notInitialized_mapCategoryId);
					return;
				}
				InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(value);
				if (mapCategory != null)
				{
					_mapCategoryName = ((InputCategory)mapCategory).name;
					return;
				}
				Debug.LogError(errorMessage_invalidMapCategoryId + value);
				_mapCategoryName = string.Empty;
			}
		}

		private static string errorMessage_notInitialized_mapCategoryId => "Rewired: Rewired must be initialized before accessing " + typeof(ControllerMapSelector).FullName + ".mapCategoryId.";

		private static string errorMessage_notInitialized_layoutId => "Rewired: Rewired must be initialized before accessing " + typeof(ControllerMapSelector).FullName + ".layoutId.";

		private static string errorMessage_invalidMapCategoryId => "Rewired: Invalid map category id: ";

		private static string errorMessage_invalidLayoutId => "Rewired: Invalid layout id: ";

		public int GetLayoutId(ControllerType controllerType)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!ReInput.isReady)
			{
				Debug.LogError(errorMessage_notInitialized_layoutId);
				return -1;
			}
			InputLayout layout = ReInput.mapping.GetLayout(controllerType, _layoutName);
			if (layout == null)
			{
				return -1;
			}
			return layout.id;
		}

		public void SetLayoutId(ControllerType controllerType, int layoutId)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (!ReInput.isReady)
			{
				Debug.LogError(errorMessage_notInitialized_layoutId);
				return;
			}
			InputLayout layout = ReInput.mapping.GetLayout(controllerType, layoutId);
			if (layout != null)
			{
				_layoutName = layout.name;
				return;
			}
			Debug.LogError(errorMessage_invalidLayoutId + layoutId);
			_layoutName = string.Empty;
		}
	}

	private static readonly ControllerElementType[] s_defaultControllerElementTypeOrder = (ControllerElementType[])(object)new ControllerElementType[2]
	{
		default(ControllerElementType),
		(ControllerElementType)1
	};

	[Tooltip("Determines if the Player's last active controller is used for glyph selection.")]
	[SerializeField]
	private bool _useLastActiveController = true;

	[Tooltip("If enabled, results will be returned only for the first controller found that has at least one matching binding. This only has an effect on the result list returned when doing manual result selection or when selecting a result index greater than 0.This prevents results from being returned from multiple different devices, for example, when trying to get the second result from a single controller, excluding any other controllers such as the default controller. Note that Keyboard and Mouse are considered a single controller for the purposes of the glyph system.")]
	[SerializeField]
	private bool _useFirstControllerResults;

	[Tooltip("Controller type priority. First in list corresponds to highest priority. This determines which controller types take precedence when displaying glyphs. If use last active controller is enabled, the active controller will always take priority, however, if there is no last active controller, selection will fall back based on this priority. In addition, keyboard and mouse are treated as a single controller for the purposes of glyph handling, so to prioritze keyboard over mouse or vice versa, the one that is lower in the list will take precedence.")]
	[SerializeField]
	private ControllerType[] _controllerTypeOrder;

	[Tooltip("Controller element type priority. First in list corresponds to highest priority. This determines which controller element types take precedence when displaying glyphs.")]
	[SerializeField]
	private ControllerElementType[] _controllerElementTypeOrder;

	[Tooltip("If enabled, the default controllers will be used if no matching mappings are found in the Player for other controllers. The purpose of this is to allow glyphs to be displayed for a controller that is not connected. Controllers will be evaluated in the order in which they appear in the list.")]
	[SerializeField]
	private bool _useDefaultControllers;

	[Tooltip("Determines which controller will be used if no matching mappings are found in the Player for other controllers. The purpose of this is to allow glyphs to be displayed for a controller that is not connected. Use Default Controllers must be enabled for this to have any effect. Controllers will be evaluated in the order in which they appear in the list.\n\nFor recognized controllers, set only the Controller Type and the Hardware Type Guid, do not specify a Hardware Identifier which is only useful for unrecognized controllers and differs based on the platform and input source in use. The Hardware Type Guid of recognized controllers can be found in the Hardware Joystick Map controller definition located at Rewired/Internal/Data/Controllers/HardwareMaps/Joysticks/ or in the exported controllers CSV file which can be found in the glyphs documentation.")]
	[SerializeField]
	private List<ControllerSelector> _defaultControllers;

	[NonSerialized]
	private Predicate<ActionElementMap> _isActionElementMapAllowedHandler;

	private static ControllerElementGlyphSelectorOptions s_defaultOptions;

	public bool useLastActiveController
	{
		get
		{
			return _useLastActiveController;
		}
		set
		{
			_useLastActiveController = value;
		}
	}

	public bool useFirstControllerResults
	{
		get
		{
			return _useFirstControllerResults;
		}
		set
		{
			_useFirstControllerResults = value;
		}
	}

	public ControllerType[] controllerTypeOrder
	{
		get
		{
			return _controllerTypeOrder;
		}
		set
		{
			_controllerTypeOrder = value;
		}
	}

	public ControllerElementType[] controllerElementTypeOrder
	{
		get
		{
			return _controllerElementTypeOrder;
		}
		set
		{
			if (value == null || value.Length == 0)
			{
				value = (ControllerElementType[])s_defaultControllerElementTypeOrder.Clone();
			}
			_controllerElementTypeOrder = value;
		}
	}

	public bool useDefaultControllers
	{
		get
		{
			return _useDefaultControllers;
		}
		set
		{
			_useDefaultControllers = value;
		}
	}

	public List<ControllerSelector> defaultControllers
	{
		get
		{
			return _defaultControllers;
		}
		set
		{
			_defaultControllers = value;
		}
	}

	public Predicate<ActionElementMap> isActionElementMapAllowedHandler
	{
		get
		{
			return _isActionElementMapAllowedHandler;
		}
		set
		{
			_isActionElementMapAllowedHandler = value;
		}
	}

	public static ControllerElementGlyphSelectorOptions defaultOptions
	{
		get
		{
			if (s_defaultOptions == null)
			{
				return s_defaultOptions = new ControllerElementGlyphSelectorOptions();
			}
			return s_defaultOptions;
		}
		set
		{
			s_defaultOptions = value;
		}
	}

	public virtual bool TryGetControllerTypeOrder(int index, out ControllerType controllerType)
	{
		if ((uint)index >= (uint)_controllerTypeOrder.Length)
		{
			controllerType = (ControllerType)0;
			return false;
		}
		controllerType = _controllerTypeOrder[index];
		return true;
	}

	public ControllerElementGlyphSelectorOptions()
	{
		ControllerType[] array = new ControllerType[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		_controllerTypeOrder = (ControllerType[])(object)array;
		_controllerElementTypeOrder = (ControllerElementType[])s_defaultControllerElementTypeOrder.Clone();
		base._002Ector();
	}
}
