using UnityEngine;
using UnityEngine.UI;

public class scrShortcutText : MonoBehaviour
{
	public bool isHeader;

	public KeyCode keyCode;

	public KeyCode otherKeyCode;

	public bool usingShift;

	public bool usingCtrl;

	public bool usingAlt;

	public string key;

	public bool otherKeyDoesntUseModifierKeys;

	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	public void SetText()
	{
		if (isHeader)
		{
			this.text.text = RDString.Get("editor.shortcuts." + key);
			return;
		}
		string text = RDEditorUtils.KeyComboToString(usingCtrl, usingShift, usingAlt, keyCode);
		string text2 = string.Empty;
		if (otherKeyCode != KeyCode.None)
		{
			if (otherKeyDoesntUseModifierKeys)
			{
				usingCtrl = (usingAlt = (usingShift = false));
			}
			text2 = " " + RDString.Get("editor.shortcuts.or") + " " + RDEditorUtils.KeyComboToString(usingCtrl, usingShift, usingAlt, otherKeyCode);
		}
		this.text.text = text + text2 + ": " + RDString.Get("editor.shortcuts." + key);
	}
}
