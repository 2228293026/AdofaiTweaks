using UnityEngine;

public class BarRotate : MonoBehaviour
{
	public float mult = 1f;

	public float zoompower;

	private float ang;

	private void LateUpdate()
	{
		ang = NewLife.instance.camy.transform.localEulerAngles.z;
		if (ang > 180f)
		{
			ang -= 360f;
		}
		ang *= mult;
		base.transform.localEulerAngles = Vector3.forward * ang;
		base.transform.localScale = Vector3.up * 1f / Mathf.Pow(NewLife.instance.camy.zoomSize, zoompower) + Vector3.right * 1f / Mathf.Pow(NewLife.instance.camy.zoomSize, zoompower) + Vector3.forward;
	}
}
