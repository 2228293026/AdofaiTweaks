using UnityEngine;

public class scrScaleToScreenSize : ADOBase
{
	private Camera mainCam;

	private void Awake()
	{
		mainCam = ADOBase.controller.camy.GetComponent<Camera>();
	}

	private void Start()
	{
		float num = 2f * mainCam.orthographicSize;
		float x = num * mainCam.aspect;
		Vector2 vector = new Vector2(x, num);
		base.transform.localScale = vector;
	}
}
