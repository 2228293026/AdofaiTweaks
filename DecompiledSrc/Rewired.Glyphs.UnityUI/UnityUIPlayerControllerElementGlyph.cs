using System;
using UnityEngine;

namespace Rewired.Glyphs.UnityUI;

[AddComponentMenu("Rewired/Glyphs/Unity UI/Unity UI Player Controller Element Glyph")]
public class UnityUIPlayerControllerElementGlyph : UnityUIPlayerControllerElementGlyphBase
{
	[Tooltip("The Player id.")]
	[SerializeField]
	private int _playerId;

	[Tooltip("The Action name.")]
	[SerializeField]
	private string _actionName;

	[Tooltip("The second Action name for 2D Actions. (Optional)")]
	[SerializeField]
	private string _actionName2;

	[NonSerialized]
	private int _actionId = -1;

	[NonSerialized]
	private bool _actionIdCached;

	[NonSerialized]
	private int _actionId2 = -1;

	[NonSerialized]
	private bool _actionId2Cached;

	public override int playerId
	{
		get
		{
			return _playerId;
		}
		set
		{
			_playerId = value;
		}
	}

	public override int actionId
	{
		get
		{
			if (!_actionIdCached)
			{
				CacheActionId();
			}
			return _actionId;
		}
		set
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (value >= 0)
			{
				InputAction action = ReInput.mapping.GetAction(value);
				if (action == null)
				{
					Debug.LogError("Invalid Action id: " + value);
					return;
				}
				_actionName = action.name;
			}
			else
			{
				_actionName = string.Empty;
			}
			CacheActionId();
		}
	}

	public override int actionId2
	{
		get
		{
			if (!_actionId2Cached)
			{
				CacheActionId2();
			}
			return _actionId2;
		}
		set
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (value >= 0)
			{
				InputAction action = ReInput.mapping.GetAction(value);
				if (action == null)
				{
					Debug.LogError("Invalid Action id 2: " + value);
					return;
				}
				_actionName2 = action.name;
			}
			else
			{
				_actionName2 = string.Empty;
			}
			CacheActionId2();
		}
	}

	public string actionName
	{
		get
		{
			return _actionName;
		}
		set
		{
			if (ReInput.isReady && !string.IsNullOrEmpty(value))
			{
				InputAction action = ReInput.mapping.GetAction(value);
				if (action == null)
				{
					Debug.LogError("Invalid Action Name: " + value);
					return;
				}
				value = action.name;
			}
			_actionName = value;
			CacheActionId();
		}
	}

	public string actionName2
	{
		get
		{
			return _actionName2;
		}
		set
		{
			if (ReInput.isReady && !string.IsNullOrEmpty(value))
			{
				InputAction action = ReInput.mapping.GetAction(value);
				if (action == null)
				{
					Debug.LogError("Invalid Action Name 2: " + value);
					return;
				}
				value = action.name;
			}
			_actionName2 = value;
			CacheActionId2();
		}
	}

	private void CacheActionId()
	{
		if (ReInput.isReady)
		{
			InputAction action = ReInput.mapping.GetAction(_actionName);
			_actionId = ((action != null) ? action.id : (-1));
			_actionIdCached = true;
		}
	}

	private void CacheActionId2()
	{
		if (ReInput.isReady)
		{
			InputAction action = ReInput.mapping.GetAction(_actionName2);
			_actionId2 = ((action != null) ? action.id : (-1));
			_actionId2Cached = true;
		}
	}
}
