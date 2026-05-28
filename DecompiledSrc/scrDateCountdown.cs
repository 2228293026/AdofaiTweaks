using System;
using UnityEngine.UI;

public class scrDateCountdown : ADOBase
{
	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		text.text = string.Empty;
	}

	private void Update()
	{
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan timeSpan = new DateTime(2020, 6, 28, 17, 0, 0) - utcNow;
		int num = (int)timeSpan.TotalHours;
		string text = timeSpan.Minutes.ToString();
		if (text.Length == 1)
		{
			text = "0" + text;
		}
		string text2 = timeSpan.Seconds.ToString();
		if (text2.Length == 1)
		{
			text2 = "0" + text2;
		}
		this.text.text = $"{num}:{text}:{text2}";
	}
}
