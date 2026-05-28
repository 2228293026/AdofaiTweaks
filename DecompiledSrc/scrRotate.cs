using UnityEngine;

public class scrRotate : MonoBehaviour
{
	public float degreesPerSecond;

	private void Update()
	{
		base.transform.Rotate(0f, 0f, Time.deltaTime * degreesPerSecond);
	}
}
