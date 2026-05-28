using System;
using UnityEngine;

public class scrKongAPI : MonoBehaviour
{
	private static scrKongAPI instance;

	public static bool Connected { get; private set; }

	public static int UserId { get; private set; }

	public static string Username { get; private set; }

	public static string GameAuthToken { get; private set; }

	public void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.name = "KongregateAPI";
		Application.ExternalEval("if(typeof(kongregateUnitySupport) != 'undefined'){\n        kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');\n      };");
	}

	public void OnKongregateAPILoaded(string userInfoString)
	{
		Debug.Log("Kong connected!");
		Connected = true;
		OnKongregateUserInfo(userInfoString);
	}

	public void OnKongregateUserInfo(string userInfoString)
	{
		string[] array = userInfoString.Split('|', StringSplitOptions.None);
		int num = Convert.ToInt32(array[0]);
		string text = array[1];
		_ = array[2];
		Debug.Log("Kongregate User Info: " + text + ", userId: " + num);
	}

	public static void Submit(string statisticName, int value)
	{
		if (Connected)
		{
			Application.ExternalCall("kongregate.stats.submit", statisticName, value);
		}
	}
}
