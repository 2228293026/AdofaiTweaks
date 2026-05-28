using System;
using UnityEngine;

namespace ADOFAI.Editor;

public struct EditorKeybind
{
	public KeyModifier modifierMask;

	public KeyCode key;

	public bool ctrlIsCmd;

	public EditorKeybind(KeyCode key)
	{
		this.key = key;
		modifierMask = KeyModifier.None;
		ctrlIsCmd = true;
	}

	public EditorKeybind(KeyModifier modifierMask, KeyCode key, bool ctrlIsCmd = true)
	{
		this.key = key;
		this.modifierMask = modifierMask;
		this.ctrlIsCmd = ctrlIsCmd;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is EditorKeybind))
		{
			return false;
		}
		return Equals((EditorKeybind)obj);
	}

	public bool Equals(EditorKeybind other)
	{
		if (modifierMask == other.modifierMask)
		{
			return key == other.key;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<KeyModifier, KeyCode>(modifierMask, key);
	}

	public bool IsReleased()
	{
		if (ModifiersHeld())
		{
			return Input.GetKeyUp(key);
		}
		return false;
	}

	public bool IsPressed()
	{
		if (ModifiersHeld())
		{
			return Input.GetKeyDown(key);
		}
		return false;
	}

	public bool IsHeld()
	{
		if (ModifiersHeld())
		{
			return Input.GetKey(key);
		}
		return false;
	}

	private bool ModifiersHeld()
	{
		KeyModifier keyModifier = KeyModifier.None;
		if (RDInput.holdingShift)
		{
			keyModifier |= KeyModifier.Shift;
		}
		if (RDInput.holdingControl)
		{
			keyModifier |= KeyModifier.Control;
		}
		if (RDInput.holdingAlt)
		{
			keyModifier |= KeyModifier.Alt;
		}
		if (Input.GetKey(KeyCode.BackQuote))
		{
			keyModifier |= KeyModifier.BackQuote;
		}
		return keyModifier == modifierMask;
	}

	public static bool operator ==(EditorKeybind a, EditorKeybind b)
	{
		if (a.key == b.key)
		{
			return a.modifierMask == b.modifierMask;
		}
		return false;
	}

	public static bool operator !=(EditorKeybind a, EditorKeybind b)
	{
		return !(a == b);
	}
}
