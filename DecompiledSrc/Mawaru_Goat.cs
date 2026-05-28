using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Mawaru_Goat : MonoBehaviour
{
	public Mawaru_Sprite goatHead;

	public Mawaru_Sprite goatEyeLight;

	public GameObject beamTop;

	public Mawaru_Sprite beamPrepare;

	public List<GameObject> beamParts;

	public List<Vector3> beamPartScale;

	private float beamWidth;

	public Vector3 initpos;

	public bool fired;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private float pulse;

	private Vector3 scatter = Vector3.zero;

	private void Awake()
	{
		initpos = base.transform.position;
		beamPrepare.render.DOColor(whiteClear, 0f);
		for (int i = 0; i < beamParts.Count; i++)
		{
			beamPartScale.Add(beamParts[i].transform.localScale);
		}
	}

	public void Enable()
	{
		goatHead.render.enabled = true;
		goatEyeLight.render.enabled = true;
		goatEyeLight.transform.localScale = new Vector3(1f, 0f, 1f);
	}

	public void Fire()
	{
		fired = true;
		beamPrepare.render.DOColor(Color.white, 0f);
		DOTween.Sequence().Append(DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 1.6f, 0f)).Append(DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 1f, 0.4f).SetEase(Ease.OutSine).OnComplete(delegate
		{
			End(1f);
		}));
	}

	public void End(float endtime)
	{
		beamPrepare.render.DOColor(whiteClear, endtime);
		DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 0f, endtime * 0.25f).SetEase(Ease.OutExpo);
	}

	private void Update()
	{
		pulse = 0.06f * Mathf.Sin(Time.time * 8f * (float)Math.PI * 2f);
		for (int i = 0; i < beamParts.Count; i++)
		{
			beamParts[i].transform.localScale = Vector3.right * beamWidth * beamPartScale[i].x + Vector3.right * pulse * beamWidth + Vector3.up * beamPartScale[i].y + Vector3.forward * beamPartScale[i].z;
		}
		beamTop.transform.localScale = Vector3.right * beamWidth * 1.2f + Vector3.up * (0.5f + beamWidth * 0.5f) * 1.2f + Vector3.forward * 1.2f;
	}
}
