using UnityEngine;

public class scrSaveLoader : ADOBase
{
	private void Start()
	{
	}

	private void ClearData()
	{
		MonoBehaviour.print("cleared data");
		GCS.maxLevel = 0;
		PlayerPrefs.SetInt("maxlevel", 0);
		PlayerPrefs.Save();
		ADOBase.loader.LoadScene("scnIntro");
	}
}
