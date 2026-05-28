using DG.Tweening;
using UnityEngine;

public class LegacyRandomRotator3D : MonoBehaviour
{
	[Header("Rotation Weights")]
	public float xRotationWeight = 0.5f;

	public float yRotationWeight = 0.5f;

	public float zRotationWeight = 0.5f;

	[Header("Rotation Time Options")]
	public float speed = 1f;

	public float tweenAverageTime = 2f;

	public float tweenTimeDeviation;

	[Header("Misc")]
	public Ease[] easeSequences;

	private Tween tween;

	private (float, float) xRotationRange;

	private (float, float) yRotationRange;

	private (float, float) zRotationRange;

	private (float, float) tweenTimeRange;

	private int lastSelectedEaseIndex = -1;

	private Vector3 randomVector => new Vector3(Random.Range(xRotationRange.Item1, xRotationRange.Item2), Random.Range(yRotationRange.Item1, yRotationRange.Item2), Random.Range(zRotationRange.Item1, zRotationRange.Item2));

	private float randomTime => Random.Range(tweenTimeRange.Item1, tweenTimeRange.Item2);

	private Ease nextEase
	{
		get
		{
			if (easeSequences.Length == 0)
			{
				return Ease.Linear;
			}
			return easeSequences[lastSelectedEaseIndex = ++lastSelectedEaseIndex % easeSequences.Length];
		}
	}

	public void UpdateOptionChanges()
	{
		xRotationRange = (-360f * xRotationWeight, 360f * xRotationWeight);
		yRotationRange = (-360f * yRotationWeight, 360f * yRotationWeight);
		zRotationRange = (-360f * zRotationWeight, 360f * zRotationWeight);
		tweenTimeRange = (tweenAverageTime - tweenTimeDeviation, tweenAverageTime + tweenTimeDeviation);
	}

	public void OverrideTween()
	{
		EndTween();
		tween = base.transform.DORotate(randomVector, randomTime / speed).SetEase(nextEase).OnComplete(OverrideTween);
	}

	public void EndTween()
	{
		if (tween != null)
		{
			tween.Kill(complete: true);
			tween = null;
		}
	}

	private void Start()
	{
		UpdateOptionChanges();
		OverrideTween();
	}
}
