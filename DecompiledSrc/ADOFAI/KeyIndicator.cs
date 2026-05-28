using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI;

public class KeyIndicator : ADOBase
{
	public KeyCode keyCodeA;

	public KeyCode keyCodeB;

	public Graphic[] graphics;

	public Color idleColor;

	public Color activeColor;

	public TMP_Text text;

	public Sprite spaceImage;

	public Sprite tabImage;

	public Image border;

	private void LateUpdate()
	{
		KeyCode[] obj = new KeyCode[2] { keyCodeA, keyCodeB };
		bool flag = false;
		KeyCode[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			if (Input.GetKey(array[i]))
			{
				flag = true;
			}
		}
		bool flag2 = false;
		GameObject currentSelectedGameObject = ADOBase.editor.eventSystem.currentSelectedGameObject;
		if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<Selectable>() != null)
		{
			flag2 = true;
		}
		Graphic[] array2 = graphics;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].color = ((flag && !flag2) ? activeColor : idleColor);
		}
	}

	public void SetKeyCode(KeyCode newKeyA, KeyCode newKeyB = KeyCode.None)
	{
		keyCodeA = newKeyA;
		keyCodeB = newKeyB;
		if (keyCodeA == KeyCode.Space)
		{
			text.text = "Space";
			if ((bool)spaceImage)
			{
				border.sprite = spaceImage;
			}
		}
		else if (keyCodeA == KeyCode.Tab)
		{
			text.text = "Tab";
			if ((bool)tabImage)
			{
				border.sprite = tabImage;
			}
		}
		else if (keyCodeA == KeyCode.LeftShift || keyCodeA == KeyCode.RightShift)
		{
			text.text = "Shift";
		}
		else
		{
			text.text = keyCodeA.ToString();
		}
	}
}
