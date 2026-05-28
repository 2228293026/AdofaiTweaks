using UnityEngine;

public class scrCountdownZoom : MonoBehaviour
{
	private scrConductor cond;

	private scrCamera cam;

	private int counter = 99;

	private void Start()
	{
		cond = scrConductor.instance;
		cam = scrCamera.instance;
	}

	private void Update()
	{
		if (!GCS.lofiVersion)
		{
			if (cond.beatNumber < 4 && !GCS.DisableAllZooming && counter != cond.beatNumber)
			{
				float num = 5 * (5 - cond.beatNumber);
				cam.setCamSizeLerp(num + 1f, num, 1f);
				counter = cond.beatNumber;
			}
			if (cond.beatNumber == 4 && counter != cond.beatNumber)
			{
				counter = cond.beatNumber;
			}
		}
	}
}
