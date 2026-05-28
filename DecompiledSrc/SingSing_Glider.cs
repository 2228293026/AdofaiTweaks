using UnityEngine;

public class SingSing_Glider : Mawaru_Sprite
{
	public int dir;

	private bool updatedThisFrame;

	private new void Awake()
	{
		base.Awake();
		base.transform.eulerAngles = Vector3.forward * -90f * dir;
	}

	private new void LateUpdate()
	{
		if (!updatedThisFrame && curFrame == 0)
		{
			if (dir == 0)
			{
				base.transform.position += 0.25f * (Vector3.right * base.transform.localScale.x + Vector3.down * base.transform.localScale.y);
			}
			else if (dir == 1)
			{
				base.transform.position += 0.25f * (Vector3.left * base.transform.localScale.x + Vector3.down * base.transform.localScale.y);
			}
			else if (dir == 2)
			{
				base.transform.position += 0.25f * (Vector3.left * base.transform.localScale.x + Vector3.up * base.transform.localScale.y);
			}
			else if (dir == 3)
			{
				base.transform.position += 0.25f * (Vector3.right * base.transform.localScale.x + Vector3.up * base.transform.localScale.y);
			}
			updatedThisFrame = true;
		}
		if (curFrame != 0 && updatedThisFrame)
		{
			updatedThisFrame = false;
		}
	}
}
