using UnityEngine;
using UnityEngine.UI;

public class scrDebugHUDMessage : MonoBehaviour
{
	private Text txt;

	private static float alpha = 1f;

	private static string logtext;

	private void Start()
	{
		txt = GetComponent<Text>();
	}

	private void Update()
	{
		if (alpha > 0f)
		{
			alpha -= 0.02f;
			txt.color = scrVfx.instance.currentColourScheme.colourText;
			txt.color = txt.color.WithAlpha(alpha);
			txt.text = logtext;
		}
	}

	public static void Log(string msg)
	{
		logtext = msg;
		alpha = 1f;
	}

	public static void LogBool(bool thebool, string name)
	{
		if (thebool)
		{
			Log(name + " on");
		}
		else
		{
			Log(name + " off");
		}
	}
}
