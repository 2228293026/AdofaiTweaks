using UnityEngine;

public class scrDisableIfBooth : MonoBehaviour
{
	private void Start()
	{
		if (GCS.d_booth)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
