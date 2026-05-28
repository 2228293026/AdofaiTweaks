using System.Collections;
using Newgrounds;
using UnityEngine;

public class scrNewgroundsAPIManager : MonoBehaviour
{
	private API m_API;

	public static scrNewgroundsAPIManager instance;

	public static bool started;

	public void Start()
	{
		if (!GCS.lofiVersion)
		{
			StartCoroutine(ConnectWithDelay());
		}
	}

	public static void StaticCheckMedals()
	{
		if (GCS.lofiVersion)
		{
			if (instance != null)
			{
				instance.CheckMedals();
				API.m_output += "\nWE ARE CHECKINNNNN";
			}
			else
			{
				API.m_output += "\nINSTANCE NOT FOUND";
			}
		}
	}

	public IEnumerator ConnectWithDelay()
	{
		for (float timer = 1f; timer >= 0f; timer -= Time.deltaTime)
		{
			yield return 0;
		}
		API.m_output = "\nlooking for API..";
		GameObject gameObject = GameObject.FindGameObjectWithTag("NewgroundsAPI");
		if (gameObject != null)
		{
			m_API = gameObject.GetComponent<API>();
			instance = this;
			API.m_output += "..found!";
			StartCoroutine(m_API.Connect());
			StartCoroutine(m_API.GetMedals());
		}
	}

	public void CheckMedals()
	{
		Debug.Log("uncomment the following lines of code");
	}

	private void OnGUI()
	{
		_ = RDC.debug;
	}
}
