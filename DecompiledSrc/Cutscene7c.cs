using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cutscene7c : TaroCutsceneScript
{
	private List<Transform> charAdd = new List<Transform>();

	private List<Vector3> charStartPos = new List<Vector3>();

	private bool shouldDisplay;

	private bool animating;

	private float floating = 0.08f;

	private new void Awake()
	{
		base.Awake();
		shouldDisplay = !GCS.seenCutscene5_4 && Persistence.taroStoryProgress < 6;
		if (RDC.skipCutscenes)
		{
			shouldDisplay = false;
		}
		characters[0].SetState(0);
		characters[0].render.enabled = false;
		characters[0].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		if (shouldDisplay)
		{
			runnables["Scene2"] = Scene2;
			runnables["OnComplete"] = FinishCutscene;
			dialog.Add(RDString.Get("neoCosmosStory.T5.4.1"));
			dialog.Add(RDString.Get("neoCosmosStory.T5.4.2"));
			dialog.Add(RDString.Get("neoCosmosStory.T5.4.3"));
		}
		charAdd.Add(new GameObject().transform);
		charAdd[0].position = Vector3.down;
		charStartPos.Add(characters[0].transform.localPosition);
	}

	private void Scene2()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(9999f, 0f).SetRelative(isRelative: true);
		GCS.seenCutscene5_4 = true;
		CharFadeIn(characters[0], 0.5f);
		charAdd[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutCubic);
	}

	private void FinishCutscene()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(-9999f, 0f).SetRelative(isRelative: true);
		CharFadeOut(characters[0], 0.5f);
		charAdd[0].DOMoveY(-1f, 0.5f).SetEase(Ease.InCubic);
	}

	private void Start()
	{
		if (shouldDisplay)
		{
			animating = true;
			ADOBase.controller.isCutscene = true;
		}
	}

	private new void Update()
	{
		if (animating)
		{
			characters[0].transform.localPosition = charStartPos[0] + floating * Vector3.up * Mathf.Sin(Time.time * 0.5f * (float)Math.PI) + charAdd[0].position;
		}
		base.Update();
	}
}
