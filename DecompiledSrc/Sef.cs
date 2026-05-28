using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Sef : MonoBehaviour
{
	public GameObject container;

	public Mawaru_Sprite actor;

	public List<CharlieAction> queue = new List<CharlieAction>();

	public Tween curTween;

	public List<Transform> waypoints;

	public string curAnim = "idle";

	public float timer;

	public float waitTime;

	public float upTime;

	public float defaultScale = 0.75f;

	public float speed => TaroBGScript.instance.speed;

	private void Awake()
	{
		SetAnim("idle");
		actor.transform.localScale = new Vector3(2f * defaultScale, 0f, 0f);
	}

	public void AddEntry(CharlieAction a)
	{
		a.fTime = timer + waitTime;
		waitTime += a.duration;
		queue.Add(a);
	}

	public void SetAnim(string anim)
	{
		curAnim = anim;
		switch (anim)
		{
		case "idle":
			actor.animate = true;
			actor.loop = true;
			actor.SetState(0);
			actor.firstFrame = 0;
			actor.lastFrame = 2;
			break;
		case "curl":
			actor.animate = true;
			actor.loop = false;
			actor.SetState(3);
			actor.firstFrame = 3;
			actor.lastFrame = 6;
			break;
		case "uncurl":
			actor.animate = true;
			actor.loop = false;
			actor.SetState(7);
			actor.firstFrame = 7;
			actor.lastFrame = 11;
			break;
		}
	}

	public void RunAction(CharlieAction a)
	{
		container.transform.DOLocalMove(a.location, a.duration * speed).SetEase(Ease.Linear);
		if (a.animName == "idle")
		{
			SetAnim("idle");
		}
		else if (a.animName == "curl")
		{
			SetAnim("curl");
		}
		else if (a.animName == "uncurl")
		{
			SetAnim("uncurl");
		}
		else if (a.animName == "spawn")
		{
			actor.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(actor.transform.DOScale(Vector3.one * defaultScale, 0.3f).SetEase(Ease.OutBack));
			SetAnim("idle");
		}
		else if (a.animName == "hide")
		{
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(actor.transform.DOScale(new Vector3(2f * defaultScale, 0f, 0f), 0.3f).SetEase(Ease.InBack).OnComplete(delegate
			{
				actor.render.enabled = false;
			}));
		}
	}

	public void ClearQueue()
	{
		queue.Clear();
		waitTime = 0f;
	}

	public void RemoveActionsWithLabel(string label)
	{
		for (int num = queue.Count - 1; num >= 0; num--)
		{
			if (label != "" && queue[num].label == label)
			{
				waitTime -= queue[num].duration;
				queue.RemoveAt(num);
			}
		}
	}

	public void Update()
	{
		upTime += Time.deltaTime / speed;
		actor.transform.localPosition = Vector3.up * 0.05f * Mathf.Sin(upTime / 4f * 2f * (float)Math.PI);
		timer += Time.deltaTime / speed;
		while (queue.Count > 0 && timer >= queue[0].fTime)
		{
			RunAction(queue[0]);
			queue.RemoveAt(0);
		}
		if (waitTime > 0f)
		{
			waitTime -= Time.deltaTime / speed;
		}
		else
		{
			waitTime = 0f;
		}
	}
}
