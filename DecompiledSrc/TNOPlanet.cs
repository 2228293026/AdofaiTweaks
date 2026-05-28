using UnityEngine;

public class TNOPlanet : MonoBehaviour
{
	[Header("Properties")]
	public float ringRotationSpeed;

	public float haloRotationSpeed;

	[Header("References")]
	public Transform outerRing;

	public Transform innerRing;

	public Transform outerHalo;

	public Transform middleHalo;

	public Transform innerHalo;

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		outerRing.Rotate(0f, 0f, ringRotationSpeed * deltaTime);
		innerRing.Rotate(0f, 0f, (0f - ringRotationSpeed) * deltaTime);
		outerHalo.Rotate(0f, 0f, haloRotationSpeed * deltaTime);
		middleHalo.Rotate(0f, 0f, (0f - haloRotationSpeed) * deltaTime);
		innerHalo.Rotate(0f, 0f, haloRotationSpeed * deltaTime);
	}
}
