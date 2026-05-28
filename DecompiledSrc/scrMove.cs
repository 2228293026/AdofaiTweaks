using UnityEngine;

public class scrMove : MonoBehaviour
{
	public Vector2 velocity;

	public bool waitTillSongStarts;

	public float delay;

	public bool decelerateOnFail;

	private double timeStart;

	private bool moving;

	private float velocityMultiplier = 1f;

	private void Update()
	{
		if (!moving)
		{
			if (waitTillSongStarts)
			{
				if (scrConductor.instance.hasSongStarted)
				{
					timeStart = Time.timeSinceLevelLoad;
					moving = true;
				}
			}
			else
			{
				timeStart = Time.timeSinceLevelLoad;
				moving = true;
			}
		}
		float num = scrConductor.instance?.song?.pitch ?? 1f;
		bool flag = (double)Time.timeSinceLevelLoad > timeStart + (double)delay;
		if (moving && flag)
		{
			Vector3 vector = new Vector3(velocity.x * Time.deltaTime * num, velocity.y * Time.deltaTime * num, 0f);
			base.transform.Translate(vector * velocityMultiplier);
		}
		if (!decelerateOnFail)
		{
			return;
		}
		States state = scrController.instance.state;
		if (state == States.Fail2 || state == States.Fail)
		{
			velocityMultiplier = Mathf.Clamp(velocityMultiplier - 1.5f * Time.deltaTime * num, -1f, 1f);
			scrSpriteAnimator component = GetComponent<scrSpriteAnimator>();
			if (component != null)
			{
				component.enabled = !(velocityMultiplier < 0.75f) || !(velocityMultiplier > 0f);
			}
		}
	}
}
