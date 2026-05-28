using System.Collections.Generic;
using UnityEngine;

public class RDCheatCode
{
	private readonly KeyCode[] cheatCode;

	public int currentIndex;

	public RDCheatCode(KeyCode[] _cheatCode)
	{
		cheatCode = _cheatCode;
		currentIndex = 0;
	}

	public RDCheatCode(string _cheatCode)
	{
		_cheatCode = _cheatCode.ToUpper();
		List<KeyCode> list = new List<KeyCode>();
		for (int i = 0; i < _cheatCode.Length; i++)
		{
			KeyCode keyCode = KeyCode.None;
			char c = _cheatCode[i];
			if (c >= 'A' && c <= 'Z')
			{
				keyCode = (KeyCode)(c - 65 + 97);
			}
			else if (c >= '0' && c <= '9')
			{
				keyCode = (KeyCode)(c - 48 + 48);
			}
			else
			{
				switch (c)
				{
				case '→':
					keyCode = KeyCode.RightArrow;
					break;
				case '↑':
					keyCode = KeyCode.UpArrow;
					break;
				case '←':
					keyCode = KeyCode.LeftArrow;
					break;
				case '↓':
					keyCode = KeyCode.DownArrow;
					break;
				case '⏎':
					keyCode = KeyCode.Return;
					break;
				}
			}
			if (keyCode != KeyCode.None)
			{
				list.Add(keyCode);
			}
		}
		cheatCode = list.ToArray();
		currentIndex = 0;
	}

	public bool CheckCheatCode()
	{
		if (currentIndex == cheatCode.Length)
		{
			currentIndex = 0;
		}
		if (Input.anyKeyDown && !Input.GetMouseButton(0))
		{
			if (Input.GetKeyDown(cheatCode[currentIndex]))
			{
				currentIndex++;
			}
			else
			{
				currentIndex = 0;
			}
		}
		return currentIndex == cheatCode.Length;
	}
}
