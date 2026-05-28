using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrTextChanger : ADOBase
{
	[Header("Localization keys")]
	public string desktopText;

	public string mobileText;

	public string boothText;

	public string drumText;

	public string drumboothText;

	[Header("Stage")]
	public bool customOverallStage;

	public int minStage;

	public int maxStage;

	[Header("Other")]
	[Tooltip("Enclose the localized text in {s}.")]
	public string format;

	[Tooltip("Changes the text's [[ and ]] for <color=red> and </color> respectively.")]
	public bool specialFormatting;

	[Tooltip("If checked, it won't change the font.")]
	public bool keepFont;

	private Text text;

	private TMP_Text tmpText;

	private void Start()
	{
		if (!base.enabled)
		{
			return;
		}
		if (customOverallStage)
		{
			int overallProgressStage = Persistence.GetOverallProgressStage();
			if (overallProgressStage < minStage || overallProgressStage > maxStage)
			{
				return;
			}
		}
		this.text = GetComponent<Text>();
		tmpText = GetComponent<TMP_Text>();
		string text = (tmpText ? tmpText.text : this.text.text);
		if (desktopText != "")
		{
			text = desktopText;
		}
		if (GCS.d_booth && boothText != "")
		{
			text = boothText;
		}
		if (GCS.d_drumcontroller && drumText != "")
		{
			text = drumText;
		}
		if (ADOBase.isMobile & (mobileText != ""))
		{
			text = mobileText;
		}
		if (GCS.d_drumcontroller && GCS.d_booth && drumboothText != "")
		{
			text = drumboothText;
		}
		if (text == "levelSelect.subtitle" && ADOBase.IsAprilFools() && !GCS.FOOL_JOKER)
		{
			text = "levelSelect.aprilFools";
		}
		scrTextFader component = GetComponent<scrTextFader>();
		if (component != null)
		{
			component.oritext = this.text.text;
		}
		bool exists;
		string text2 = RDString.GetWithCheck(text, out exists);
		if (exists && !keepFont)
		{
			if ((bool)this.text)
			{
				this.text.SetLocalizedFont();
			}
			if ((bool)tmpText)
			{
				tmpText.SetLocalizedFont();
			}
		}
		if (specialFormatting)
		{
			text2 = text2.Replace("[[", "<color=red>").Replace("]]", "</color>");
		}
		if (!string.IsNullOrEmpty(format))
		{
			text2 = format.Replace("{s}", text2);
		}
		if ((bool)this.text)
		{
			this.text.text = (exists ? text2 : text);
		}
		if ((bool)tmpText)
		{
			tmpText.text = (exists ? text2 : text);
		}
	}
}
