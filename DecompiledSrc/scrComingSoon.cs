using UnityEngine;

public class scrComingSoon : MonoBehaviour
{
	public float minX;

	public float maxX;

	private void Awake()
	{
	}

	private void LateUpdate()
	{
		float x = scrCamera.instance.transform.position.x;
		base.transform.position = new Vector3(Mathf.Clamp(x, minX, maxX), base.transform.position.y, base.transform.position.z);
	}
}
