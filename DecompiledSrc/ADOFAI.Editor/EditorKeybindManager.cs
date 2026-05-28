using System.Collections;
using System.Collections.Generic;
using ADOFAI.Editor.Actions;

namespace ADOFAI.Editor;

public class EditorKeybindManager : IEnumerable<KeyValuePair<EditorKeybind, List<EditorAction>>>, IEnumerable
{
	private scnEditor editor;

	private Dictionary<EditorKeybind, List<EditorAction>> actionDict;

	public Dictionary<EditorKeybind, List<EditorAction>> dictionary => actionDict;

	public EditorKeybindManager(scnEditor editor)
	{
		this.editor = editor;
		actionDict = new Dictionary<EditorKeybind, List<EditorAction>>();
	}

	public void RegisterKeybind(EditorKeybind keybind, EditorAction action)
	{
		if (!actionDict.ContainsKey(keybind))
		{
			actionDict[keybind] = new List<EditorAction>();
		}
		actionDict[keybind].Add(action);
	}

	public void UnregisterKeybind(EditorKeybind keybind)
	{
		actionDict.Remove(keybind);
	}

	public bool ExecutePressedActions()
	{
		foreach (KeyValuePair<EditorKeybind, List<EditorAction>> item in actionDict)
		{
			if (item.Key.IsPressed())
			{
				item.Value.ForEach(delegate(EditorAction action)
				{
					action.Execute(editor);
				});
				return true;
			}
		}
		return false;
	}

	public IEnumerator<KeyValuePair<EditorKeybind, List<EditorAction>>> GetEnumerator()
	{
		return actionDict.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
