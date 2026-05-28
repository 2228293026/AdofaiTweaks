using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Glyphs.UnityUI;

public abstract class UnityUIPlayerControllerElementGlyphBase : UnityUIControllerElementGlyphBase
{
	public delegate int ResultSelectionHandler(IList<ActionElementMapPair> results);

	public delegate int Result2DSelectionHandler(IList<Pair<ActionElementMapPair>> results);

	[Tooltip("Optional reference to an object that defines options. If blank, the global default options will be used.")]
	[SerializeField]
	private ControllerElementGlyphSelectorOptionsSOBase _options;

	[Tooltip("The range of the Action for which to show glyphs / text. This determines whether to show the glyph for an axis-type Action (ex: Move Horizontal), or the positive/negative pole of an Action (ex: Move Right). For button-type Actions, Full and Positive are equivalent. This value has no effect when displaying two Actions.")]
	[SerializeField]
	private AxisRange _actionRange;

	[Tooltip("Optional parent Transform of the first group of instantiated glyph / text objects. For a single Action query, if an axis-type Action is bound to multiple elements, the glyphs bound to the negative pole of the Action will be instantiated under this Transform. For a two Action query, if multiple glyphs are returned, the glyphs bound to the first Action will be instantiated under this Transform. If a single glyph is returned, it will be instantiated under this Transform as well.This allows you to separate results by negative / positive binding or Action 1 / Action 2 in order to stack glyph groups horizontally or vertically, for example. If an Action is only bound to one element, the glyph will be instantiated under this transform. If blank, objects will be created as children of this object's Transform.")]
	[SerializeField]
	private Transform _group1;

	[Tooltip("Optional parent Transform of the second group of instantiated glyph / text objects. For a single Action query, if an axis-type Action is bound to multiple elements, the glyphs bound to the positive pole of the Action will be instantiated under this Transform. For a two Action query, if multiple glyphs are returned, the glyphs bound to the second Action will be instantiated under this Transform unless there were no results found for the first Action, in which case they will be displayed under group1. Otherwise, if a single glyph is returned, it will be instantiated under group1.This allows you to separate results by negative / positive binding or Action 1 / Action 2 in order to stack glyph groups horizontally or vertically, for example. If an Action is only bound to one element, the glyph will be instantiated under group1 instead. If blank, objects will be created as children of either group1 if set or the object's Transform.")]
	[SerializeField]
	private Transform _group2;

	[Tooltip("The index of the result to return. This can be used to return, for example, the second matching glpyh(s) instead of the first found. This will be ignored if you are using a custom result selector.")]
	[SerializeField]
	private int _resultIndex;

	[Tooltip("Determines the display order of split-axis and button glyphs for the first Action.When two glyphs for an axis-type action are displayed, this determines which pole is displayed first.")]
	[SerializeField]
	private Pole _action1FirstPole = (Pole)1;

	[Tooltip("Determines the display order of split-axis and button glyphs for the second Action.When two glyphs for an axis-type action are displayed, this determines which pole is displayed first.")]
	[SerializeField]
	private Pole _action2FirstPole = (Pole)1;

	[NonSerialized]
	private List<Pair<ActionElementMapPair>> _temp2dResults;

	[NonSerialized]
	private List<ActionElementMap> _tempCombinedElementAems = new List<ActionElementMap>();

	[NonSerialized]
	private List<ActionElementMapPair> _tempResults;

	[NonSerialized]
	private readonly List<GlyphOrTextObject> _group1Objects = new List<GlyphOrTextObject>();

	[NonSerialized]
	private readonly List<GlyphOrTextObject> _group2Objects = new List<GlyphOrTextObject>();

	[NonSerialized]
	private ResultSelectionHandler _resultSelectionHandler;

	[NonSerialized]
	private Result2DSelectionHandler _result2dSelectionHandler;

	public virtual ControllerElementGlyphSelectorOptionsSOBase options
	{
		get
		{
			return _options;
		}
		set
		{
			_options = value;
			RequireRebuild();
		}
	}

	public abstract int playerId { get; set; }

	public abstract int actionId { get; set; }

	public abstract int actionId2 { get; set; }

