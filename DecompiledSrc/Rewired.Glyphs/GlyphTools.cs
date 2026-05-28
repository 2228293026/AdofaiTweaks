using System;
using System.Collections.Generic;
using Rewired.Internal.Helpers;
using UnityEngine;

namespace Rewired.Glyphs;

public static class GlyphTools
{
	private static class Action2DHelper
	{
		private enum Result
		{
			GoNext,
			Quit
		}

		private static readonly FastList<bool> _usedAction1Aems = new FastList<bool>(16);

		private static readonly FastList<bool> _usedAction2Aems = new FastList<bool>(16);

		private static FastList<ActionElementMap> _action1Aems;

		private static FastList<ActionElementMap> _action2Aems;

		private static ControllerElementType[] _controllerElementTypeOrder;

		private static List<Pair<ActionElementMapPair>> _results;

		private static int _resultsRemainingCount;

		private static Func<Result>[] __steps_axisPriority;

		private static Func<Result>[] __steps_buttonPriority;

		private static Func<Result>[] steps_axisPriority
		{
			get
			{
				if (__steps_axisPriority == null)
				{
					__steps_axisPriority = new Func<Result>[7] { GetCompleteFullAxisPairs, GetMixedFullAxisAndSplitAxisPairs, GetCompleteSplitAxisQuadSets, GetCompleteButtonQuadSets, GetMixedFullAxisAndButtonPairs, GetMixedSplitAxisAndButtonPairs, GetRemaining };
				}
				return __steps_axisPriority;
			}
		}

		private static Func<Result>[] steps_buttonPriority
		{
			get
			{
				if (__steps_buttonPriority == null)
				{
					__steps_buttonPriority = new Func<Result>[7] { GetCompleteButtonQuadSets, GetCompleteFullAxisPairs, GetMixedFullAxisAndSplitAxisPairs, GetCompleteSplitAxisQuadSets, GetMixedFullAxisAndButtonPairs, GetMixedSplitAxisAndButtonPairs, GetRemaining };
				}
				return __steps_buttonPriority;
			}
		}

		private static ControllerElementType elementTypePriority
		{
			get
			{
				if ((int)_controllerElementTypeOrder[0] == 1)
				{
					return (ControllerElementType)1;
				}
				return (ControllerElementType)0;
			}
		}

		public static int GetActionElementMaps(FastList<ActionElementMap> action1Aems, FastList<ActionElementMap> action2Aems, ControllerElementType[] controllerElementTypeOrder, List<Pair<ActionElementMapPair>> results, ref int resultsRemainingCount)
		{
			_action1Aems = action1Aems;
			_action2Aems = action2Aems;
			_controllerElementTypeOrder = controllerElementTypeOrder;
			_results = results;
			_resultsRemainingCount = resultsRemainingCount;
			int count = results.Count;
			_usedAction1Aems.SetCount(action1Aems.Count);
			_usedAction2Aems.SetCount(action2Aems.Count);
			try
			{
				Func<Result>[] steps = GetSteps();
				for (int i = 0; i < steps.Length && steps[i]() != Result.Quit; i++)
				{
				}
				return results.Count - count;
			}
			finally
			{
				resultsRemainingCount = _resultsRemainingCount;
				_usedAction1Aems.Clear();
				_usedAction2Aems.Clear();
			}
		}

		private static Result GetCompleteFullAxisPairs()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Invalid comparison between Unknown and I4
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			FastList<ActionElementMap> action1Aems = _action1Aems;
			FastList<bool> usedAction1Aems = _usedAction1Aems;
			FastList<bool> usedAction2Aems = _usedAction2Aems;
			int count = action1Aems.Count;
			for (int i = 0; i < count; i++)
			{
				if (usedAction1Aems.Array[i])
				{
					continue;
				}
				ActionElementMap val = action1Aems.Array[i];
				ActionElementMap a;
				if ((int)val.elementType == 0 && (int)val.axisType == 1 && (a = Find(_action2Aems, 0, (ControllerElementType)0, (AxisType)1, out var index, usedAction2Aems)) != null)
				{
					_results.Add(new Pair<ActionElementMapPair>(new ActionElementMapPair(val, null), new ActionElementMapPair(a, null)));
					usedAction1Aems.Array[i] = true;
					usedAction2Aems.Array[index] = true;
					if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
					{
						return Result.Quit;
					}
				}
			}
			return Result.GoNext;
		}

		private static Result GetMixedFullAxisAndSplitAxisPairs()
		{
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			int num = 0;
			int num2 = 0;
			do
			{
				FastList<ActionElementMap> fastList;
				FastList<ActionElementMap> list;
				FastList<bool> fastList2;
				FastList<bool> fastList3;
				if (Find(_action1Aems, num, (ControllerElementType)0, (AxisType)1, out var index, _usedAction1Aems) != null)
				{
					fastList = _action1Aems;
					list = _action2Aems;
					num = index;
					fastList2 = _usedAction1Aems;
					fastList3 = _usedAction2Aems;
				}
				else
				{
					if (Find(_action2Aems, num2, (ControllerElementType)0, (AxisType)1, out index, _usedAction2Aems) == null)
					{
						break;
					}
					fastList = _action2Aems;
					list = _action1Aems;
					num2 = index;
					fastList2 = _usedAction2Aems;
					fastList3 = _usedAction1Aems;
				}
				ActionElementMap a = fastList.Array[index];
				ActionElementMap a2;
				ActionElementMap b;
				if ((a2 = Find(list, 0, (ControllerElementType)0, (AxisType)2, (Pole)1, out var index2, fastList3)) != null && (b = Find(list, 0, (ControllerElementType)0, (AxisType)2, (Pole)0, out var index3, fastList3)) != null)
				{
					_results.Add(Create(new ActionElementMapPair(a, null), new ActionElementMapPair(a2, b), fastList == _action2Aems));
					fastList2.Array[index] = true;
					fastList3.Array[index2] = true;
					fastList3.Array[index3] = true;
					if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
					{
						return Result.Quit;
					}
				}
				if (fastList == _action1Aems)
				{
					num = index + 1;
				}
				else
				{
					num2 = index + 1;
				}
			}
			while (num < _action1Aems.Count && num2 < _action2Aems.Count);
			return Result.GoNext;
		}

