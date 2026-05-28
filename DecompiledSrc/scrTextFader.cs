using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrTextFader : ADOBase
{
	public int activephase;

	public bool typing = true;

	public string oritext;

	private Color oricolor;

	private Text text;

	private bool fadeToggle = true;

	private void Start()
	{
		text = GetComponent<Text>();
		oricolor = text.color;
		oritext = text.text;
		if (ADOBase.levelSelectBase.menuPhase != activephase)
		{
			text.DOColor(text.color.WithAlpha(0f), 0f);
		}
	}

	private void Update()
	{
		if (ADOBase.levelSelectBase.menuPhase == activephase)
		{
			if (!fadeToggle)
			{
				fadeToggle = true;
				if (typing)
				{
					text.DOText(oritext, 1f);
				}
				else
				{
					text.DOColor(oricolor, 0.4f);
				}
			}
		}
		else if (fadeToggle)
		{
			fadeToggle = false;
			if (typing)
			{
				text.DOText("", 0f);
			}
			else
			{
				text.DOColor(text.color.WithAlpha(0f), 0.4f);
			}
		}
	}
}
