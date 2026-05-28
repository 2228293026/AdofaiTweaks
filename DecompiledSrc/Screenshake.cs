using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Utility/Super Game Feel Effects/Screenshake", 1)]
public class Screenshake : MonoBehaviour
{
	private Transform _t;

	private Coroutine c;

	private Vector3 lastMovement = Vector3.zero;

	public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f));

	public bool useLocalSpace = true;

	public Vector3 strength = Vector2.one;

	public float intensity = 1f;

	public float time = 0.2f;

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

	public void Shake()
	{
		Shake(1f);
	}

	public void Shake(float multi)
	{
		if (c != null)
		{
			StopCoroutine(c);
		}
		c = StartCoroutine(DoShake(multi));
	}

	private IEnumerator DoShake(float multi)
	{
		float timer = 0f;
		while (timer < time)
		{
			t.localPosition -= lastMovement;
			Vector3 vector = new Vector3(curve.Evaluate(timer / time) * multi * intensity * strength.x, curve.Evaluate(timer / time) * multi * intensity * strength.y, curve.Evaluate(timer / time) * multi * intensity * strength.z);
			if (useLocalSpace)
			{
				lastMovement = t.localRotation * new Vector3(UnityEngine.Random.Range(0f - vector.x, vector.x), UnityEngine.Random.Range(0f - vector.y, vector.y), UnityEngine.Random.Range(0f - vector.z, vector.z));
			}
			else
			{
				lastMovement = new Vector3(UnityEngine.Random.Range(0f - vector.x, vector.x), UnityEngine.Random.Range(0f - vector.y, vector.y), UnityEngine.Random.Range(0f - vector.z, vector.z));
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
