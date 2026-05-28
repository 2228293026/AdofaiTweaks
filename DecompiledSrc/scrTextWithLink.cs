using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrTextWithLink : ADOBase
{
	public Color color;

	public Text text;

	public string url;

	public Button button;

	public bool localized;

	public string localizationKey = "";

	private bool done;

	private string linkStartString;

	private string linkEndString;

	private string linkStartMarker;

	private string linkEndMarker;

	private void Start()
	{
		button.onClick.AddListener(delegate
		{
			Application.OpenURL(url);
		});
		linkStartString = "<b><color=" + color.ToHex() + ">";
		linkEndString = "</color></b>";
		linkStartMarker = (localized ? "[[" : "[");
		linkEndMarker = (localized ? "]]" : "]");
		if (localized)
		{
			text.text = RDString.Get(localizationKey);
		}
		text.text = text.text.Replace(linkStartMarker, linkStartString).Replace(linkEndMarker, linkEndString);
		text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.GetComponent<RectTransform>().sizeDelta));
	}

	private void Update()
	{
		if (!done)
		{
			Process();
		}
	}

	private void Process()
	{
		string text = this.text.text;
		IList<UICharInfo> characters = this.text.cachedTextGenerator.characters;
		if (characters.Count != 0)
		{
			int num = text.IndexOf(linkStartString);
			int num2 = text.IndexOf(linkEndString);
			if (num == -1 || num2 == -1)
			{
				base.enabled = false;
				return;
			}
			float num3 = ADOBase.editor.GetComponent<CanvasScaler>().referenceResolution.y / (float)Screen.height;
			float x = characters[0].cursorPos.x;
			float num4 = (characters[num].cursorPos.x - x) * num3;
			float num5 = (characters[num2 + 1].cursorPos.x - x) * num3;
			RectTransform component = button.GetComponent<RectTransform>();
			component.AnchorPosX(num4);
			component.SizeDeltaX(num5 - num4);
			done = true;
		}
	}
}
