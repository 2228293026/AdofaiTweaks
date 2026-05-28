using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EndingSparkle : MonoBehaviour
{
	public List<SpriteRenderer> tris;

	public List<Transform> triPs;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Color alpha = new Color(0f, 0f, 0f, 1f);

	private List<float> speeds = new List<float>();

	private List<float> dirs = new List<float>();

	private List<float> rotation = new List<float>();

	private List<float> extraKick = new List<float>();

	private LineRenderer endCircle;

	private float circThick;

	private float circRad = 0.1f;

	private float circAlp = 1f;

	private float alp;

	private float alpFlicker = 1f;

	private Vector3 scale1 = new Vector3(1f, 1f, 1f);

	private Vector3 scale2 = new Vector3(3f, 4f, 1f);

	private Vector3 scale3 = new Vector3(4f, 10f, 1f);

	private Vector3 scale4 = new Vector3(0f, 20f, 1f);

	private int circSegments = 50;

	private float ang;

	private Color circYellow = new Color(1f, 1f, 0.8f, 0f);

	private void Start()
	{
		foreach (SpriteRenderer tri in tris)
		{
			tri.DOColor(whiteClear, 0f);
			alp = 0f;
			circAlp = 0f;
			dirs.Add((UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1 : (-1));
			speeds.Add(UnityEngine.Random.Range(50f, 80f));
			rotation.Add(UnityEngine.Random.Range(0f, 360f));
			extraKick.Add(0f);
		}
		CreateEndCircle();
	}

	public void Proc()
	{
		for (int i = 0; i < tris.Count; i++)
		{
			int j = i;
			DOTween.Sequence().Append(DOTween.To(() => extraKick[j], delegate(float x)
			{
				extraKick[j] = x;
			}, 0f, 0f).SetEase(Ease.OutExpo)).Append(DOTween.To(() => extraKick[j], delegate(float x)
			{
				extraKick[j] = x;
			}, 180f, 0.9f).SetEase(Ease.OutCubic))
				.AppendInterval(0.8f)
				.Append(DOTween.To(() => extraKick[j], delegate(float x)
				{
					extraKick[j] = x;
				}, 360f, 0.9f).SetEase(Ease.InCubic));
			DOTween.Sequence().Append(DOTween.To(() => alp, delegate(float x)
			{
				alp = x;
			}, 0f, 0f).SetEase(Ease.Linear)).Append(DOTween.To(() => alp, delegate(float x)
			{
				alp = x;
			}, 1f, 0.8f).SetEase(Ease.Linear))
				.AppendInterval(1f)
				.Append(DOTween.To(() => alp, delegate(float x)
				{
					alp = x;
				}, 0f, 0.8f).SetEase(Ease.Linear));
			DOTween.Sequence().Append(triPs[j].DOScale(scale1, 0f).SetEase(Ease.OutQuad)).Append(triPs[j].DOScale(scale2, 0.6f).SetEase(Ease.Linear))
				.Append(triPs[j].DOScale(scale3, 1.4f).SetEase(Ease.Linear))
				.Append(triPs[j].DOScale(scale4, 0.6f).SetEase(Ease.InCubic));
		}
		DOTween.Sequence().Append(DOTween.To(() => alp, delegate(float x)
		{
			alp = x;
		}, 0f, 0f).SetEase(Ease.Linear)).Append(DOTween.To(() => alp, delegate(float x)
		{
			alp = x;
		}, 1f, 0.8f).SetEase(Ease.Linear))
			.AppendInterval(1f)
			.Append(DOTween.To(() => alp, delegate(float x)
			{
				alp = x;
			}, 0f, 0.8f).SetEase(Ease.Linear));
		DOTween.Sequence().Append(DOTween.To(() => circThick, delegate(float x)
		{
			circThick = x;
		}, 0f, 0f).SetEase(Ease.Linear)).Append(DOTween.To(() => circThick, delegate(float x)
		{
			circThick = x;
		}, 3f, 1f).SetEase(Ease.Linear))
			.Append(DOTween.To(() => circThick, delegate(float x)
			{
				circThick = x;
			}, 0f, 1f).SetEase(Ease.OutExpo));
		DOTween.Sequence().Append(DOTween.To(() => circRad, delegate(float x)
		{
			circRad = x;
		}, 0f, 0f).SetEase(Ease.Linear)).Append(DOTween.To(() => circRad, delegate(float x)
		{
			circRad = x;
		}, 3f, 1f).SetEase(Ease.Linear))
			.Append(DOTween.To(() => circRad, delegate(float x)
			{
				circRad = x;
			}, 6f, 1f).SetEase(Ease.OutExpo));
		DOTween.Sequence().Append(DOTween.To(() => circAlp, delegate(float x)
		{
			circAlp = x;
		}, 0f, 0f).SetEase(Ease.Linear)).Append(DOTween.To(() => circAlp, delegate(float x)
		{
			circAlp = x;
		}, 1f, 0.5f).SetEase(Ease.OutExpo))
			.Append(DOTween.To(() => circAlp, delegate(float x)
			{
				circAlp = x;
			}, 0f, 1.5f).SetEase(Ease.OutExpo));
	}

	private void CreateEndCircle()
	{
		endCircle = base.gameObject.AddComponent<LineRenderer>();
		endCircle.material = new Material(Shader.Find("Sprites/Default"));
		endCircle.positionCount = circSegments + 1;
		endCircle.sortingOrder = 150;
		endCircle.startWidth = 0f;
		endCircle.endWidth = 0f;
		endCircle.enabled = false;
	}

	private void DrawEndCircle(Color col, float radius, float thickness)
	{
		endCircle.startWidth = thickness;
		endCircle.endWidth = thickness;
		endCircle.enabled = true;
		endCircle.startColor = col;
		endCircle.endColor = col;
		for (int i = 0; i < circSegments + 1; i++)
		{
			ang = (float)i / (float)circSegments * (float)Math.PI * 2f - (float)Math.PI / 2f;
			endCircle.SetPosition(i, base.transform.position + radius * Vector3.up * Mathf.Sin(ang) + radius * Vector3.right * Mathf.Cos(ang));
		}
	}

	private void Update()
	{
		if (circThick > 0f && circAlp > 0f)
		{
			DrawEndCircle(circYellow + alpha * circAlp, circRad, circThick);
		}
		else
		{
			endCircle.enabled = false;
		}
		alpFlicker = 0.8f + 0.07f * Mathf.Sin((float)Math.PI * Time.time * 20f);
		for (int i = 0; i < tris.Count; i++)
		{
			rotation[i] += Time.deltaTime * dirs[i] * speeds[i];
			triPs[i].localEulerAngles = Vector3.forward * (rotation[i] + extraKick[i] * dirs[i]);
			tris[i].DOColor(whiteClear + alpha * (alp * alpFlicker), 0f);
		}
	}
}
