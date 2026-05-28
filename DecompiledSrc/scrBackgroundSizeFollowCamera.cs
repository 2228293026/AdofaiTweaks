using UnityEngine;

public class scrBackgroundSizeFollowCamera : ADOBase
{
	public float startObjScale = 1.1f;

	public float startCamScale;

	private scrCamera camscr;

	private void Start()
	{
		camscr = ADOBase.controller.camy.GetComponent<scrCamera>();
		startCamScale = camscr.camsizenormal;
	}

	private void Update()
	{
		float orthographicSize = camscr.camobj.orthographicSize;
		base.transform.localScale = new Vector3(startObjScale * orthographicSize / startCamScale, startObjScale * orthographicSize / startCamScale, 1f);
	}
}