	public virtual AxisRange actionRange
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _actionRange;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_actionRange = value;
		}
	}

	public virtual Transform group1
	{
		get
		{
			return _group1;
		}
		set
		{
			_group1 = value;
			RequireRebuild();
		}
	}

	public virtual Transform group2
	{
		get
		{
			return _group2;
		}
		set
		{
			_group2 = value;
			RequireRebuild();
		}
	}

	public int resultIndex
	{
		get
		{
			return _resultIndex;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			_resultIndex = value;
		}
	}

	public Pole action1FirstPole
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _action1FirstPole;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_action1FirstPole = value;
		}
	}

	public Pole action2FirstPole
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _action2FirstPole;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_action2FirstPole = value;
		}
	}

	public virtual ResultSelectionHandler resultSelectionHandler
	{
		get
		{
			return _resultSelectionHandler;
		}
		set
		{
			_resultSelectionHandler = value;
		}
	}

	public virtual Result2DSelectionHandler result2dSelectionHandler
	{
		get
		{
			return _result2dSelectionHandler;
		}
		set
		{
			_result2dSelectionHandler = value;
		}
	}

	protected virtual bool isMousePrioritizedOverKeyboard
	{
		get
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			ControllerType controllerType;
			for (int i = 0; TryGetControllerTypeOrder(i, out controllerType); i++)
			{
				if ((int)controllerType == 1)
				{
					return true;
				}
				if ((int)controllerType == 0)
				{
					return false;
				}
			}
			return false;
		}
	}

	protected virtual bool TryGetControllerTypeOrder(int index, out ControllerType controllerType)
	{
		return GetOptionsOrDefault().TryGetControllerTypeOrder(index, out controllerType);
	}

	protected override void Update()
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!ReInput.isReady)
		{
			return;
		}
		ActionElementMapPair aemResult = default(ActionElementMapPair);
		ActionElementMapPair aemResult2 = default(ActionElementMapPair);
		ControllerElementGlyphSelectorOptions optionsOrDefault = GetOptionsOrDefault();
		bool flag = false;
		bool flag2 = actionId2 >= 0 && actionId2 != actionId;
		if (flag2)
		{
			actionRange = (AxisRange)0;
		}
		if ((flag2 ? (_result2dSelectionHandler != null) : (_resultSelectionHandler != null)) || _resultIndex > 0)
		{
			if (flag2)
			{
				if (_temp2dResults == null)
				{
					_temp2dResults = new List<Pair<ActionElementMapPair>>();
				}
				flag |= TryGetActionElementMaps(playerId, actionId, actionId2, optionsOrDefault, _resultIndex, _result2dSelectionHandler, _temp2dResults, out aemResult, out aemResult2);
				_temp2dResults.Clear();
			}
			else
			{
				if (_tempResults == null)
				{
					_tempResults = new List<ActionElementMapPair>();
				}
				flag |= TryGetActionElementMaps(playerId, actionId, actionRange, optionsOrDefault, _resultIndex, _resultSelectionHandler, _tempResults, out aemResult);
				_tempResults.Clear();
			}
		}
		else if (flag2)
		{
			flag |= GlyphTools.TryGetActionElementMaps(playerId, actionId, actionId2, optionsOrDefault, null, out aemResult, out aemResult2);
		}
		else
		{
			flag |= GlyphTools.TryGetActionElementMaps(playerId, actionId, actionRange, optionsOrDefault, null, out var aemResult3, out var aemResult4);
			aemResult = new ActionElementMapPair(aemResult3, aemResult4);
		}
		if (!flag)
		{
			Hide();
		}
		else if (flag2 && (aemResult.Count > 0 || aemResult2.Count > 0))
		{
			ShowAction2DBindings(aemResult, aemResult2);
		}
		else if (aemResult.a != null && aemResult.b != null)
		{
			ShowSplitAxisBindings(aemResult.a, aemResult.b);
		}
		else if (aemResult.a != null)
		{
			ShowBinding(aemResult.a);
		}
		else if (aemResult.b != null)
		{
			ShowBinding(aemResult.b);
		}
	}

	private static bool TryGetActionElementMaps(int playerId, int actionId, AxisRange actionRange, ControllerElementGlyphSelectorOptions options, int resultIndex, ResultSelectionHandler resultSelectionHandler, List<ActionElementMapPair> tempResults, out ActionElementMapPair result)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		tempResults.Clear();
		GlyphTools.GetActionElementMaps(playerId, actionId, actionRange, options, null, tempResults);
		try
		{
			if (resultSelectionHandler != null)
			{
				resultIndex = resultSelectionHandler(tempResults);
			}
			else if (resultIndex <= 0)
			{
				result = default(ActionElementMapPair);
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Rewired: An exception was thrown in resultSortingHandler callback. This exception was thrown by your code.\n" + ex);
			result = default(ActionElementMapPair);
			return false;
		}
		if (resultIndex < 0 || resultIndex >= tempResults.Count)
		{
			result = default(ActionElementMapPair);
			return false;
		}
		result = tempResults[resultIndex];
		return tempResults[resultIndex].Count > 0;
	}

	private static bool TryGetActionElementMaps(int playerId, int actionId, int actionId2, ControllerElementGlyphSelectorOptions options, int resultIndex, Result2DSelectionHandler resultSelectionHandler, List<Pair<ActionElementMapPair>> tempResults, out ActionElementMapPair action1Result, out ActionElementMapPair action2Result)
	{
		action1Result = default(ActionElementMapPair);
		action2Result = default(ActionElementMapPair);
		tempResults.Clear();
		GlyphTools.GetActionElementMaps(playerId, actionId, actionId2, options, null, tempResults);
		if (tempResults.Count != 0)
		{
			try
			{
				if (resultSelectionHandler != null)
				{
					resultIndex = resultSelectionHandler(tempResults);
				}
				else if (resultIndex <= 0)
				{
					goto IL_00a1;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Rewired: An exception was thrown in resultSortingHandler callback. This exception was thrown by your code.\n" + ex);
				goto IL_00a1;
			}
			if (resultIndex >= 0 && resultIndex < tempResults.Count)
			{
				action1Result = tempResults[resultIndex].a;
				action2Result = tempResults[resultIndex].b;
			}
		}
		goto IL_00a1;
		IL_00a1:
		tempResults.Clear();
		if (action1Result.Count <= 0)
		{
			return action2Result.Count > 0;
		}
		return true;
	}

	protected override void ClearObjects()
	{
		_group1Objects.Clear();
		_group2Objects.Clear();
		base.ClearObjects();
	}

	protected virtual bool ShowBinding(ActionElementMap actionElementMap)
	{
		if (actionElementMap == null)
		{
			return false;
		}
		int num = ShowGlyphsOrText(actionElementMap, GetObjectGroupTransform(0), _group1Objects);
		EvaluateObjectVisibility();
		return num > 0;
	}

	protected virtual bool ShowSplitAxisBindings(ActionElementMap negativeAem, ActionElementMap positiveAem)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		if (negativeAem == null && positiveAem == null)
		{
			return false;
		}
		int num = 0;
		if (negativeAem != null && positiveAem != null)
		{
			_tempCombinedElementAems.Clear();
			_tempCombinedElementAems.Add(negativeAem);
			_tempCombinedElementAems.Add(positiveAem);
			num = ShowGlyphsOrText(_tempCombinedElementAems, GetObjectGroupTransform(0), _group1Objects);
		}
		if (num == 0)
		{
			num += ShowGlyphsOrText(((int)_action1FirstPole == 1) ? negativeAem : positiveAem, GetObjectGroupTransform(0), _group1Objects);
			num += ShowGlyphsOrText(((int)_action1FirstPole == 1) ? positiveAem : negativeAem, GetObjectGroupTransform(1), _group2Objects);
		}
		EvaluateObjectVisibility();
		return num > 0;
	}

	protected virtual bool ShowAction2DBindings(ActionElementMapPair result1, ActionElementMapPair result2)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (result1.Count == 0 && result2.Count == 0)
		{
			return false;
		}
		int currentUsedObjectCount = 0;
		int groupObjectCount = 0;
		_tempCombinedElementAems.Clear();
		if (result1.a != null)
		{
			_tempCombinedElementAems.Add(result1.a);
		}
		if (result1.b != null)
		{
			_tempCombinedElementAems.Add(result1.b);
		}
		if (result2.a != null)
		{
			_tempCombinedElementAems.Add(result2.a);
		}
		if (result2.b != null)
		{
			_tempCombinedElementAems.Add(result2.b);
		}
		int num = ShowGlyphsOrText(_tempCombinedElementAems, GetObjectGroupTransform(0), _group1Objects, ref currentUsedObjectCount);
		if (num == 0)
		{
			if (result1.Count > 0 && result2.Count > 0)
			{
				int num2 = ShowAction2dBindings_ShowResultBindings(0, _action1FirstPole, result1, ref currentUsedObjectCount);
				num = ((num2 != 0) ? (num + ShowAction2dBindings_ShowResultBindings(1, _action2FirstPole, result2, ref groupObjectCount)) : (num + ShowAction2dBindings_ShowResultBindings(0, _action2FirstPole, result2, ref currentUsedObjectCount)));
				num += num2;
			}
			else if (result1.Count > 0)
			{
				num += ShowAction2dBindings_ShowResultBindings(0, _action1FirstPole, result1, ref currentUsedObjectCount);
			}
			else if (result2.Count > 0)
			{
				num += ShowAction2dBindings_ShowResultBindings(0, _action2FirstPole, result2, ref currentUsedObjectCount);
			}
		}
		EvaluateObjectVisibility();
		return num > 0;
	}

	private int ShowAction2dBindings_ShowResultBindings(int groupIndex, Pole poleOrder, ActionElementMapPair result, ref int groupObjectCount)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		if (groupIndex > 1)
		{
			throw new ArgumentOutOfRangeException("groupIndex");
		}
		int num = 0;
		List<GlyphOrTextObject> objects = ((groupIndex == 0) ? _group1Objects : _group2Objects);
		if (result.a != null && result.b != null)
		{
			_tempCombinedElementAems.Clear();
			_tempCombinedElementAems.Add(result.a);
			_tempCombinedElementAems.Add(result.b);
			int num2 = ShowGlyphsOrText(_tempCombinedElementAems, GetObjectGroupTransform(groupIndex), objects, ref groupObjectCount);
			num += num2;
			if (num2 > 0)
			{
				return num;
			}
		}
		ActionElementMap val;
		ActionElementMap val2;
		if ((int)poleOrder == 1)
		{
			val = result.a;
			val2 = result.b;
		}
		else
		{
			val = result.b;
			val2 = result.a;
		}
		if (val != null)
		{
			num += ShowGlyphsOrText(val, GetObjectGroupTransform(groupIndex), objects, ref groupObjectCount);
		}
		if (val2 != null)
		{
			num += ShowGlyphsOrText(val2, GetObjectGroupTransform(groupIndex), objects, ref groupObjectCount);
		}
		return num;
	}

	protected override void EvaluateObjectVisibility()
	{
		base.EvaluateObjectVisibility();
		Transform objectGroupTransform = GetObjectGroupTransform(0);
		Transform objectGroupTransform2 = GetObjectGroupTransform(1);
		if (objectGroupTransform == objectGroupTransform2)
		{
			EvaluateObjectVisibility(objectGroupTransform);
			return;
		}
		EvaluateObjectVisibility(objectGroupTransform, _group1Objects);
		EvaluateObjectVisibility(objectGroupTransform2, _group2Objects);
	}

	protected virtual int ShowGlyphsOrText(IList<ActionElementMap> bindings, Transform parent, List<GlyphOrTextObject> objects)
	{
		int currentUsedObjectCount = 0;
		return ShowGlyphsOrText(bindings, parent, objects, ref currentUsedObjectCount);
	}

	protected virtual int ShowGlyphsOrText(IList<ActionElementMap> bindings, Transform parent, List<GlyphOrTextObject> objects, ref int currentUsedObjectCount)
	{
		if (bindings == null)
		{
			return 0;
		}
		if (currentUsedObjectCount < 0)
		{
			currentUsedObjectCount = 0;
		}
		object glyph = default(object);
		if (IsAllowed(AllowedTypes.Glyphs) && ActionElementMap.TryGetCombinedElementIdentifierGlyph(bindings, ref glyph))
		{
			if (!CreateObjectsAsNeeded(parent, objects, currentUsedObjectCount + 1))
			{
				return 0;
			}
			objects[currentUsedObjectCount].ShowGlyph(glyph);
			currentUsedObjectCount++;
			return 1;
		}
		string text = default(string);
		if (IsAllowed(AllowedTypes.Text) && ActionElementMap.TryGetCombinedElementIdentifierName(bindings, ref text))
		{
			if (!CreateObjectsAsNeeded(parent, objects, currentUsedObjectCount + 1))
			{
				return 0;
			}
			objects[currentUsedObjectCount].ShowText(text);
			currentUsedObjectCount++;
			return 1;
		}
		return 0;
	}

	protected override void Hide()
	{
		base.Hide();
		if (_group1 != null && _group1 != base.transform)
		{
			_group1.gameObject.SetActive(value: false);
		}
		if (_group2 != null && _group2 != base.transform)
		{
			_group2.gameObject.SetActive(value: false);
		}
	}

	protected virtual Transform GetObjectGroupTransform(int groupIndex)
	{
		switch (groupIndex)
		{
		default:
			throw new ArgumentOutOfRangeException();
		case 1:
			if (groupIndex == 1)
			{
				if (_group1 == null)
				{
					return base.transform;
				}
				if (_group2 != null)
				{
					return _group2;
				}
				if (_group1 != null)
				{
					return _group1;
				}
				return base.transform;
			}
			throw new NotImplementedException();
		case 0:
			if (!(_group1 != null))
			{
				return base.transform;
			}
			return _group1;
		}
	}

	protected virtual ControllerElementGlyphSelectorOptions GetOptionsOrDefault()
	{
		if (_options != null && _options.options == null)
		{
			Debug.LogError("Rewired: Options missing on " + typeof(ControllerElementGlyphSelectorOptions).Name + ". Global default options will be used instead.");
			return ControllerElementGlyphSelectorOptions.defaultOptions;
		}
		if (!(_options != null))
		{
			return ControllerElementGlyphSelectorOptions.defaultOptions;
		}
		return _options.options;
	}
}
