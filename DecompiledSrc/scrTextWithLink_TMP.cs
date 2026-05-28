using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrTextWithLink_TMP : ADOBase
{
	public Color color;

	public TMP_Text text;

	public string url;

	public Button button;

	public bool localized;

	public string localizationKey = "";

	private string linkStartString;

	private string linkEndString;

	private string linkStartMarker;

	private string linkEndMarker;

	private void Start()
	{
		base.gameObject.AddComponent<TMP_TextHyperlinks>();
		button.onClick.AddListener(delegate
		{
			ADOBase.platformHelper.OpenURL(url);
		});
		linkStartString = "<link=" + url + "><b><color=" + color.ToHex() + ">";
		linkEndString = "</color></b></link>";
		linkStartMarker = (localized ? "[[" : "[");
		linkEndMarker = (localized ? "]]" : "]");
		if (localized)
		{
			text.text = RDString.Get(localizationKey);
		}
		text.text = text.text.Replace(linkStartMarker, linkStartString).Replace(linkEndMarker, linkEndString);
	}
}
