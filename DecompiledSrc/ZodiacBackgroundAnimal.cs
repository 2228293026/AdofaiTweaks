using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ZodiacBackgroundAnimal : MonoBehaviour
{
	public float jumpHeight;

	public float jumpDuration;

	public float minJumpDuration;

	public float jumpSteepness;

	public Image image;

	public Vector2 realPosition;

	private Vector2 direction;

	private float yKillPosition;

	private Vector2 initialPos;

	private float startTime;

	private float jumpModifier;

	private List<float> jumpTimes = new List<float>();

	private Tween colorAnimation;

	public void Setup(Sprite sprite, Vector2 moveDirection, float killY)
	{
		image.sprite = sprite;
		direction = moveDirection;
		realPosition = base.transform.localPosition;
		yKillPosition = killY;
		initialPos = base.transform.localPosition;
		startTime = Time.unscaledTime;
	}

	public void ChangeColor(Color color, float duration)
	{
		if (base.transform.localPosition.y < yKillPosition)
		{
			colorAnimation.Kill();
			colorAnimation = image.DOColor(color, duration).SetEase(Ease.InOutQuad).SetUpdate(isIndependentUpdate: true);
		}
	}

	private void Update()
	{
		float num = jumpDuration;
		if (jumpTimes.Count >= 2)
		{
			num = Mathf.Min(jumpDuration, jumpTimes[1] - jumpTimes[0]);
			num = Mathf.Max(minJumpDuration, num);
		}
		if (jumpTimes.Count > 0 && Time.unscaledTime < jumpTimes[0] + num && Time.unscaledTime >= jumpTimes[0])
		{
			float num2 = (Time.unscaledTime - jumpTimes[0]) / num;
			jumpModifier = jumpHeight * Mathf.Atan(jumpSteepness * Mathf.Sin(num2 * 180f * ((float)Math.PI / 180f))) / Mathf.Atan(jumpSteepness);
		}
		else if (jumpTimes.Count > 0 && Time.unscaledTime >= jumpTimes[0] + num)
		{
			jumpModifier = 0f;
			int num3 = 0;
			for (int i = 0; i < jumpTimes.Count && !(jumpTimes[i] > Time.unscaledTime); i++)
			{
				num3++;
			}
			num3 = Mathf.Max(1, num3 - 1);
			jumpTimes.RemoveRange(0, num3);
		}
		realPosition = initialPos + (Time.unscaledTime - startTime) * direction;
		base.transform.localPosition = realPosition + new Vector2(0f, jumpModifier);
		if (base.transform.localPosition.y >= yKillPosition * 1.5f)
		{
			ZodiacBackground.animals.Remove(this);
			DestroySelf();
		}
	}

	public void DestroySelf()
	{
		colorAnimation.Kill();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Jump(float delay)
	{
		if (!(delay >= 0f))
		{
			return;
		}
		float num = Time.unscaledTime + delay;
		bool flag = false;
		for (int i = 0; i < jumpTimes.Count; i++)
		{
			if (!flag && jumpTimes[i] >= num)
			{
				jumpTimes.Insert(i, num);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			jumpTimes.Add(num);
		}
	}
}
