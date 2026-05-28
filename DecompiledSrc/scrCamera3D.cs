using UnityEngine;

public class scrCamera3D : ADOBase
{
	public float speed = 0.005f;

	private void Update()
	{
		base.transform.position = Vector3.Slerp(base.transform.position, scrController.instance.chosenPlanet.transform.position, speed);
	}
}
