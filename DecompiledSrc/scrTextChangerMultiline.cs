using UnityEngine;
using UnityEngine.UI;

public class scrTextChangerMultiline : ADOBase
{
	public string[] localizationTokens;

	public float[] scales;

	private Text textComponent;

	private void Start()
	{
		if (!base.enabled)
		{
			return;
		}
		textComponent = GetComponent<Text>();
		textComponent.SetLocalizedFont();
		string text = "";
		int num = 0;
		string[] array = localizationTokens;
		foreach (string key in array)
		{
			bool exists = false;
			string withCheck = RDString.GetWithCheck(key, out exists);
			if (exists)
			{
				if (scales.Length == localizationTokens.Length)
				{
					int num2 = Mathf.RoundToInt((float)textComponent.fontSize * scales[num]);
					text += $"<size={num2}>{withCheck}</size>";
				}
				else
				{
					text += withCheck;
				}
			}
			if (num < localizationTokens.Length - 1)
			{
				text += "\n";
			}
			num++;
		}
		textComponent.text = text;
	}
}
