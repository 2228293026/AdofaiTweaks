using System;
using UnityEngine;

public abstract class GeneralPauseButton : ADOBase
{
	[NonSerialized]
	public int index;

	[NonSerialized]
	public RectTransform rectangleRT;

	public RectTransform rectTransform => GetComponent<RectTransform>();

	protected PauseMenu pauseMenu => scrController.instance.pauseMenu;

	public abstract void SetFocus(bool value);

	public abstract void Select();
}
