using UnityEngine;

public class KillSpriteAfterTime : MonoBehaviour
{
	private float timer;

	public float aliveTime;

	public bool fadeAway;

	public AnimationCurve fadeCurve;

	public SpriteRenderer sr;

	private void Update()
	{
		if (aliveTime > 0f)
		{
			if (timer >= aliveTime)
			{
				Object.Destroy(base.gameObject);
			}
			if (fadeAway)
			{
				float a = fadeCurve.Evaluate(timer / aliveTime);
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
			}
			timer += Time.deltaTime;
		}
	}
}
