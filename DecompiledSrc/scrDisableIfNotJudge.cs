using UnityEngine;

public class scrDisableIfNotJudge : MonoBehaviour
{
	private void Start()
	{
		if (!GCS.d_judges)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
