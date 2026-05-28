using UnityEngine;
using UnityEngine.UI;

public class scrDifficultyText : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Text>().color = Color.clear;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			GCS.HITMARGIN_COUNTED -= 3f;
			SaveDifficulty();
			GetComponent<Text>().color = Color.blue;
			GetComponent<Text>().text = GCS.HITMARGIN_COUNTED.ToString();
			Debug.Log(GCS.HITMARGIN_COUNTED.ToString());
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			GCS.HITMARGIN_COUNTED += 3f;
			SaveDifficulty();
			GetComponent<Text>().color = Color.blue;
			GetComponent<Text>().text = GCS.HITMARGIN_COUNTED.ToString();
			Debug.Log(GCS.HITMARGIN_COUNTED.ToString());
		}
	}

	private void SaveDifficulty()
	{
		PlayerPrefs.SetFloat("difficulty", GCS.HITMARGIN_COUNTED);
		PlayerPrefs.Save();
	}
}
