using UnityEngine;

public class scrClearTaroStoryProg : MonoBehaviour
{
	public scrFloor floor;

	public GameObject doneText;

	private bool active = true;

	private void Update()
	{
		if (scrController.instance.currFloor == floor)
		{
			if (active)
			{
				active = false;
				doneText.SetActive(value: true);
				Persistence.ResetTaroStoryProgress();
			}
		}
		else if (!active)
		{
			active = true;
			doneText.SetActive(value: false);
		}
	}

	public void SetStoryProgTo5()
	{
		Persistence.taroStoryProgress = 5;
	}

	public void SetStoryProgTo7()
	{
		Persistence.taroStoryProgress = 7;
	}
}
