using UnityEngine;

public class scrButtonURL_OSBased : scrButtonURL
{
	public string macLink;

	public override void OpenURL()
	{
		string text = ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) ? macLink : link);
		scrController.instance.TogglePauseGame();
		Application.OpenURL(localized ? RDString.Get(text) : text);
	}
}
