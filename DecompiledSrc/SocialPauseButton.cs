using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SocialPauseButton : GeneralPauseButton
{
	public TMP_Text label;

	public Image image;

	public Image background;

	public Button button;

	public Action action;

	public override void Select()
	{
		action?.Invoke();
	}

	public override void SetFocus(bool focus)
	{
		image.color = (focus ? Color.white : new Color(0.6078432f, 0.6078432f, 0.6078432f));
		TMP_Text tMP_Text = label;
		bool flag = (background.enabled = focus);
		tMP_Text.enabled = flag;
	}
}
