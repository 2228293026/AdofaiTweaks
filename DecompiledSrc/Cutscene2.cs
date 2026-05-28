using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cutscene2 : TaroCutsceneScript
{
	public List<Mawaru_Sprite> midspins;

	private List<Transform> charAdd = new List<Transform>();

	private List<Vector3> charStartPos = new List<Vector3>();

	private bool shouldDisplay;

	private float floating = 0.08f;

	private new void Awake()
	{
		base.Awake();
		shouldDisplay = !GCS.seenCutscene2_1a && Persistence.taroStoryProgress < 2;
		if (RDC.skipCutscenes)
		{
			shouldDisplay = false;
		}
		characters[0].render.enabled = false;
		characters[0].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		for (int i = 0; i < 2; i++)
		{
			midspins[i].render.enabled = false;
			midspins[i].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		}
		if (shouldDisplay)
		{
			runnables["Scene2"] = Scene2;
			runnables["SpinOn"] = ShowMidspins;
			runnables["SpinOff"] = HideMidspins;
			runnables["OnComplete"] = FinishCutscene;
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.1"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.2"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.3"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.4"));
			if (Persistence.IsWorldComplete("PA"))
			{
				dialog.Add(RDString.Get("neoCosmosStory.T2.2.pa"));
			}
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.5"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.6"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.7"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.8"));
			dialog.Add(RDString.Get("neoCosmosStory.T2.2.9"));
		}
		charAdd.Add(new GameObject().transform);
		charAdd[0].position = Vector3.down;
		charStartPos.Add(characters[0].transform.localPosition);
	}

	private void Scene2()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(9999f, 0f).SetRelative(isRelative: true);
		GCS.seenCutscene2_1a = true;
		CharFadeIn(characters[0], 0.5f);
		charAdd[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutCubic);
	}

	private void FinishCutscene()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(-9999f, 0f).SetRelative(isRelative: true);
		CharFadeOut(characters[0], 0.5f);
		charAdd[0].DOMoveY(-1f, 0.5f).SetEase(Ease.InCubic);
	}

	private void ShowMidspins()
	{
		for (int i = 0; i < 2; i++)
		{
			CharFadeIn(midspins[i], 0.5f, (float)i * 0.1f);
			DOTween.Sequence().AppendInterval((float)i * 0.1f).Append(midspins[i].transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.OutBack).SetRelative(isRelative: true));
		}
	}

	private void HideMidspins()
	{
		for (int i = 0; i < 2; i++)
		{
			CharFadeOut(midspins[i], 0.5f, (float)i * 0.1f);
			DOTween.Sequence().AppendInterval((float)i * 0.1f).Append(midspins[i].transform.DOLocalMoveY(-1f, 0.5f).SetEase(Ease.InBack).SetRelative(isRelative: true));
		}
	}

	private void Start()
	{
		if (shouldDisplay)
		{
			ADOBase.controller.isCutscene = true;
		}
	}

	private new void Update()
	{
		characters[0].transform.localPosition = charStartPos[0] + floating * Vector3.up * Mathf.Sin(Time.time * 0.5f * (float)Math.PI) + charAdd[0].position;
		for (int i = 0; i < 2; i++)
		{
			midspins[i].transform.eulerAngles = Vector3.forward * 5f * Mathf.Sin(Time.time * 0.7f * (float)Math.PI + (float)i * 0.5f);
		}
		base.Update();
	}
}
