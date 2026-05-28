using UnityEngine;

public class scrBGCamNoRotate : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		scrMisc.Rotate2D(base.transform, 0f - base.transform.parent.transform.rotation.eulerAngles.z);
	}
}
