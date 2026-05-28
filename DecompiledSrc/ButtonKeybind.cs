using ADOFAI.Editor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonKeybind : MonoBehaviour
{
	[Header("Keybind")]
	public KeyModifier keyModifier;

	public KeyCode keyCode;

	private Button button;

	private EditorKeybind keybind;

	private void Awake()
	{
		button = GetComponent<Button>();
		keybind = new EditorKeybind(keyModifier, keyCode);
	}

	private void Update()
	{
		if (!(button == null) && keybind.IsPressed())
		{
			button.onClick.Invoke();
		}
	}
}
