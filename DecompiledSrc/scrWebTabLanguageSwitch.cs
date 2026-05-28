using UnityEngine;

public class scrWebTabLanguageSwitch : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			RDConstants.data.forceLanguage = true;
			RDString.ChangeLanguage((RDString.language == SystemLanguage.English) ? SystemLanguage.ChineseSimplified : SystemLanguage.English);
			scrController.instance.Restart();
		}
	}
}
