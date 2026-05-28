using UnityEngine;

public class scrDisableIfOverallProgressStage : MonoBehaviour
{
	public int requiredStage;

	public bool inverted;

	private void Start()
	{
		if (GCS.FOOL_JOKER && requiredStage >= 7)
		{
			requiredStage = 5;
		}
		bool flag = Persistence.GetOverallProgressStage() < requiredStage;
		if (inverted)
		{
			flag = !flag;
		}
		if (flag)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
