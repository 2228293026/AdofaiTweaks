using UnityEngine;

public class SSTSampleMovement : MonoBehaviour
{
	private Transform t;

	private Vector3 lastFrame;

	public SpriteRenderer spriteRenderer;

	public bool movementIs3D;

	public float moveSpeed = 1f;

	private void Start()
	{
		t = base.transform;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			t.position += new Vector3((0f - moveSpeed) * Time.deltaTime, 0f, 0f);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			t.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			if (movementIs3D)
			{
				t.position += new Vector3(0f, 0f, moveSpeed * Time.deltaTime);
			}
			else
			{
				t.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);
			}
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			if (movementIs3D)
			{
				t.position += new Vector3(0f, 0f, (0f - moveSpeed) * Time.deltaTime);
			}
			else
			{
				t.position += new Vector3(0f, (0f - moveSpeed) * Time.deltaTime, 0f);
			}
		}
		if (t.position.x < lastFrame.x)
		{
			spriteRenderer.flipX = true;
		}
		if (t.position.x > lastFrame.x)
		{
			spriteRenderer.flipX = false;
		}
		lastFrame = t.position;
	}
}
