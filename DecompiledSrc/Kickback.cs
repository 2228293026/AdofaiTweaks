using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Utility/Super Game Feel Effects/Kickback", 1)]
public class Kickback : MonoBehaviour
{
	private Transform _t;

	private Coroutine c;

	private Vector3 lastMovement;

	public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f));

	public bool useLocalSpace;

	public float intensity = 0.5f;

	public float time = 0.1f;

	[Range(0f, 7f)]
	public int roundDecimals = 7;

	private Transform t
	{
		get
		{
			if (_t == null)
			{
				_t = base.transform;
			}
			return _t;
		}
	}

	private void OnValidate()
	{
		if (time < 0f)
		{
			time = 0f;
		}
	}

	public void Kick(Vector3 dir)
	{
		Kick(dir, 1f);
	}

	public void Kick(Vector3 dir, float multi)
	{
		if (c != null)
		{
			StopCoroutine(c);
		}
		c = StartCoroutine(DoKick(dir, multi));
	}

	private IEnumerator DoKick(Vector3 dir, float multi)
	{
		float timer = 0f;
		while (timer < time)
		{
			t.localPosition -= lastMovement;
			float num = curve.Evaluate(timer / time) * multi * intensity;
			if (useLocalSpace)
			{
				lastMovement = t.localRotation * dir.normalized * num;
			}
			else
			{
				lastMovement = dir.normalized * num;
			}
			if (roundDecimals != 7)
			{
				lastMovement.x = (float)Math.Round(lastMovement.x, roundDecimals);
				lastMovement.y = (float)Math.Round(lastMovement.y, roundDecimals);
				lastMovement.z = (float)Math.Round(lastMovement.z, roundDecimals);
			}
			t.localPosition += lastMovement;
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		t.localPosition -= lastMovement;
		lastMovement = Vector3.zero;
	}
}
