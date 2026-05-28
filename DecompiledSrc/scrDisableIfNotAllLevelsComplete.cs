using UnityEngine;

public class scrDisableIfNotAllLevelsComplete : MonoBehaviour
{
	public bool disableIfAllLevelsComplete;

	private void Update()
	{
		if (GCS.maxLevel < 18 && !disableIfAllLevelsComplete)
		{
			base.gameObject.SetActive(value: false);
		}
		if (GCS.maxLevel >= 18 && disableIfAllLevelsComplete)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
