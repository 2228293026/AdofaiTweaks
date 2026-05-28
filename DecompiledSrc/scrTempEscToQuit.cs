using UnityEngine;

public class scrTempEscToQuit : ADOBase
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
		{
			Quit();
		}
	}

	private void Quit()
	{
		if (GCS.webVersion)
		{
			ADOBase.loader.LoadScene("scnIntro");
		}
	}
}
