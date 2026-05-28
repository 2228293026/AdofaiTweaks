using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Mawaru_Charlie : MonoBehaviour
{
	public GameObject container;

	public Mawaru_Sprite actor;

	public List<CharlieAction> queue = new List<CharlieAction>();

	public Tween curTween;

	public List<Transform> waypoints;

	public string curAnim = "idle";

	public float timer;

	public float waitTime;

	public bool lastSection;

	public float speed => TaroBGScript.instance.speed;

	private void Awake()
	{
		SetAnim("idle");
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
			actor.animate = false;
			actor.SetState(0);
			break;
		case "run":
			actor.animate = true;
			actor.SetState(1);
			actor.firstFrame = 2;
			actor.lastFrame = 9;
			break;
		case "jump":
			actor.animate = true;
			actor.SetState(11);
			actor.firstFrame = 14;
			actor.lastFrame = 15;
			break;
		case "land":
			actor.animate = false;
			actor.SetState(0);
			break;
		case "dive":
			actor.animate = false;
			actor.SetState(16);
			break;
		case "flop":
			actor.animate = true;
			actor.SetState(17);
			actor.firstFrame = 19;
			actor.lastFrame = 22;
			break;
		case "victory":
			actor.animate = false;
			actor.SetState(23);
			break;
		case "talk":
			actor.animate = false;
			actor.SetState(24);
			break;
		}
	}

	public void RunAction(CharlieAction a)
	{
		if (a.animName != "wait")
		{
			actor.transform.localScale = Vector3.right * -0.8f * a.facingDir + Vector3.up * 0.8f + Vector3.forward;
		}
		if (a.animName == "hide")
		{
			actor.render.enabled = false;
		}
		else
		{
			actor.render.enabled = true;
		}
		if (a.animName == "idle")
		{
			SetAnim("idle");
		}
		else if (a.animName == "jump")
		{
			SetAnim("jump");
			container.transform.DOScale(Vector3.one, 0f);
			container.transform.DOLocalMove(a.location, (a.duration - 0.2f) * speed).SetEase(Ease.Linear).OnComplete(delegate
			{
				SetAnim("land");
			});
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(actor.transform.DOLocalMoveY(a.jumpHeight, (a.duration * 0.5f - 0.1f) * speed).SetRelative(isRelative: true).SetEase(Ease.OutQuad))
				.Append(actor.transform.DOLocalMoveY(0f - a.jumpHeight, (a.duration * 0.5f - 0.1f) * speed).SetRelative(isRelative: true).SetEase(Ease.InQuad))
				.Append(container.transform.DOScale(new Vector3(1.3f, 0.7f, 1f), 0f))
				.Append(container.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.OutQuad));
		}
		else if (a.animName == "run")
		{
			container.transform.DOScale(Vector3.one, 0f);
			container.transform.DOLocalMove(a.location, a.duration * speed).SetEase(Ease.Linear);
			SetAnim("run");
		}
		else if (a.animName == "dive")
		{
			container.transform.DOLocalMove(a.location, a.duration * speed).SetEase(Ease.OutQuad);
			SetAnim("dive");
		}
		else if (a.animName == "flop")
		{
			container.transform.DOLocalMove(a.location, 0f).SetEase(Ease.Linear);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(container.transform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0f))
				.Append(container.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.OutQuad));
			SetAnim("flop");
		}
		else if (a.animName == "warp")
		{
			container.transform.DOLocalMove(a.location, 0f).SetEase(Ease.Linear);
			SetAnim("idle");
		}
		else if (a.animName == "victory")
		{
			container.transform.DOLocalMove(a.location, 0f).SetEase(Ease.Linear);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(container.transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0f))
				.Append(container.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.OutQuad));
			SetAnim("victory");
		}
		else if (a.animName == "talk")
		{
			container.transform.DOLocalMove(a.location, 0f).SetEase(Ease.Linear);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(container.transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0f))
				.Append(container.transform.DOScale(Vector3.one, 0.2f * speed).SetEase(Ease.OutQuad));
			SetAnim("talk");
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
		if (!lastSection || (lastSection && scrController.instance.currentState != States.Checkpoint && scrController.instance.currentState != States.Start))
		{
			if (curAnim == "idle" || curAnim == "victory")
			{
				container.transform.localScale = Vector3.up * (1f + 0.02f * Mathf.Sin(Time.time * 3.14159f)) + Vector3.right * (1f + 0.013f * Mathf.Cos(Time.time * 3.14159f)) + Vector3.forward;
			}
			if (curAnim == "talk")
			{
				container.transform.localScale = Vector3.up * (1f + 0.01f * Mathf.Sin(Time.time * 3.14159f)) + Vector3.right * (1f + 0.008f * Mathf.Cos(Time.time * 3.14159f)) + Vector3.forward;
			}
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
}
