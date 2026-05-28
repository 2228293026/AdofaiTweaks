using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrOptionsExperiencingText : ADOBase
{
	public CanvasScaler canvasScaler;

	public List<TMP_Text> experiencingTexts;

	public float speed = -150f;

	public float distance = 200f;

	public float width;

	public float fullDistance;

	private string baseString;

	private bool init;

	private bool generatedWidth;

	private void Awake()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low && baseString == null)
		{
			Init(0f);
		}
	}

	private void Update()
	{
		if (!init)
		{
			return;
		}
		if (!generatedWidth)
		{
			GenerateWidth();
		}
		if (generatedWidth)
		{
			for (int i = 0; i < experiencingTexts.Count; i++)
			{
				TMP_Text tMP_Text = experiencingTexts[i];
				scrMisc.AnchorPosX(x: (fullDistance * (float)i + Time.unscaledTime * speed) % ((0f - fullDistance) * (float)experiencingTexts.Count), rt: tMP_Text.rectTransform);
			}
		}
	}

	private void GenerateWidth()
	{
		TMP_Text tMP_Text = experiencingTexts[0];
		tMP_Text.text = baseString.Replace("[angle]", "90");
		tMP_Text.ForceMeshUpdate();
		fullDistance = distance + tMP_Text.textBounds.size.x;
		generatedWidth = true;
		(base.transform as RectTransform).AnchorPosY(0f);
		base.gameObject.SetActive(value: false);
	}

	private void Init(float angle)
	{
		if (!init)
		{
			init = true;
			baseString = RDString.Get("frumsOptions.experiencing");
			if (baseString == null)
			{
				baseString = "[angle]";
			}
			SetAngleText(angle);
		}
	}

	public void SetAngleText(float angle)
	{
		if (!init)
		{
			Init(angle);
		}
		foreach (TMP_Text experiencingText in experiencingTexts)
		{
			experiencingText.text = baseString.Replace("[angle]", angle.ToString());
		}
	}
}
