using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TestRoom : TaroBGScript
{
	[Header("Assets", order = 0)]
	[Header("  Test", order = 1)]
	public GameObject beatAux;

	public TextMeshPro textMesh;

	private MeshRenderer textMeshRender;

	private float countdownOrigPos;

	private Color fadedText = new Color(1f, 1f, 1f, 0.6f);

	private Dictionary<Filter, MonoBehaviour> filterToComp => scrVfxPlus.instance.filterToComp;

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 100.0));
		mpf(-100f, FloorVibes, 100f);
		mb(-1f, LevelNameTextAway);
		mb(2f, FunnyText);
		mb(5f, DOFisheye);
		SortTables();
		textMeshRender = textMesh.GetComponent<MeshRenderer>();
		countdownOrigPos = scrUIController.instance.txtCountdown.transform.position.y;
	}

	public void LevelNameTextAway()
	{
		scrUIController.instance.txtLevelName.transform.DOMoveY(100f, 1f).SetRelative(isRelative: true).SetEase(Ease.InBack);
	}

	private void FunnyText()
	{
	}

	private void DOFisheye()
	{
		DOTween.Sequence().Append(thisTransform.DOMoveX(-0.1f, beats(0.2f))).Append(thisTransform.DOMoveX(0f, beats(1.5f)).SetEase(Ease.OutBack));
	}

	public void DoText(string text, float hangTime, float persistTime, float zoom = 1f)
	{
		scrUIController.instance.txtCountdown.transform.DOMoveY(countdownOrigPos - 160f, 0f);
		textMesh.text = text;
		textMeshRender.enabled = true;
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(textMesh.transform.DOLocalMove(Vector3.up * 2f + Vector3.forward * 9.95f, 0f))
			.AppendInterval(beats(hangTime, currentBPM))
			.Append(textMesh.transform.DOLocalMove(Vector3.up * 4.6f + Vector3.forward * 9.95f, beats(2f, currentBPM)).SetEase(Ease.InOutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(textMesh.transform.DOScale(Vector3.one * 2f * zoom + Vector3.right * zoom, 0f))
			.Append(textMesh.transform.DOScale(Vector3.one * zoom, 0.3f * speed).SetEase(Ease.InCubic))
			.AppendInterval(beats(hangTime, currentBPM) - 0.3f * speed)
			.Append(textMesh.transform.DOScale(Vector3.one * 0.5f * zoom, beats(2f, currentBPM)).SetEase(Ease.InOutCubic));
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(textMesh.DOColor(whiteClear, 0f))
			.Append(textMesh.DOColor(Color.white, 0.3f * speed).SetEase(Ease.Linear))
			.AppendInterval(beats(hangTime, currentBPM) - 0.3f * speed)
			.Append(textMesh.DOColor(fadedText, beats(2f, currentBPM)).SetEase(Ease.Linear))
			.AppendInterval(beats(persistTime, currentBPM))
			.Append(textMesh.DOColor(whiteClear, 1f * speed).SetEase(Ease.Linear));
	}

	public void FloorVibes()
	{
		for (int i = 0; i < 4; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibe(floor, 0.5f, 0f, 4f, 0f, 2f);
			FloorDrunkVibe(floor, 0.5f, (float)Math.PI / 2f, 4f, (float)Math.PI / 2f, 2f);
		}
		float distortion = thisTransform.position.x + 0.5f;
		if (thisTransform.position.x > 0f)
		{
			distortion = thisTransform.position.x * 2f + 0.5f;
		}
		(filterToComp[Filter.Fisheye] as CameraFilterPack_Distortion_FishEye).Distortion = distortion;
	}
}