		private static Result GetCompleteSplitAxisQuadSets()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			FastList<ActionElementMap> action1Aems = _action1Aems;
			FastList<ActionElementMap> action2Aems = _action2Aems;
			FastList<bool> usedAction1Aems = _usedAction1Aems;
			FastList<bool> usedAction2Aems = _usedAction2Aems;
			int count = action1Aems.Count;
			for (int i = 0; i < count; i++)
			{
				if (usedAction1Aems.Array[i])
				{
					continue;
				}
				ActionElementMap val = action1Aems.Array[i];
				ActionElementMap b;
				ActionElementMap a;
				ActionElementMap b2;
				if ((int)val.elementType == 0 && (int)val.axisType == 2 && (int)val.axisContribution == 1 && (b = Find(action1Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)0, out var index, usedAction1Aems)) != null && (a = Find(action2Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)1, out var index2, usedAction2Aems)) != null && (b2 = Find(action2Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)0, out var index3, usedAction2Aems)) != null)
				{
					_results.Add(new Pair<ActionElementMapPair>(new ActionElementMapPair(val, b), new ActionElementMapPair(a, b2)));
					usedAction1Aems.Array[i] = true;
					usedAction1Aems.Array[index] = true;
					usedAction2Aems.Array[index2] = true;
					usedAction2Aems.Array[index3] = true;
					if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
					{
						return Result.Quit;
					}
				}
			}
			return Result.GoNext;
		}

		private static Result GetCompleteButtonQuadSets()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Invalid comparison between Unknown and I4
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			FastList<ActionElementMap> action1Aems = _action1Aems;
			FastList<ActionElementMap> action2Aems = _action2Aems;
			FastList<bool> usedAction1Aems = _usedAction1Aems;
			FastList<bool> usedAction2Aems = _usedAction2Aems;
			int count = action1Aems.Count;
			for (int i = 0; i < count; i++)
			{
				if (usedAction1Aems.Array[i])
				{
					continue;
				}
				ActionElementMap val = _action1Aems.Array[i];
				ActionElementMap b;
				ActionElementMap a;
				ActionElementMap b2;
				if ((int)val.elementType == 1 && (int)val.axisContribution == 1 && (b = Find(action1Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)0, out var index, usedAction1Aems)) != null && (a = Find(action2Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)1, out var index2, usedAction2Aems)) != null && (b2 = Find(action2Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)0, out var index3, usedAction2Aems)) != null)
				{
					_results.Add(new Pair<ActionElementMapPair>(new ActionElementMapPair(val, b), new ActionElementMapPair(a, b2)));
					usedAction1Aems.Array[i] = true;
					usedAction1Aems.Array[index] = true;
					usedAction2Aems.Array[index2] = true;
					usedAction2Aems.Array[index3] = true;
					if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
					{
						return Result.Quit;
					}
				}
			}
			return Result.GoNext;
		}

		private static Result GetMixedFullAxisAndButtonPairs()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			int num = 0;
			int num2 = 0;
			int index;
			int index3;
			ActionElementMapPair result;
			if ((int)elementTypePriority == 1)
			{
				do
				{
					FastList<ActionElementMap> fastList;
					FastList<ActionElementMap> list;
					FastList<bool> fastList2;
					FastList<bool> fastList3;
					if (Find(_action1Aems, num, (ControllerElementType)1, (AxisType)0, out index, _usedAction1Aems) != null)
					{
						fastList = _action1Aems;
						list = _action2Aems;
						num = index;
						fastList2 = _usedAction1Aems;
						fastList3 = _usedAction2Aems;
					}
					else
					{
						if (Find(_action2Aems, num2, (ControllerElementType)1, (AxisType)0, out index, _usedAction2Aems) == null)
						{
							break;
						}
						fastList = _action2Aems;
						list = _action1Aems;
						num2 = index;
						fastList2 = _usedAction2Aems;
						fastList3 = _usedAction1Aems;
					}
					ActionElementMap val = fastList.Array[index];
					ActionElementMap aem;
					ActionElementMap a;
					if ((aem = Find(fastList, 0, (ControllerElementType)1, (AxisType)0, (Pole)((int)val.axisContribution == 0), out var index2, fastList2)) != null && (a = Find(list, 0, (ControllerElementType)0, (AxisType)1, out index3, fastList3)) != null && TryCreate(val, aem, out result))
					{
						_results.Add(Create(result, new ActionElementMapPair(a, null), fastList == _action2Aems));
						fastList2.Array[index] = true;
						fastList2.Array[index2] = true;
						fastList3.Array[index3] = true;
						if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
						{
							return Result.Quit;
						}
					}
					if (fastList == _action1Aems)
					{
						num = index + 1;
					}
					else
					{
						num2 = index + 1;
					}
				}
				while (num < _action1Aems.Count && num2 < _action2Aems.Count);
			}
			else
			{
				do
				{
					FastList<ActionElementMap> fastList;
					FastList<ActionElementMap> list;
					FastList<bool> fastList2;
					FastList<bool> fastList3;
					if (Find(_action1Aems, num, (ControllerElementType)0, (AxisType)1, out index, _usedAction1Aems) != null)
					{
						fastList = _action1Aems;
						list = _action2Aems;
						num = index;
						fastList2 = _usedAction1Aems;
						fastList3 = _usedAction2Aems;
					}
					else
					{
						if (Find(_action2Aems, num2, (ControllerElementType)0, (AxisType)1, out index, _usedAction2Aems) == null)
						{
							break;
						}
						fastList = _action2Aems;
						list = _action1Aems;
						num2 = index;
						fastList2 = _usedAction2Aems;
						fastList3 = _usedAction1Aems;
					}
					ActionElementMap val = fastList.Array[index];
					ActionElementMap a;
					ActionElementMap aem2;
					if ((a = Find(list, 0, (ControllerElementType)1, (AxisType)0, (Pole)1, out index3, fastList3)) != null && (aem2 = Find(list, 0, (ControllerElementType)1, (AxisType)0, (Pole)0, out var index4, fastList3)) != null && TryCreate(a, aem2, out result))
					{
						_results.Add(Create(new ActionElementMapPair(val, null), result, fastList == _action2Aems));
						fastList2.Array[index] = true;
						fastList3.Array[index3] = true;
						fastList3.Array[index4] = true;
						if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
						{
							return Result.Quit;
						}
					}
					if (fastList == _action1Aems)
					{
						num = index + 1;
					}
					else
					{
						num2 = index + 1;
					}
				}
				while (num < _action1Aems.Count && num2 < _action2Aems.Count);
			}
			return Result.GoNext;
		}

		private static Result GetMixedSplitAxisAndButtonPairs()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Invalid comparison between Unknown and I4
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Invalid comparison between Unknown and I4
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			_ = _action1Aems;
			_ = _action2Aems;
			FastList<bool> usedAction1Aems = _usedAction1Aems;
			FastList<bool> usedAction2Aems = _usedAction2Aems;
			int count = _action1Aems.Count;
			for (int i = 0; i < count; i++)
			{
				if (usedAction1Aems.Array[i])
				{
					continue;
				}
				ActionElementMap val = _action1Aems.Array[i];
				int index;
				int index2;
				int index3;
				ActionElementMap b;
				ActionElementMap a;
				ActionElementMap b2;
				if ((int)val.elementType == 0 && (int)val.axisType == 2 && (int)val.axisContribution == 1)
				{
					if ((b = Find(_action1Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)0, out index, usedAction1Aems)) != null && (a = Find(_action2Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)1, out index2, usedAction2Aems)) != null && (b2 = Find(_action2Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)0, out index3, usedAction2Aems)) != null)
					{
						_results.Add(new Pair<ActionElementMapPair>(new ActionElementMapPair(val, b), new ActionElementMapPair(a, b2)));
						usedAction1Aems.Array[i] = true;
						usedAction1Aems.Array[index] = true;
						usedAction2Aems.Array[index2] = true;
						usedAction2Aems.Array[index3] = true;
						if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
						{
							return Result.Quit;
						}
					}
				}
				else if ((int)val.elementType == 1 && (int)val.axisContribution == 1 && (b = Find(_action1Aems, 0, (ControllerElementType)1, (AxisType)0, (Pole)0, out index, usedAction1Aems)) != null && (a = Find(_action2Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)1, out index2, usedAction2Aems)) != null && (b2 = Find(_action2Aems, 0, (ControllerElementType)0, (AxisType)2, (Pole)0, out index3, usedAction2Aems)) != null)
				{
					_results.Add(new Pair<ActionElementMapPair>(new ActionElementMapPair(val, b), new ActionElementMapPair(a, b2)));
					usedAction1Aems.Array[i] = true;
					usedAction1Aems.Array[index] = true;
					usedAction2Aems.Array[index2] = true;
					usedAction2Aems.Array[index3] = true;
					if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
					{
						return Result.Quit;
					}
				}
			}
			return Result.GoNext;
		}

		private static Result GetRemaining()
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Invalid comparison between Unknown and I4
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Invalid comparison between Unknown and I4
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Invalid comparison between Unknown and I4
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Invalid comparison between Unknown and I4
			if (_resultsRemainingCount == 0)
			{
				return Result.Quit;
			}
			Pair<ActionElementMapPair> target = default(Pair<ActionElementMapPair>);
			int num = 0;
			int num2 = 0;
			do
			{
				for (int i = 0; i < 2; i++)
				{
					FastList<ActionElementMap> fastList;
					int num3;
					int index;
					FastList<bool> fastList2;
					if (i == 0)
					{
						fastList = _action1Aems;
						num3 = num;
						index = 0;
						fastList2 = _usedAction1Aems;
					}
					else
					{
						fastList = _action2Aems;
						num3 = num2;
						index = 1;
						fastList2 = _usedAction2Aems;
					}
					if (num3 >= fastList.Count)
					{
						continue;
					}
					if (!fastList2.Array[num3])
					{
						ActionElementMap val = fastList.Array[num3];
						int index2;
						if ((int)val.elementType == 0)
						{
							if ((int)val.axisType == 1)
							{
								bool num4 = SetAndAddIfFull(new ActionElementMapPair(val, null), index, ref target, _results);
								fastList2.Array[num3] = true;
								if (num4 && !AllowMoreResultsDecrement(ref _resultsRemainingCount))
								{
									return Result.Quit;
								}
							}
							else if ((int)val.axisType == 2)
							{
								bool flag = (int)val.axisContribution == 0;
								ActionElementMap val2 = Find(fastList, 0, (ControllerElementType)0, (AxisType)2, (Pole)(flag ? 1 : 0), out index2, fastList2);
								if (val2 == null)
								{
									val2 = Find(fastList, 0, (ControllerElementType)1, (AxisType)0, (Pole)(flag ? 1 : 0), out index2, fastList2);
								}
								bool num5 = (flag ? SetAndAddIfFull(new ActionElementMapPair(val2, val), index, ref target, _results) : SetAndAddIfFull(new ActionElementMapPair(val, val2), index, ref target, _results));
								fastList2.Array[num3] = true;
								if (val2 != null)
								{
									fastList2.Array[index2] = true;
								}
								if (num5 && !AllowMoreResultsDecrement(ref _resultsRemainingCount))
								{
									return Result.Quit;
								}
							}
						}
						else if ((int)val.elementType == 1)
						{
							bool flag = (int)val.axisContribution == 0;
							ActionElementMap val2 = Find(fastList, 0, (ControllerElementType)1, (AxisType)0, (Pole)(flag ? 1 : 0), out index2, fastList2);
							if (val2 == null)
							{
								val2 = Find(fastList, 0, (ControllerElementType)0, (AxisType)2, (Pole)(flag ? 1 : 0), out index2, fastList2);
							}
							bool num6 = (flag ? SetAndAddIfFull(new ActionElementMapPair(val2, val), index, ref target, _results) : SetAndAddIfFull(new ActionElementMapPair(val, val2), index, ref target, _results));
							fastList2.Array[num3] = true;
							if (val2 != null)
							{
								fastList2.Array[index2] = true;
							}
							if (num6 && !AllowMoreResultsDecrement(ref _resultsRemainingCount))
							{
								return Result.Quit;
							}
						}
					}
					if (fastList == _action1Aems)
					{
						num = num3 + 1;
					}
					else
					{
						num2 = num3 + 1;
					}
				}
			}
			while (num < _action1Aems.Count || num2 < _action2Aems.Count);
			if (target.a.Count > 0 || target.b.Count > 0)
			{
				_results.Add(target);
				if (!AllowMoreResultsDecrement(ref _resultsRemainingCount))
				{
					return Result.Quit;
				}
			}
			return Result.GoNext;
		}

		private static Func<Result>[] GetSteps()
		{
			if ((int)_controllerElementTypeOrder[0] == 1)
			{
				return steps_buttonPriority;
			}
			return steps_axisPriority;
		}
	}

	private sealed class DefaultControllerMapCache
	{
		private struct Selector(int playerId, ControllerIdentifier controllerIdentifier, int mapCategoryId, int layoutId)
		{
			public readonly int playerId = playerId;

			public readonly ControllerIdentifier controllerIdentifier = controllerIdentifier;

			public readonly int mapCategoryId = mapCategoryId;

			public readonly int layoutId = layoutId;
		}

		private class Entry
		{
			public readonly Selector selector;

			public bool loaded;

			public ControllerMap controllerMap;

			public int lastTouchedFrame;

			public Entry(Selector selector)
			{
				this.selector = selector;
			}

			public void Clear()
			{
				loaded = false;
				controllerMap = null;
			}
		}

		private static DefaultControllerMapCache s_instance;

		private readonly List<Entry> _cache;

		public static DefaultControllerMapCache instance
		{
			get
			{
				if (!ReInput.isReady)
				{
					return null;
				}
				if (s_instance == null)
				{
					return s_instance = new DefaultControllerMapCache();
				}
				return s_instance;
			}
		}

		private DefaultControllerMapCache()
		{
			_cache = new List<Entry>();
			ReInput.ShutDownEvent += OnRewiredShutDown;
		}

		private void OnRewiredShutDown()
		{
			ReInput.ShutDownEvent -= OnRewiredShutDown;
			s_instance = null;
		}

		public ControllerMap GetControllerMap(int playerId, ControllerIdentifier controllerIdentifier, string mapCategoryName, string layoutName)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			if (!ReInput.isReady)
			{
				return null;
			}
			int mapCategoryId = ReInput.mapping.GetMapCategoryId(mapCategoryName);
			if (mapCategoryId < 0)
			{
				return null;
			}
			int layoutId = ReInput.mapping.GetLayoutId(((ControllerIdentifier)(ref controllerIdentifier)).controllerType, layoutName);
			if (layoutId < 0)
			{
				return null;
			}
			int num = IndexOf(playerId, controllerIdentifier, mapCategoryId, layoutId);
			Entry entry;
			if (num < 0)
			{
				entry = new Entry(new Selector(playerId, controllerIdentifier, mapCategoryId, layoutId));
				_cache.Add(entry);
			}
			else
			{
				entry = _cache[num];
			}
			if (!IsEqualOrNextFrame(Time.frameCount, entry.lastTouchedFrame))
			{
				entry.Clear();
			}
			if (!entry.loaded)
			{
				entry.controllerMap = ReInput.mapping.GetControllerMapInstanceSavedOrDefault(playerId, controllerIdentifier, mapCategoryId, layoutId);
				entry.loaded = true;
			}
			entry.lastTouchedFrame = Time.frameCount;
			return entry.controllerMap;
		}

		private int IndexOf(int playerId, ControllerIdentifier controllerIdentifier, int mapCategoryId, int layoutId)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			int count = _cache.Count;
			for (int i = 0; i < count; i++)
			{
				Selector selector = _cache[i].selector;
				if (selector.playerId != playerId || selector.mapCategoryId != mapCategoryId || selector.layoutId != layoutId)
				{
					continue;
				}
				ControllerIdentifier controllerIdentifier2 = selector.controllerIdentifier;
				if (((ControllerIdentifier)(ref controllerIdentifier2)).controllerType != ((ControllerIdentifier)(ref controllerIdentifier)).controllerType)
				{
					continue;
				}
				controllerIdentifier2 = selector.controllerIdentifier;
				if (((ControllerIdentifier)(ref controllerIdentifier2)).deviceInstanceGuid == ((ControllerIdentifier)(ref controllerIdentifier)).deviceInstanceGuid)
				{
					controllerIdentifier2 = selector.controllerIdentifier;
					if (string.Equals(((ControllerIdentifier)(ref controllerIdentifier2)).hardwareIdentifier, ((ControllerIdentifier)(ref controllerIdentifier)).hardwareIdentifier, StringComparison.Ordinal))
					{
						return i;
					}
				}
			}
			return -1;
		}

		private static bool IsEqualOrNextFrame(int a, int b)
		{
			if (a == b)
			{
				return true;
			}
			if (b == int.MaxValue)
			{
				return a == 0;
			}
			return a == b + 1;
		}
	}

	private sealed class FastList<T>
	{
		private const int minCapacity = 2;

		public T[] Array;

		public int Count;

		public int Capacity;

		public FastList(int startingCapacity)
		{
			if (startingCapacity < 2)
			{
				startingCapacity = 2;
			}
			Array = new T[startingCapacity];
			Capacity = startingCapacity;
		}

		public void Add(T item)
		{
			if (Count >= Capacity)
			{
				Expand(Capacity * 2);
			}
			Array[Count] = item;
			Count++;
		}

		public void RemoveAt(int index)
		{
			if ((uint)index >= (uint)Count)
			{
				throw new IndexOutOfRangeException();
			}
			int num = Count - 1;
			for (int i = index; i < num; i++)
			{
				Array[i] = Array[i + 1];
			}
			Array[num] = default(T);
			Count--;
		}

		public void Expand(int size)
		{
			if (size <= 2)
			{
				size = 2;
			}
			if (size > Capacity)
			{
				if (!IsPowerOfTwo((uint)size))
				{
					size = (int)RoundUpToPowerOf2((uint)size);
				}
				T[] array = new T[size];
				int num = ((Capacity < size) ? Capacity : size);
				for (int i = 0; i < num; i++)
				{
					array[i] = Array[i];
				}
				Array = array;
				Capacity = Array.Length;
			}
		}

		public void SetCount(int size)
		{
			if (size < 0)
			{
				size = 0;
			}
			if (size != Count)
			{
				if (size < Count)
				{
					System.Array.Clear(Array, size, Count - size);
				}
				if (size > Capacity)
				{
					Expand(size);
				}
				Count = size;
			}
		}

		public void ReplaceFrom(IList<T> source)
		{
			Clear();
			int count = source.Count;
			Expand(count);
			for (int i = 0; i < count; i++)
			{
				Array[i] = source[i];
			}
			Count = count;
		}

		public void ReplaceFrom(FastList<T> source)
		{
			Clear();
			int count = source.Count;
			Expand(count);
			for (int i = 0; i < count; i++)
			{
				Array[i] = source.Array[i];
			}
			Count = count;
		}

		public void Clear()
		{
			if (Count > 0)
			{
				System.Array.Clear(Array, 0, Count);
			}
			Count = 0;
		}

		private static uint RoundUpToPowerOf2(uint value)
		{
			if (value == 0)
			{
				return 1u;
			}
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value;
		}

		private static bool IsPowerOfTwo(uint x)
		{
			if (x != 0)
			{
				return (x & (x - 1)) == 0;
			}
			return false;
		}
	}

	private sealed class ObjectPool<T> where T : class
	{
		private readonly List<T> _objects;

		private readonly Func<T> _createDelegate;

		private readonly Action<T> _onReturnDelegate;

		public ObjectPool(Func<T> createDelegate, Action<T> onReturnDelegate)
		{
			_createDelegate = createDelegate;
			_onReturnDelegate = onReturnDelegate;
			_objects = new List<T>();
		}

		public T Get()
		{
			if (_objects.Count != 0)
			{
				int index = _objects.Count - 1;
				T result = _objects[index];
				_objects.RemoveAt(index);
				return result;
			}
			return _createDelegate();
		}

		public void Return(T obj)
		{
			if (obj != null && !_objects.Contains(obj))
			{
				_objects.Add(obj);
				_onReturnDelegate(obj);
			}
		}
	}

	private struct ControllerInfo(ControllerType type, int controllerId)
	{
		public ControllerType type = type;

		public int controllerId = controllerId;
	}

	private static readonly ObjectPool<FastList<ActionElementMap>> aemFastListPool = new ObjectPool<FastList<ActionElementMap>>(() => new FastList<ActionElementMap>(16), delegate(FastList<ActionElementMap> x)
	{
		x.Clear();
	});

	private static readonly ObjectPool<List<ActionElementMapPair>> aemPairListPool = new ObjectPool<List<ActionElementMapPair>>(() => new List<ActionElementMapPair>(16), delegate(List<ActionElementMapPair> x)
	{
		x.Clear();
	});

	private static readonly ObjectPool<List<Pair<ActionElementMapPair>>> aemPair2dListPool = new ObjectPool<List<Pair<ActionElementMapPair>>>(() => new List<Pair<ActionElementMapPair>>(8), delegate(List<Pair<ActionElementMapPair>> x)
	{
		x.Clear();
	});

	private static readonly ObjectPool<FastList<bool>> boolFastListPool = new ObjectPool<FastList<bool>>(() => new FastList<bool>(16), delegate(FastList<bool> x)
	{
		x.Clear();
	});

	private static readonly ObjectPool<FastList<ControllerInfo>> controllerInfoFastListPool = new ObjectPool<FastList<ControllerInfo>>(() => new FastList<ControllerInfo>(8), delegate(FastList<ControllerInfo> x)
	{
		x.Clear();
	});

	private static readonly List<ActionElementMap> GetElementMapsWithAction_tempAems = new List<ActionElementMap>();

	private static Predicate<ActionElementMap> __defaultGetElementMapsWithActionisAllowedHandler;

	private static Predicate<ActionElementMap> defaultGetElementMapsWithActionisAllowedHandler
	{
		get
		{
			if (__defaultGetElementMapsWithActionisAllowedHandler == null)
			{
				__defaultGetElementMapsWithActionisAllowedHandler = (ActionElementMap aem) => (aem != null && aem.controllerMap.enabled && aem.enabled) ? true : false;
			}
			return __defaultGetElementMapsWithActionisAllowedHandler;
		}
	}

	public static bool TryGetActionElementMaps(int playerId, int actionId, AxisRange actionRange, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, out ActionElementMap aemResult1, out ActionElementMap aemResult2)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		List<ActionElementMapPair> list = aemPairListPool.Get();
		if (GetActionElementMaps(playerId, actionId, actionRange, options, isAemAllowedHandlerOverride, list, 1) > 0)
		{
			aemResult1 = list[0].a;
			aemResult2 = list[0].b;
		}
		else
		{
			aemResult1 = null;
			aemResult2 = null;
		}
		aemPairListPool.Return(list);
		if (aemResult1 == null)
		{
			return aemResult2 != null;
		}
		return true;
	}

	public static bool TryGetActionElementMaps(InputAction action, AxisRange actionRange, List<ActionElementMap> aems, out ActionElementMap aemResult1, out ActionElementMap aemResult2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryGetActionElementMaps(action, actionRange, aems, null, out aemResult1, out aemResult2);
	}

	public static bool TryGetActionElementMaps(InputAction action, AxisRange actionRange, List<ActionElementMap> aems, ControllerElementGlyphSelectorOptions options, out ActionElementMap aemResult1, out ActionElementMap aemResult2)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		List<ActionElementMapPair> list = aemPairListPool.Get();
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		int resultsRemainingCount = 1;
		fastList.ReplaceFrom(aems);
		if (GetActionElementMaps(action, actionRange, fastList, isSorted: false, options, list, ref resultsRemainingCount) > 0)
		{
			aemResult1 = list[0].a;
			aemResult2 = list[0].b;
		}
		else
		{
			aemResult1 = null;
			aemResult2 = null;
		}
		aemPairListPool.Return(list);
		aemFastListPool.Return(fastList);
		if (aemResult1 == null)
		{
			return aemResult2 != null;
		}
		return true;
	}

	public static bool TryGetActionElementMaps(int playerId, int actionId, int actionId2, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, out ActionElementMapPair aemResult1, out ActionElementMapPair aemResult2)
	{
		List<Pair<ActionElementMapPair>> list = aemPair2dListPool.Get();
		if (GetActionElementMaps(playerId, actionId, actionId2, options, isAemAllowedHandlerOverride, list, 1) > 0)
		{
			aemResult1 = list[0].a;
			aemResult2 = list[0].b;
		}
		else
		{
			aemResult1 = default(ActionElementMapPair);
			aemResult2 = default(ActionElementMapPair);
		}
		aemPair2dListPool.Return(list);
		if (aemResult1.Count <= 0)
		{
			return aemResult2.Count > 0;
		}
		return true;
	}

	public static int GetActionElementMaps(int playerId, int actionId, AxisRange actionRange, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, List<ActionElementMapPair> results)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetActionElementMaps(playerId, actionId, actionRange, options, isAemAllowedHandlerOverride, results, 0);
	}

	public static int GetActionElementMaps(int playerId, int actionId, int actionId2, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, List<Pair<ActionElementMapPair>> results)
	{
		return GetActionElementMaps(playerId, actionId, actionId2, options, isAemAllowedHandlerOverride, results, 0);
	}

	public static ActionElementMap FindFirstFullAxisBinding(List<ActionElementMap> actionElementMaps)
	{
		return FindFirstFullAxisBinding(actionElementMaps, null);
	}

	public static ActionElementMap FindFirstFullAxisBinding(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options)
	{
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		List<ActionElementMapPair> list = aemPairListPool.Get();
		int resultsRemainingCount = 1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		ActionElementMap result = ((FindFullAxisBindingsOnly(fastList, usedPooledList, list, ref resultsRemainingCount) <= 0) ? null : list[0].a);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		aemPairListPool.Return(list);
		return result;
	}

	public static int FindFullAxisBindings(List<ActionElementMap> actionElementMaps, List<ActionElementMapPair> results)
	{
		return FindFullAxisBindings(actionElementMaps, null, results);
	}

	public static int FindFullAxisBindings(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, List<ActionElementMapPair> results)
	{
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = -1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		int result = FindFullAxisBindingsOnly(fastList, usedPooledList, results, ref resultsRemainingCount);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		return result;
	}

	public static ActionElementMap FindFirstBinding(List<ActionElementMap> actionElementMaps, AxisRange actionRange)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return FindFirstBinding(actionElementMaps, actionRange);
	}

	public static ActionElementMap FindFirstBinding(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, AxisRange actionRange)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		List<ActionElementMapPair> list = aemPairListPool.Get();
		int resultsRemainingCount = 1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		ActionElementMap result = ((FindBindings(fastList, usedPooledList, actionRange, list, ref resultsRemainingCount) <= 0) ? null : ((list[0].a != null) ? list[0].a : list[0].b));
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		aemPairListPool.Return(list);
		return result;
	}

	public static int FindBindings(List<ActionElementMap> actionElementMaps, AxisRange actionRange, List<ActionElementMapPair> results)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return FindBindings(actionElementMaps, actionRange, null, results);
	}

	public static int FindBindings(List<ActionElementMap> actionElementMaps, AxisRange actionRange, ControllerElementGlyphSelectorOptions options, List<ActionElementMapPair> results)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = -1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		int result = FindBindings(fastList, usedPooledList, actionRange, results, ref resultsRemainingCount);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		return result;
	}

	public static int FindSplitAxisBindingPairs(List<ActionElementMap> actionElementMaps, List<ActionElementMapPair> results)
	{
		return FindSplitAxisBindingPairs(actionElementMaps, null, results);
	}

	public static int FindSplitAxisBindingPairs(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, List<ActionElementMapPair> results)
	{
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = -1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		int result = FindSplitAxisBindingPairsOnly(fastList, usedPooledList, results, ref resultsRemainingCount);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		return result;
	}

	public static bool FindFirstSplitAxisBindingPair(List<ActionElementMap> actionElementMaps, out ActionElementMap negativeAem, out ActionElementMap positiveAem)
	{
		return FindFirstSplitAxisBindingPair(actionElementMaps, null, out negativeAem, out positiveAem);
	}

	public static bool FindFirstSplitAxisBindingPair(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, out ActionElementMap negativeAem, out ActionElementMap positiveAem)
	{
		List<ActionElementMapPair> list = aemPairListPool.Get();
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = 1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		if (FindSplitAxisBindingPairsOnly(fastList, usedPooledList, list, ref resultsRemainingCount) > 0)
		{
			negativeAem = Get(list[0], (Pole)1);
			positiveAem = Get(list[0], (Pole)0);
		}
		else
		{
			negativeAem = null;
			positiveAem = null;
		}
		aemPairListPool.Return(list);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		if (negativeAem == null)
		{
			return positiveAem != null;
		}
		return true;
	}

	public static int FindButtonBindingPairs(List<ActionElementMap> actionElementMaps, List<ActionElementMapPair> results)
	{
		return FindSplitAxisBindingPairs(actionElementMaps, null, results);
	}

	public static int FindButtonBindingPairs(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, List<ActionElementMapPair> results)
	{
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = -1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		int result = FindButtonBindingPairsOnly(fastList, usedPooledList, results, ref resultsRemainingCount);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		return result;
	}

	public static bool FindFirstButtonBindingPair(List<ActionElementMap> actionElementMaps, out ActionElementMap negativeAem, out ActionElementMap positiveAem)
	{
		return FindFirstSplitAxisBindingPair(actionElementMaps, null, out negativeAem, out positiveAem);
	}

	public static bool FindFirstButtonBindingPair(List<ActionElementMap> actionElementMaps, ControllerElementGlyphSelectorOptions options, out ActionElementMap negativeAem, out ActionElementMap positiveAem)
	{
		List<ActionElementMapPair> list = aemPairListPool.Get();
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(actionElementMaps.Count);
		int resultsRemainingCount = 1;
		fastList.ReplaceFrom(actionElementMaps);
		if (options != null)
		{
			SortByElementType(fastList, options.controllerElementTypeOrder);
		}
		if (FindButtonBindingPairsOnly(fastList, usedPooledList, list, ref resultsRemainingCount) > 0)
		{
			negativeAem = Get(list[0], (Pole)1);
			positiveAem = Get(list[0], (Pole)0);
		}
		else
		{
			negativeAem = null;
			positiveAem = null;
		}
		aemPairListPool.Return(list);
		aemFastListPool.Return(fastList);
		ReturnUsedPoolList(usedPooledList);
		if (negativeAem == null)
		{
			return positiveAem != null;
		}
		return true;
	}

	public static bool IsMousePrioritizedOverKeyboard(ControllerElementGlyphSelectorOptions options)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (options == null)
		{
			return false;
		}
		ControllerType controllerType;
		for (int i = 0; options.TryGetControllerTypeOrder(i, out controllerType); i++)
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

	private static int GetActionElementMaps(int playerId, int actionId, AxisRange actionRange, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, List<ActionElementMapPair> results, int maxResultCount)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Invalid comparison between Unknown and I4
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Invalid comparison between Unknown and I4
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Invalid comparison between Unknown and I4
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Invalid comparison between Unknown and I4
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Invalid comparison between Unknown and I4
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Invalid comparison between Unknown and I4
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Invalid comparison between Unknown and I4
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		if (!ReInput.isReady)
		{
			return 0;
		}
		if (options == null)
		{
			return 0;
		}
		if (results == null)
		{
			return 0;
		}
		if (maxResultCount < 0)
		{
			maxResultCount = 0;
		}
		InputAction action = ReInput.mapping.GetAction(actionId);
		if (action == null)
		{
			return 0;
		}
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return 0;
		}
		ControllerElementType[] controllerElementTypeOrder = options.controllerElementTypeOrder;
		int count = results.Count;
		int resultsRemainingCount = ((maxResultCount > 0) ? maxResultCount : (-1));
		bool useFirstControllerResults = options.useFirstControllerResults;
		Controller val = player.controllers.GetLastActiveController();
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<ControllerInfo> fastList2 = controllerInfoFastListPool.Get();
		try
		{
			Predicate<ActionElementMap> predicate = null;
			if (isAemAllowedHandlerOverride != null)
			{
				predicate = isAemAllowedHandlerOverride;
			}
			else if (options != null)
			{
				predicate = options.isActionElementMapAllowedHandler;
			}
			if (predicate == null)
			{
				predicate = defaultGetElementMapsWithActionisAllowedHandler;
			}
			if (options.useLastActiveController && val != null)
			{
				Controller val2 = null;
				if ((int)val.type == 0 || (int)val.type == 1)
				{
					if (IsMousePrioritizedOverKeyboard(options))
					{
						if (((Controller)ReInput.controllers.Mouse).enabled && player.controllers.hasMouse)
						{
							val = (Controller)(object)ReInput.controllers.Mouse;
							val2 = (Controller)(object)ReInput.controllers.Keyboard;
						}
					}
					else if (((Controller)ReInput.controllers.Keyboard).enabled && player.controllers.hasKeyboard)
					{
						val = (Controller)(object)ReInput.controllers.Keyboard;
						val2 = (Controller)(object)ReInput.controllers.Mouse;
					}
				}
				if (!Contains(fastList2, val.type, val.id))
				{
					if (GetElementMapsWithAction(player, val.type, val.id, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
					fastList2.Add(new ControllerInfo(val.type, val.id));
				}
				if (val2 != null && !Contains(fastList2, val2.type, val2.id))
				{
					if (GetElementMapsWithAction(player, val2.type, val2.id, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
					fastList2.Add(new ControllerInfo(val2.type, val2.id));
				}
				if (useFirstControllerResults && results.Count - count > 0)
				{
					return results.Count - count;
				}
				ControllerType type = val.type;
				if ((int)type != 2)
				{
					if ((int)type == 20)
					{
						for (int i = 0; i < player.controllers.customControllerCount; i++)
						{
							int id = ((Controller)player.controllers.CustomControllers[i]).id;
							if (!Contains(fastList2, type, id))
							{
								if (GetElementMapsWithAction(player, type, id, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
								{
									return results.Count - count;
								}
								fastList2.Add(new ControllerInfo(type, id));
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < player.controllers.joystickCount; j++)
					{
						int id2 = ((Controller)player.controllers.Joysticks[j]).id;
						if (!Contains(fastList2, type, id2))
						{
							if (GetElementMapsWithAction(player, type, id2, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
							{
								return results.Count - count;
							}
							fastList2.Add(new ControllerInfo(type, id2));
						}
					}
				}
			}
			int num = 15;
			ControllerType[] controllerTypeOrder = options.controllerTypeOrder;
			int num2 = 0;
			while (num != 0)
			{
				ControllerType val3;
				if (num2 < controllerTypeOrder.Length)
				{
					val3 = controllerTypeOrder[num2];
				}
				else if ((num & 1) != 0)
				{
					val3 = (ControllerType)2;
				}
				else if ((num & 4) != 0)
				{
					val3 = (ControllerType)1;
				}
				else if ((num & 2) != 0)
				{
					val3 = (ControllerType)0;
				}
				else
				{
					if ((num & 8) == 0)
					{
						throw new NotImplementedException();
					}
					val3 = (ControllerType)20;
				}
				if ((int)val3 > 1)
				{
					if ((int)val3 != 2)
					{
						if ((int)val3 == 20 && (num & 8) != 0)
						{
							for (int k = 0; k < player.controllers.customControllerCount; k++)
							{
								int id3 = ((Controller)player.controllers.CustomControllers[k]).id;
								if (!Contains(fastList2, val3, id3))
								{
									if (GetElementMapsWithAction(player, val3, id3, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
									{
										return results.Count - count;
									}
									fastList2.Add(new ControllerInfo(val3, id3));
								}
							}
							num &= -9;
						}
					}
					else if ((num & 1) != 0)
					{
						for (int l = 0; l < player.controllers.joystickCount; l++)
						{
							int id3 = ((Controller)player.controllers.Joysticks[l]).id;
							if (!Contains(fastList2, val3, id3))
							{
								if (GetElementMapsWithAction(player, val3, id3, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
								{
									return results.Count - count;
								}
								fastList2.Add(new ControllerInfo(val3, id3));
							}
						}
						num &= -2;
					}
				}
				else
				{
					bool flag = false;
					bool flag2 = useFirstControllerResults;
					if (((int)val3 == 1 || flag2) && (num & 4) != 0)
					{
						if (player.controllers.hasMouse)
						{
							int id3 = ((Controller)ReInput.controllers.Mouse).id;
							if (!Contains(fastList2, (ControllerType)1, id3))
							{
								if (GetElementMapsWithAction(player, (ControllerType)1, id3, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0)
								{
									if (maxResultCount > 0 && resultsRemainingCount <= 0)
									{
										return results.Count - count;
									}
									flag = true;
								}
								fastList2.Add(new ControllerInfo((ControllerType)1, id3));
							}
						}
						num &= -5;
					}
					if (((int)val3 == 0 || flag2) && (num & 2) != 0)
					{
						if (player.controllers.hasKeyboard)
						{
							int id3 = ((Controller)ReInput.controllers.Keyboard).id;
							if (!Contains(fastList2, (ControllerType)0, id3))
							{
								if (GetElementMapsWithAction(player, (ControllerType)0, id3, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0)
								{
									if (maxResultCount > 0 && resultsRemainingCount <= 0)
									{
										return results.Count - count;
									}
									flag = true;
								}
								fastList2.Add(new ControllerInfo((ControllerType)0, id3));
							}
						}
						num &= -3;
					}
					if (useFirstControllerResults && flag)
					{
						return results.Count - count;
					}
				}
				num2++;
			}
			if (options.useDefaultControllers)
			{
				List<ControllerElementGlyphSelectorOptions.ControllerSelector> defaultControllers = options.defaultControllers;
				int num3 = defaultControllers?.Count ?? 0;
				for (int m = 0; m < num3; m++)
				{
					ControllerElementGlyphSelectorOptions.ControllerSelector controllerSelector = defaultControllers[m];
					List<ControllerElementGlyphSelectorOptions.ControllerMapSelector> controllerMapSelectors = controllerSelector.controllerMapSelectors;
					if (controllerMapSelectors != null)
					{
						int count2 = controllerMapSelectors.Count;
						ControllerIdentifier blank = ControllerIdentifier.Blank;
						((ControllerIdentifier)(ref blank)).controllerType = controllerSelector.controllerType;
						((ControllerIdentifier)(ref blank)).hardwareTypeGuid = controllerSelector.hardwareTypeGuid;
						((ControllerIdentifier)(ref blank)).hardwareIdentifier = controllerSelector.hardwareIdentifier;
						for (int n = 0; n < count2; n++)
						{
							ControllerMap controllerMap = DefaultControllerMapCache.instance.GetControllerMap(player.id, blank, controllerMapSelectors[n].mapCategoryName, controllerMapSelectors[n].layoutName);
							if (controllerMap != null)
							{
								controllerMap.enabled = true;
								if (GetElementMapsWithAction(controllerMap, actionId, predicate, controllerElementTypeOrder, fastList) > 0 && GetActionElementMaps(action, actionRange, fastList, isSorted: true, options, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
								{
									return results.Count - count;
								}
							}
						}
					}
					if (useFirstControllerResults && results.Count - count > 0)
					{
						return results.Count - count;
					}
				}
			}
			return results.Count - count;
		}
		finally
		{
			aemFastListPool.Return(fastList);
			controllerInfoFastListPool.Return(fastList2);
		}
	}

	private static int GetActionElementMaps(InputAction action, AxisRange actionRange, FastList<ActionElementMap> aems, bool isSorted, ControllerElementGlyphSelectorOptions options, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		if (aems == null || results == null)
		{
			throw new ArgumentNullException();
		}
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		FastList<bool> usedPooledList = GetUsedPooledList(aems.Count);
		int count = results.Count;
		bool flag = (int)action.type == 0;
		if (!isSorted && options != null)
		{
			SortByElementType(aems, options.controllerElementTypeOrder);
		}
		ControllerElementType val = (ControllerElementType)0;
		if (options != null)
		{
			ControllerElementType[] controllerElementTypeOrder = options.controllerElementTypeOrder;
			for (int i = 0; i < controllerElementTypeOrder.Length; i++)
			{
				if ((int)controllerElementTypeOrder[i] == 0)
				{
					val = (ControllerElementType)0;
					break;
				}
				if ((int)controllerElementTypeOrder[i] == 1)
				{
					val = (ControllerElementType)1;
					break;
				}
			}
		}
		if (flag)
		{
			if ((int)actionRange == 0)
			{
				if ((int)val == 1)
				{
					FindButtonBindingPairsOnly(aems, usedPooledList, results, ref resultsRemainingCount);
					if (resultsRemainingCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
				}
				FindFullAxisBindingsOnly(aems, usedPooledList, results, ref resultsRemainingCount);
				if (resultsRemainingCount > 0 && resultsRemainingCount <= 0)
				{
					return results.Count - count;
				}
				FindSplitAxisBindingPairsOnly(aems, usedPooledList, results, ref resultsRemainingCount);
				if (resultsRemainingCount > 0 && resultsRemainingCount <= 0)
				{
					return results.Count - count;
				}
				if ((int)val != 1)
				{
					FindButtonBindingPairsOnly(aems, usedPooledList, results, ref resultsRemainingCount);
					if (resultsRemainingCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
				}
				FindSplitAxisAndButtonBindingPairsAndRemaining(aems, usedPooledList, results, ref resultsRemainingCount);
				if (resultsRemainingCount > 0 && resultsRemainingCount <= 0)
				{
					return results.Count - count;
				}
			}
			else
			{
				FindBindings(aems, usedPooledList, actionRange, results, ref resultsRemainingCount);
			}
		}
		else
		{
			FindBindings(aems, usedPooledList, actionRange, results, ref resultsRemainingCount);
		}
		boolFastListPool.Return(usedPooledList);
		return results.Count - count;
	}

	private static int GetActionElementMaps(int playerId, int actionId, int actionId2, ControllerElementGlyphSelectorOptions options, Predicate<ActionElementMap> isAemAllowedHandlerOverride, List<Pair<ActionElementMapPair>> results, int maxResultCount)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Invalid comparison between Unknown and I4
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Invalid comparison between Unknown and I4
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Invalid comparison between Unknown and I4
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Invalid comparison between Unknown and I4
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Invalid comparison between Unknown and I4
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Invalid comparison between Unknown and I4
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Invalid comparison between Unknown and I4
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		if (!ReInput.isReady)
		{
			return 0;
		}
		if (options == null)
		{
			return 0;
		}
		if (results == null)
		{
			return 0;
		}
		if (maxResultCount < 0)
		{
			maxResultCount = 0;
		}
		InputAction action = ReInput.mapping.GetAction(actionId);
		if (action == null)
		{
			return 0;
		}
		InputAction action2 = ReInput.mapping.GetAction(actionId2);
		if (action2 == null)
		{
			return 0;
		}
		if (action2 == action)
		{
			return 0;
		}
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return 0;
		}
		ControllerElementType[] controllerElementTypeOrder = options.controllerElementTypeOrder;
		int count = results.Count;
		int resultsRemainingCount = ((maxResultCount > 0) ? maxResultCount : (-1));
		bool useFirstControllerResults = options.useFirstControllerResults;
		Controller val = player.controllers.GetLastActiveController();
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<ActionElementMap> fastList2 = aemFastListPool.Get();
		FastList<ControllerInfo> fastList3 = controllerInfoFastListPool.Get();
		try
		{
			Predicate<ActionElementMap> predicate = null;
			if (isAemAllowedHandlerOverride != null)
			{
				predicate = isAemAllowedHandlerOverride;
			}
			else if (options != null)
			{
				predicate = options.isActionElementMapAllowedHandler;
			}
			if (predicate == null)
			{
				predicate = defaultGetElementMapsWithActionisAllowedHandler;
			}
			if (options.useLastActiveController && val != null)
			{
				Controller val2 = null;
				if ((int)val.type == 0 || (int)val.type == 1)
				{
					if (IsMousePrioritizedOverKeyboard(options))
					{
						if (((Controller)ReInput.controllers.Mouse).enabled && player.controllers.hasMouse)
						{
							val = (Controller)(object)ReInput.controllers.Mouse;
							val2 = (Controller)(object)ReInput.controllers.Keyboard;
						}
					}
					else if (((Controller)ReInput.controllers.Keyboard).enabled && player.controllers.hasKeyboard)
					{
						val = (Controller)(object)ReInput.controllers.Keyboard;
						val2 = (Controller)(object)ReInput.controllers.Mouse;
					}
				}
				if (!Contains(fastList3, val.type, val.id) && GetElementMapsWithAction(player, val.type, val.id, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0)
				{
					if (Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
					fastList3.Add(new ControllerInfo(val.type, val.id));
				}
				if (val2 != null && !Contains(fastList3, val2.type, val2.id))
				{
					if (GetElementMapsWithAction(player, val2.type, val2.id, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
					{
						return results.Count - count;
					}
					fastList3.Add(new ControllerInfo(val2.type, val2.id));
				}
				if (useFirstControllerResults && results.Count - count > 0)
				{
					return results.Count - count;
				}
				ControllerType type = val.type;
				if ((int)type != 2)
				{
					if ((int)type == 20)
					{
						for (int i = 0; i < player.controllers.customControllerCount; i++)
						{
							int id = ((Controller)player.controllers.CustomControllers[i]).id;
							if (!Contains(fastList3, type, id))
							{
								if (GetElementMapsWithAction(player, type, id, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
								{
									return results.Count - count;
								}
								fastList3.Add(new ControllerInfo(type, id));
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < player.controllers.joystickCount; j++)
					{
						int id2 = ((Controller)player.controllers.Joysticks[j]).id;
						if (!Contains(fastList3, type, id2))
						{
							if (GetElementMapsWithAction(player, type, id2, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
							{
								return results.Count - count;
							}
							fastList3.Add(new ControllerInfo(type, id2));
						}
					}
				}
			}
			int num = 15;
			ControllerType[] controllerTypeOrder = options.controllerTypeOrder;
			int num2 = 0;
			while (num != 0)
			{
				ControllerType val3;
				if (num2 < controllerTypeOrder.Length)
				{
					val3 = controllerTypeOrder[num2];
				}
				else if ((num & 1) != 0)
				{
					val3 = (ControllerType)2;
				}
				else if ((num & 4) != 0)
				{
					val3 = (ControllerType)1;
				}
				else if ((num & 2) != 0)
				{
					val3 = (ControllerType)0;
				}
				else
				{
					if ((num & 8) == 0)
					{
						throw new NotImplementedException();
					}
					val3 = (ControllerType)20;
				}
				if ((int)val3 > 1)
				{
					if ((int)val3 != 2)
					{
						if ((int)val3 == 20 && (num & 8) != 0)
						{
							for (int k = 0; k < player.controllers.customControllerCount; k++)
							{
								int id3 = ((Controller)player.controllers.CustomControllers[k]).id;
								if (!Contains(fastList3, val3, id3))
								{
									if (GetElementMapsWithAction(player, val3, id3, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
									{
										return results.Count - count;
									}
									fastList3.Add(new ControllerInfo(val3, id3));
								}
							}
							num &= -9;
						}
					}
					else if ((num & 1) != 0)
					{
						for (int l = 0; l < player.controllers.joystickCount; l++)
						{
							int id3 = ((Controller)player.controllers.Joysticks[l]).id;
							if (!Contains(fastList3, val3, id3))
							{
								if (GetElementMapsWithAction(player, val3, id3, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0)))
								{
									return results.Count - count;
								}
								fastList3.Add(new ControllerInfo(val3, id3));
							}
						}
						num &= -2;
					}
				}
				else
				{
					bool flag = false;
					bool flag2 = useFirstControllerResults;
					if (((int)val3 == 1 || flag2) && (num & 4) != 0)
					{
						if (player.controllers.hasMouse)
						{
							int id3 = ((Controller)ReInput.controllers.Mouse).id;
							if (!Contains(fastList3, (ControllerType)1, id3))
							{
								if (GetElementMapsWithAction(player, (ControllerType)1, id3, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0)
								{
									if (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0))
									{
										return results.Count - count;
									}
									flag = true;
								}
								fastList3.Add(new ControllerInfo((ControllerType)1, id3));
							}
						}
						num &= -5;
					}
					if (((int)val3 == 0 || flag2) && (num & 2) != 0)
					{
						if (player.controllers.hasKeyboard)
						{
							int id3 = ((Controller)ReInput.controllers.Keyboard).id;
							if (!Contains(fastList3, (ControllerType)0, id3))
							{
								if (GetElementMapsWithAction(player, (ControllerType)0, id3, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0)
								{
									if (useFirstControllerResults || (maxResultCount > 0 && resultsRemainingCount <= 0))
									{
										return results.Count - count;
									}
									flag = true;
								}
								fastList3.Add(new ControllerInfo((ControllerType)0, id3));
							}
						}
						num &= -3;
					}
					if (useFirstControllerResults && flag)
					{
						return results.Count - count;
					}
				}
				num2++;
			}
			if (options.useDefaultControllers)
			{
				List<ControllerElementGlyphSelectorOptions.ControllerSelector> defaultControllers = options.defaultControllers;
				int num3 = defaultControllers?.Count ?? 0;
				for (int m = 0; m < num3; m++)
				{
					ControllerElementGlyphSelectorOptions.ControllerSelector controllerSelector = defaultControllers[m];
					List<ControllerElementGlyphSelectorOptions.ControllerMapSelector> controllerMapSelectors = controllerSelector.controllerMapSelectors;
					if (controllerMapSelectors != null)
					{
						int count2 = controllerMapSelectors.Count;
						ControllerIdentifier blank = ControllerIdentifier.Blank;
						((ControllerIdentifier)(ref blank)).controllerType = controllerSelector.controllerType;
						((ControllerIdentifier)(ref blank)).hardwareTypeGuid = controllerSelector.hardwareTypeGuid;
						((ControllerIdentifier)(ref blank)).hardwareIdentifier = controllerSelector.hardwareIdentifier;
						for (int n = 0; n < count2; n++)
						{
							ControllerMap controllerMap = DefaultControllerMapCache.instance.GetControllerMap(player.id, blank, controllerMapSelectors[n].mapCategoryName, controllerMapSelectors[n].layoutName);
							if (controllerMap != null)
							{
								controllerMap.enabled = true;
								if (GetElementMapsWithAction(controllerMap, actionId, actionId2, predicate, controllerElementTypeOrder, fastList, fastList2) > 0 && Action2DHelper.GetActionElementMaps(fastList, fastList2, controllerElementTypeOrder, results, ref resultsRemainingCount) > 0 && maxResultCount > 0 && resultsRemainingCount <= 0)
								{
									return results.Count - count;
								}
							}
						}
					}
					if (useFirstControllerResults && results.Count - count > 0)
					{
						return results.Count - count;
					}
				}
			}
			return results.Count - count;
		}
		finally
		{
			aemFastListPool.Return(fastList);
			aemFastListPool.Return(fastList2);
			controllerInfoFastListPool.Return(fastList3);
		}
	}

	private static int FindFullAxisBindingsOnly(FastList<ActionElementMap> actionElementMaps, FastList<bool> usedAems, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		int count = results.Count;
		int count2 = actionElementMaps.Count;
		for (int i = 0; i < count2; i++)
		{
			if (usedAems.Array[i])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[i];
			if ((int)val.elementType == 0 && (int)val.axisType == 1)
			{
				results.Add(new ActionElementMapPair(val, null));
				usedAems.Array[i] = true;
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		return results.Count - count;
	}

	private static int FindBindings(FastList<ActionElementMap> actionElementMaps, FastList<bool> usedAems, AxisRange actionRange, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Invalid comparison between Unknown and I4
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Invalid comparison between Unknown and I4
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (actionElementMaps.Count == 0)
		{
			return 0;
		}
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		int count = results.Count;
		int count2 = actionElementMaps.Count;
		ActionElementMap val3 = default(ActionElementMap);
		ActionElementMap val4 = default(ActionElementMap);
		for (int i = 0; i < count2; i++)
		{
			if (usedAems.Array[i])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[i];
			if ((int)actionRange != 0)
			{
				if (actionRange - 1 > 1)
				{
					continue;
				}
				Pole val2 = (Pole)((int)actionRange != 1);
				if ((int)val.axisType == 2 || (int)val.elementType == 1)
				{
					if (val.axisContribution == val2)
					{
						results.Add(Create(val, val2));
						usedAems.Array[i] = true;
						if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
						{
							return results.Count - count;
						}
					}
				}
				else if ((int)val.axisType == 1 && ActionElementMapHelper.TryGetSplitAxisMaps(val, ref val3, ref val4))
				{
					results.Add(Create((val3.axisContribution == val2) ? val3 : val4, val2));
					usedAems.Array[i] = true;
					if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
					{
						return results.Count - count;
					}
				}
			}
			else if ((int)val.axisRange == 0 && (int)val.elementType == 0)
			{
				results.Add(new ActionElementMapPair(val, null));
				usedAems.Array[i] = true;
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		if ((int)actionRange == 0)
		{
			for (int j = 0; j < count2; j++)
			{
				if (usedAems.Array[j])
				{
					continue;
				}
				bool flag = false;
				ActionElementMap val = actionElementMaps.Array[j];
				if (((int)val.axisType != 2 && (int)val.elementType != 1) || (int)val.axisContribution != 0)
				{
					continue;
				}
				for (int k = count; k < results.Count; k++)
				{
					if (results[k].a == val && results[k].b == null)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					results.Add(new ActionElementMapPair(val, null));
					usedAems.Array[j] = true;
					if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
					{
						return results.Count - count;
					}
				}
			}
		}
		return results.Count - count;
	}

	private static int FindSplitAxisBindingPairsOnly(FastList<ActionElementMap> actionElementMaps, FastList<bool> usedAems, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		int count = results.Count;
		int count2 = actionElementMaps.Count;
		ActionElementMapPair destination = default(ActionElementMapPair);
		for (int i = 0; i < count2; i++)
		{
			if (usedAems.Array[i])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[i];
			if ((int)val.elementType != 0 || (int)val.axisType == 1 || (int)val.axisType == 0)
			{
				continue;
			}
			Pole val2 = (Pole)((int)val.axisContribution == 0);
			int index;
			ActionElementMap val3 = Find(actionElementMaps, 0, (ControllerElementType)0, val.elementIdentifierId, (AxisType)2, val2, out index, usedAems);
			if (val3 == null)
			{
				val3 = Find(actionElementMaps, 0, (ControllerElementType)0, (AxisType)2, val2, out index, usedAems);
			}
			if (val3 != null)
			{
				Set(val, val.axisContribution, ref destination);
				Set(val3, val2, ref destination);
				results.Add(destination);
				Clear(ref destination);
				usedAems.Array[i] = true;
				usedAems.Array[index] = true;
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		return results.Count - count;
	}

	private static int FindButtonBindingPairsOnly(FastList<ActionElementMap> actionElementMaps, FastList<bool> usedAems, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		int count = results.Count;
		int count2 = actionElementMaps.Count;
		ActionElementMapPair destination = default(ActionElementMapPair);
		for (int i = 0; i < count2; i++)
		{
			if (usedAems.Array[i])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[i];
			if ((int)val.elementType != 1)
			{
				continue;
			}
			Pole val2 = (Pole)((int)val.axisContribution == 0);
			int index;
			ActionElementMap val3 = Find(actionElementMaps, 0, (ControllerElementType)1, (AxisType)0, val2, out index, usedAems);
			if (val3 != null)
			{
				Set(val, val.axisContribution, ref destination);
				Set(val3, val2, ref destination);
				results.Add(destination);
				Clear(ref destination);
				usedAems.Array[i] = true;
				usedAems.Array[index] = true;
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		return results.Count - count;
	}

	private static int FindSplitAxisAndButtonBindingPairsAndRemaining(FastList<ActionElementMap> actionElementMaps, FastList<bool> usedAems, List<ActionElementMapPair> results, ref int resultsRemainingCount)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Invalid comparison between Unknown and I4
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Invalid comparison between Unknown and I4
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (resultsRemainingCount == 0)
		{
			return 0;
		}
		int count = results.Count;
		int count2 = actionElementMaps.Count;
		ActionElementMapPair destination = default(ActionElementMapPair);
		for (int i = 0; i < count2; i++)
		{
			if (usedAems.Array[i])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[i];
			if ((int)val.elementType == 0)
			{
				if ((int)val.axisType == 1 || (int)val.axisType == 0)
				{
					continue;
				}
			}
			else if ((int)val.elementType != 1)
			{
				continue;
			}
			Pole val2 = (Pole)((int)val.axisContribution == 0);
			ActionElementMap val3;
			int index;
			if ((int)val.elementType == 0)
			{
				val3 = Find(actionElementMaps, 0, (ControllerElementType)0, val.elementIdentifierId, (AxisType)2, val2, out index, usedAems);
				if (val3 == null)
				{
					val3 = Find(actionElementMaps, 0, (ControllerElementType)0, (AxisType)2, val2, out index, usedAems);
				}
			}
			else
			{
				val3 = Find(actionElementMaps, 0, (ControllerElementType)1, (AxisType)0, val2, out index, usedAems);
			}
			if (val3 != null)
			{
				Set(val, val.axisContribution, ref destination);
				Set(val3, val2, ref destination);
				results.Add(destination);
				Clear(ref destination);
				usedAems.Array[i] = true;
				usedAems.Array[index] = true;
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		for (int j = 0; j < count2; j++)
		{
			if (usedAems.Array[j])
			{
				continue;
			}
			ActionElementMap val = actionElementMaps.Array[j];
			if ((int)val.elementType == 0)
			{
				if ((int)val.axisType == 1 || (int)val.axisType == 0)
				{
					continue;
				}
			}
			else if ((int)val.elementType != 1)
			{
				continue;
			}
			if (Get(destination, val.axisContribution) == null)
			{
				Set(val, val.axisContribution, ref destination);
				usedAems.Array[j] = true;
			}
			if (destination.Count == 2)
			{
				results.Add(destination);
				Clear(ref destination);
				if (!AllowMoreResultsDecrement(ref resultsRemainingCount))
				{
					return results.Count - count;
				}
			}
		}
		if (destination.Count > 0)
		{
			results.Add(destination);
			AllowMoreResultsDecrement(ref resultsRemainingCount);
			return results.Count - count;
		}
		return results.Count - count;
	}

	private static int GetElementMapsWithAction(Player player, ControllerType controllerType, int controllerId, int actionId, Predicate<ActionElementMap> isAllowedPredicate, ControllerElementType[] searchOrder, FastList<ActionElementMap> results)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		results.Clear();
		player.controllers.maps.GetElementMapsWithAction(controllerType, controllerId, actionId, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, results);
		RemoveInvalidElementMaps(player, results, 0, isAllowedPredicate);
		GetElementMapsWithAction_tempAems.Clear();
		return results.Count;
	}

	private static int GetElementMapsWithAction(ControllerMap controllerMap, int actionId, Predicate<ActionElementMap> isAllowedPredicate, ControllerElementType[] searchOrder, FastList<ActionElementMap> results)
	{
		results.Clear();
		if (controllerMap == null)
		{
			return 0;
		}
		controllerMap.GetElementMapsWithAction(actionId, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, results);
		RemoveInvalidElementMaps(results, 0, isAllowedPredicate);
		GetElementMapsWithAction_tempAems.Clear();
		return results.Count;
	}

	private static int GetElementMapsWithAction(Player player, ControllerType controllerType, int controllerId, int actionId, int actionId2, Predicate<ActionElementMap> isAllowedPredicate, ControllerElementType[] searchOrder, FastList<ActionElementMap> action1Results, FastList<ActionElementMap> action2Results)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		action1Results.Clear();
		action2Results.Clear();
		player.controllers.maps.GetElementMapsWithAction(controllerType, controllerId, actionId, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, action1Results);
		RemoveInvalidElementMaps(player, action1Results, 0, isAllowedPredicate);
		player.controllers.maps.GetElementMapsWithAction(controllerType, controllerId, actionId2, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, action2Results);
		RemoveInvalidElementMaps(player, action2Results, 0, isAllowedPredicate);
		GetElementMapsWithAction_tempAems.Clear();
		return action1Results.Count + action2Results.Count;
	}

	private static int GetElementMapsWithAction(ControllerMap controllerMap, int actionId, int actionId2, Predicate<ActionElementMap> isAllowedPredicate, ControllerElementType[] searchOrder, FastList<ActionElementMap> action1Results, FastList<ActionElementMap> action2Results)
	{
		action1Results.Clear();
		action2Results.Clear();
		if (controllerMap == null)
		{
			return 0;
		}
		controllerMap.GetElementMapsWithAction(actionId, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, action1Results);
		RemoveInvalidElementMaps(action1Results, 0, isAllowedPredicate);
		controllerMap.GetElementMapsWithAction(actionId2, false, GetElementMapsWithAction_tempAems);
		SortByElementType(GetElementMapsWithAction_tempAems, searchOrder, action2Results);
		RemoveInvalidElementMaps(action2Results, 0, isAllowedPredicate);
		GetElementMapsWithAction_tempAems.Clear();
		return action1Results.Count + action2Results.Count;
	}

	private static int RemoveInvalidElementMaps(Player player, FastList<ActionElementMap> results, int startIndex, Predicate<ActionElementMap> isAllowedPredicate)
	{
		int count = results.Count;
		for (int num = count - 1; num >= startIndex; num--)
		{
			if (!player.controllers.ContainsController(results.Array[num].controllerMap.controller) || !results.Array[num].controllerMap.controller.enabled)
			{
				results.RemoveAt(num);
			}
		}
		RemoveInvalidElementMaps(results, startIndex, isAllowedPredicate);
		return results.Count - count;
	}

	private static int RemoveInvalidElementMaps(FastList<ActionElementMap> results, int startIndex, Predicate<ActionElementMap> isAllowedPredicate)
	{
		int count = results.Count;
		if (isAllowedPredicate != null)
		{
			int num = results.Count;
			for (int i = startIndex; i < num; i++)
			{
				bool flag = false;
				try
				{
					if (!isAllowedPredicate(results.Array[i]))
					{
						flag = true;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Rewired: An exception was thrown in isAllowedPredicate callback. This exception was thrown by your code.\n" + ex);
					continue;
				}
				if (flag)
				{
					results.RemoveAt(i);
					num--;
					i--;
				}
			}
		}
		return results.Count - count;
	}

	private static ActionElementMap Find(FastList<ActionElementMap> list, int startIndex, ControllerElementType controllerElementType, AxisType axisType, out int index, FastList<bool> used)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int count = list.Count;
		for (int i = startIndex; i < count; i++)
		{
			if (!used.Array[i])
			{
				ActionElementMap val = list.Array[i];
				if (val.elementType == controllerElementType && val.axisType == axisType)
				{
					index = i;
					return val;
				}
			}
		}
		index = -1;
		return null;
	}

	private static ActionElementMap Find(FastList<ActionElementMap> list, int startIndex, ControllerElementType controllerElementType, AxisType axisType, Pole axisContribution, out int index, FastList<bool> used)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int count = list.Count;
		for (int i = startIndex; i < count; i++)
		{
			if (!used.Array[i])
			{
				ActionElementMap val = list.Array[i];
				if (val.elementType == controllerElementType && val.axisType == axisType && val.axisContribution == axisContribution)
				{
					index = i;
					return val;
				}
			}
		}
		index = -1;
		return null;
	}

	private static ActionElementMap Find(FastList<ActionElementMap> list, int startIndex, ControllerElementType controllerElementType, int elementIdentifierId, AxisType axisType, Pole axisContribution, out int index, FastList<bool> used)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int count = list.Count;
		for (int i = startIndex; i < count; i++)
		{
			if (!used.Array[i])
			{
				ActionElementMap val = list.Array[i];
				if (val.elementType == controllerElementType && val.elementIdentifierId == elementIdentifierId && val.axisType == axisType && val.axisContribution == axisContribution)
				{
					index = i;
					return val;
				}
			}
		}
		index = -1;
		return null;
	}

	private static bool Contains(FastList<ControllerInfo> list, ControllerType type, int id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < list.Count; i++)
		{
			if (list.Array[i].type == type && list.Array[i].controllerId == id)
			{
				return true;
			}
		}
		return false;
	}

	private static Pair<ActionElementMapPair> Create(ActionElementMapPair a, ActionElementMapPair b, bool reverse)
	{
		if (reverse)
		{
			return new Pair<ActionElementMapPair>(b, a);
		}
		return new Pair<ActionElementMapPair>(a, b);
	}

	private static ActionElementMapPair Create(ActionElementMap aem, Pole pole)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		if ((int)pole != 0)
		{
			if ((int)pole == 1)
			{
				return new ActionElementMapPair(aem, null);
			}
			throw new NotImplementedException();
		}
		return new ActionElementMapPair(null, aem);
	}

	private static bool TryCreate(ActionElementMap aem1, ActionElementMap aem2, out ActionElementMapPair result)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		result = default(ActionElementMapPair);
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			ActionElementMap val = ((i == 0) ? aem1 : aem2);
			if (val == null)
			{
				continue;
			}
			if ((int)val.axisContribution == 1)
			{
				if (result.a != null)
				{
					flag = true;
				}
				else
				{
					result.a = val;
				}
			}
			else if (result.b != null)
			{
				flag = true;
			}
			else
			{
				result.b = val;
			}
		}
		return !flag;
	}

	private static bool SetAndAddIfFull(ActionElementMapPair item, int index, ref Pair<ActionElementMapPair> target, List<Pair<ActionElementMapPair>> items)
	{
		bool result = false;
		if (!TrySet(item, index, ref target))
		{
			items.Add(target);
			result = true;
			Clear(ref target);
			TrySet(item, index, ref target);
		}
		if (target.a.Count > 0 && target.b.Count > 0)
		{
			items.Add(target);
			result = true;
			Clear(ref target);
		}
		return result;
	}

	private static bool TrySet(ActionElementMapPair item, int index, ref Pair<ActionElementMapPair> target)
	{
		switch (index)
		{
		case 0:
			if (target.a.Count > 0)
			{
				return false;
			}
			target.a = item;
			return true;
		case 1:
			if (target.b.Count > 0)
			{
				return false;
			}
			target.b = item;
			return true;
		default:
			throw new ArgumentOutOfRangeException("index");
		}
	}

	private static void Set(ActionElementMap aem, Pole pole, ref ActionElementMapPair destination)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		if ((int)pole != 0)
		{
			if ((int)pole != 1)
			{
				throw new NotImplementedException();
			}
			destination.a = aem;
		}
		else
		{
			destination.b = aem;
		}
	}

	private static ActionElementMap Get(ActionElementMapPair source, Pole pole)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		if ((int)pole != 0)
		{
			if ((int)pole == 1)
			{
				return source.a;
			}
			throw new NotImplementedException();
		}
		return source.b;
	}

	private static void Clear(ref Pair<ActionElementMapPair> target)
	{
		Clear(ref target.a);
		Clear(ref target.b);
	}

	private static void Clear(ref ActionElementMapPair target)
	{
		target.a = null;
		target.b = null;
	}

	private static void SortByElementType(List<ActionElementMap> aems, ControllerElementType[] controllerElementTypes, FastList<ActionElementMap> results)
	{
		results.Clear();
		results.ReplaceFrom(aems);
		SortByElementType(results, controllerElementTypes);
	}

	private static void SortByElementType(FastList<ActionElementMap> aems, ControllerElementType[] controllerElementTypes)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		FastList<ActionElementMap> fastList = aemFastListPool.Get();
		FastList<bool> usedPooledList = GetUsedPooledList(aems.Count);
		for (int i = 0; i < controllerElementTypes.Length; i++)
		{
			for (int j = 0; j < aems.Count; j++)
			{
				if ((int)aems.Array[j].elementType == (int)controllerElementTypes[i])
				{
					fastList.Add(aems.Array[j]);
					usedPooledList.Array[j] = true;
				}
			}
		}
		if (fastList.Count < aems.Count)
		{
			for (int k = 0; k < aems.Count; k++)
			{
				if (!usedPooledList.Array[k])
				{
					fastList.Add(aems.Array[k]);
				}
			}
		}
		aems.ReplaceFrom(fastList);
		aemFastListPool.Return(fastList);
		boolFastListPool.Return(usedPooledList);
	}

	private static bool AllowMoreResultsDecrement(ref int remainingCount)
	{
		if (remainingCount < 0)
		{
			return true;
		}
		remainingCount--;
		if (remainingCount < 0)
		{
			remainingCount = 0;
		}
		return remainingCount > 0;
	}

	private static FastList<bool> GetUsedPooledList(int count)
	{
		FastList<bool> fastList = boolFastListPool.Get();
		fastList.SetCount(count);
		return fastList;
	}

	private static void ReturnUsedPoolList(FastList<bool> list)
	{
		boolFastListPool.Return(list);
	}
}
