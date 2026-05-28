using UnityEngine;

public class scrDisableIfPastProgressStage : MonoBehaviour
{
	public int requiredStage;

	private void Start()
	{
		if (Persistence.GetOverallProgressStage() >= requiredStage)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
