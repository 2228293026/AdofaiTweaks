using System;
using DG.Tweening;
using UnityEngine;

public class Mawaru_Oskari : MonoBehaviour
{
	public GameObject handContainer;

	public GameObject hands;

	public Mawaru_Sprite portal;

	public Mawaru_Sprite body;

	private bool hidden;

	private Vector3 startPos;

	private float beat => (float)TaroBGScript.instance.songBeat;

	private float speed => TaroBGScript.instance.speed;

	private void Start()
	{
		body.SetState(15);
		handContainer.transform.localScale = Vector3.zero;
		handContainer.transform.localPosition = new Vector3(0.08f, -1f, 0f);
		portal.transform.localScale = Vector3.zero;
		base.transform.position += Vector3.forward * -1.5f;
		startPos = base.transform.position;
	}

	public void Rise()
	{
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(portal.transform.DOScale(Vector3.one * 0.15f, 0f))
			.Append(portal.transform.DOScale(new Vector3(1.2f, 0.15f, 1f), 0.2f * speed).SetEase(Ease.OutExpo))
			.Append(portal.transform.DOScale(Vector3.one * 1.2f, 0.3f * speed).SetEase(Ease.InOutCubic).OnComplete(delegate
			{
				body.SetState(0);
				body.lastFrame = 7;
			}));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.9f * speed)
			.Append(handContainer.transform.DOLocalMove(new Vector3(0.08f, -0.35f, 0f), 0.5f * speed).SetEase(Ease.InOutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.9f * speed)
			.Append(handContainer.transform.DOScale(Vector3.one * 0.2f, 0f))
			.Append(handContainer.transform.DOScale(Vector3.one, 0.5f * speed).SetEase(Ease.InOutCubic));
	}

	public void Fall()
	{
		if (!hidden)
		{
			hidden = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DOLocalMove(new Vector3(0.08f, -1f, 0f), 0.5f * speed).SetEase(Ease.InOutCubic).OnComplete(delegate
			{
				body.SetState(8);
				body.lastFrame = 15;
			}));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DOScale(Vector3.zero, 0.5f * speed).SetEase(Ease.InOutCubic));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.6f * speed)
				.Append(portal.transform.DOScale(new Vector3(1.2f, 0f, 1f), 0.2f * speed).SetEase(Ease.InBack).OnComplete(delegate
				{
					portal.render.enabled = false;
				}));
		}
	}

	private void Sad()
	{
		hidden = true;
		body.SetState(16);
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(body.transform.DOScale(Vector3.one * 1.2f, 0f))
			.Append(body.transform.DOScale(Vector3.one * 1f, 0.3f * speed));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DOLocalMove(new Vector3(0.12f, -0.5f, 0f), 0.5f * speed).SetEase(Ease.OutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(handContainer.transform.DORotate(Vector3.forward * 20f, 0.5f * speed).SetEase(Ease.OutCubic));
	}

	private float oc(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t = t / d - 1f;
		return c * (Mathf.Pow(t, 3f) + 1f) + b;
	}

	public void Update()
	{
		float t = beat / 1f - Mathf.Floor(beat / 1f);
		float num = oc(t);
		if (beat % 2f < 1f)
		{
			num = 1f - num;
		}
		if (body.curFrame == 7 && (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2))
		{
			Sad();
			hidden = true;
		}
		if (ADOBase.controller.currentState == States.PlayerControl)
		{
			hands.transform.localEulerAngles = Vector3.forward * (num * 10f - 5f);
		}
		base.transform.position = startPos + Vector3.up * 0.05f * Mathf.Sin(beat * (float)Math.PI * 0.5f);
	}
}
