using UnityEngine;

public class scrPlanetCopyCam : MonoBehaviour
{
	private Camera refCam;

	private Camera thisCam;

	public Vector2 position = Vector2.zero;

	public float scale = 1f;

	public float parallax = 1f;

	public float rotationOffset;

	private void Awake()
	{
		refCam = scrCamera.instance.GetComponent<Camera>();
		thisCam = GetComponent<Camera>();
	}

	public void Enable()
	{
		thisCam.enabled = true;
	}

	public void Disable()
	{
		thisCam.enabled = false;
	}

	private void LateUpdate()
	{
		float num = ((parallax == 0.99f || (double)parallax == 1.01) ? 1f : parallax);
		if (scale != 0f)
		{
			base.transform.position = (refCam.transform.position + new Vector3(0f - position.x, 0f - position.y, refCam.transform.position.z)) * num / scale;
		}
		base.transform.rotation = refCam.transform.rotation;
		base.transform.eulerAngles += Vector3.forward * rotationOffset;
		thisCam.orthographicSize = refCam.orthographicSize / scale;
		thisCam.depth = Mathf.Max(parallax, Mathf.Pow(10f, -6f)) - 2f;
	}
}
