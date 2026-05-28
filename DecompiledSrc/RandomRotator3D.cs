using UnityEngine;

public class RandomRotator3D : MonoBehaviour
{
	public float deltaAngle = 3.5f;

	public float rotationSpeed = 1f;

	public float timeSpeed = 1f;

	private Vector3 direction;

	private static Vector3 randomNormalizedVector => new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

	private void Awake()
	{
		direction = randomNormalizedVector;
	}

	private void Update()
	{
		direction = Quaternion.AngleAxis(deltaAngle, randomNormalizedVector) * direction;
		base.transform.Rotate(direction * (rotationSpeed * timeSpeed));
	}
}
