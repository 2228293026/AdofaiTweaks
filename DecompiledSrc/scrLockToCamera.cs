using UnityEngine;

public class scrLockToCamera : ADOBase
{
	public bool lockPos = true;

	public bool lockRot;

	public bool lockScale = true;

	public bool ignoreScaleToScreenSizeComponent;

	public Vector2 offset = Vector2.zero;

	public Vector2 scaleMultiplier = Vector2.one;

	private Camera mainCam;

	private Vector3 currentScale;

	private void Awake()
	{
		mainCam = ADOBase.controller.camy.GetComponent<Camera>();
		currentScale = base.transform.localScale;
	}

	private void LateUpdate()
	{
		if (lockPos)
		{
			Vector3 position = mainCam.transform.position;
			base.transform.position = new Vector3(position.x + offset.x, position.y + offset.y, base.transform.position.z);
		}
		if (lockRot)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, mainCam.transform.rotation.eulerAngles.z);
		}
		if (lockScale)
		{
			float num = mainCam.orthographicSize / 2f;
			base.transform.localScale = scaleMultiplier * (num / 6f);
			currentScale = base.transform.localScale;
		}
		if (ignoreScaleToScreenSizeComponent)
		{
			float num2 = 2f * mainCam.orthographicSize;
			float num3 = num2 * mainCam.aspect;
			Vector2 vector = new Vector2(1f / num3, 1f / num2);
			base.transform.localScale = Vector3.Scale(currentScale, vector);
		}
	}
}
