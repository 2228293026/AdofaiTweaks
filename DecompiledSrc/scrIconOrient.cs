using UnityEngine;

public class scrIconOrient : ADOBase
{
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = ADOBase.controller.camy.camobj;
	}

	private void Update()
	{
		scrMisc.RotateWorld2DCW(base.transform, 0f);
	}
}
