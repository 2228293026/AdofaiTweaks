using UnityEngine.UI;

public class scrMenuContinueInfo : ADOBase
{
	private void Start()
	{
		string savedCurrentLevel = Persistence.savedCurrentLevel;
		Text component = GetComponent<Text>();
		component.SetLocalizedFont();
		if (savedCurrentLevel != "0-0")
		{
			string text = savedCurrentLevel;
			bool flag = false;
			if (text.EndsWith("EX-X"))
			{
				text = text.Replace("EX-X", "-X");
				flag = true;
			}
			string text2 = RDUtils.RemoveRichTags(ADOBase.GetLocalizedLevelName(text)) ?? "";
			if (flag)
			{
				text2 = text2.Replace("-X", "-EX");
			}
			component.text = text2;
		}
		else
		{
			component.text = "";
		}
	}
}
