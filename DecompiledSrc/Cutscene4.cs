using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cutscene4 : TaroCutsceneScript
{
	private List<Transform> charAdd = new List<Transform>();

	private List<Vector3> charStartPos = new List<Vector3>();

	public GameObject cameraFocus;

	private bool shouldDisplay;

	private float floating = 0.08f;

	private new void Awake()
	{
		base.Awake();
		shouldDisplay = !Persistence.IsWorldComplete("T3") && !GCS.seenCutscene3_1;
		if (RDC.skipCutscenes)
		{
			shouldDisplay = false;
		}
		characters[0].render.enabled = false;
		characters[0].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		if (shouldDisplay)
		{
			runnables["Scene2"] = Scene2;
			runnables["ShowIsland"] = ShowIsland;
			runnables["BackFromIsland"] = BackFromIsland;
			runnables["OnComplete"] = FinishCutscene;
			runnables["Skippable"] = Skippable;
			runnables["Unskippable"] = Unskippable;
			dialog.Add(RDString.Get("neoCosmosStory.T3.1.1"));
			dialog.Add(RDString.Get("neoCosmosStory.T3.1.2"));
			dialog.Add(RDString.Get("neoCosmosStory.T3.1.3"));
			dialog.Add(RDString.Get("neoCosmosStory.T3.1.4"));
		}
		charAdd.Add(new GameObject().transform);
		charAdd[0].position = Vector3.down;
		charStartPos.Add(characters[0].transform.localPosition);
	}

	private void Skippable()
	{
		canSkip = true;
	}

	private void Unskippable()
	{
		canSkip = false;
	}

	private void Scene2()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(9999f, 0f).SetRelative(isRelative: true);
		GCS.seenCutscene3_1 = true;
		CharFadeIn(characters[0], 0.5f);
		charAdd[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutCubic);
	}

	private void FinishCutscene()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(-9999f, 0f).SetRelative(isRelative: true);
		CharFadeOut(characters[0], 0.5f);
		charAdd[0].DOMoveY(-1f, 0.5f).SetEase(Ease.InCubic);
	}

	private void ShowIsland()
	{
		ADOBase.controller.MoveCameraToObject(cameraFocus, 2f, Ease.Linear);
	}

	private void BackFromIsland()
	{
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.InOutCubic);
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
		base.Update();
	}
}
