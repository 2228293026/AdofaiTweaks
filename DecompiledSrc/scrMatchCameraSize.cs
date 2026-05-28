using UnityEngine;

public class scrMatchCameraSize : MonoBehaviour
{
	private Camera myCamera;

	private void Awake()
	{
		myCamera = GetComponent<Camera>();
	}

	private void Update()
	{
		scrCamera instance = scrCamera.instance;
		if (instance != null)
		{
			myCamera.orthographicSize = instance.camobj.orthographicSize;
		}
	}
}
