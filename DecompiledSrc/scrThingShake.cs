using UnityEngine;

public class scrThingShake : MonoBehaviour
{
	private float thingJitterTime;

	private int thingJitterAmountX = 20;

	private int thingJitterAmountY = 20;

	private float saveThingX;

	private float saveThingY;

	public float x
	{
		get
		{
			return base.transform.localPosition.x;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithX(value);
		}
	}

	public float y
	{
		get
		{
			return base.transform.localPosition.y;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithY(value);
		}
	}

	public void StopShake()
	{
		thingJitterTime = 0f;
		x = saveThingX;
		y = saveThingY;
	}

	public void activateThingJitter(int jitterCount, int jitterAmount)
	{
		if (thingJitterTime <= 0f)
		{
			thingJitterTime = (float)jitterCount / 60f;
			thingJitterAmountX = jitterAmount;
			thingJitterAmountY = jitterAmount;
			saveThingX = x;
			saveThingY = y;
		}
		else if (jitterAmount > thingJitterAmountX)
		{
			thingJitterTime = (float)jitterCount / 60f;
			thingJitterAmountX = jitterAmount;
			thingJitterAmountY = jitterAmount;
		}
	}

	public void activateThingJitter(float duration, int amountX, int amountY)
	{
		if (thingJitterTime <= 0f)
		{
			thingJitterTime = duration;
			thingJitterAmountX = amountX;
			thingJitterAmountY = amountY;
			saveThingX = x;
			saveThingY = y;
		}
		else if (amountX > thingJitterAmountX || amountY > thingJitterAmountY)
		{
			thingJitterTime = duration;
			thingJitterAmountX = amountX;
			thingJitterAmountY = amountY;
		}
	}

	private void Update()
	{
		if (!(thingJitterTime > 0f))
		{
			return;
		}
		thingJitterTime -= Time.unscaledDeltaTime;
		if (thingJitterTime <= 0f)
		{
			x = saveThingX;
			y = saveThingY;
			return;
		}
		float num = Random.value * (float)thingJitterAmountX;
		if ((double)Random.value < 0.5)
		{
			num = 0f - num;
		}
		float num2 = Random.value * (float)thingJitterAmountY;
		if ((double)Random.value < 0.5)
		{
			num2 = 0f - num2;
		}
		x = saveThingX + num;
		y = saveThingY + num2;
	}
}
