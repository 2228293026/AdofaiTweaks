using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PuzzleRoom : MonoBehaviour
{
	private static PuzzleRoom _instance;

	public List<scrFloor> listFloors;

	public List<scrSpike> listSpikes;

	public Dictionary<scrFloor, bool> landedOn = new Dictionary<scrFloor, bool>();

	private Color wobblyFloor = new Color(1f, 0.9f, 0.8f, 1f);

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Vector3 scatter = Vector3.zero;

	private float ypos;

	public static PuzzleRoom instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindAnyObjectByType<PuzzleRoom>();
			}
			return _instance;
		}
	}

	public scrController controller => scrController.instance;

	public scrConductor conductor => scrConductor.instance;

	private void Awake()
	{
		scrFloor[] array = UnityEngine.Object.FindObjectsByType(typeof(scrFloor), FindObjectsSortMode.None) as scrFloor[];
		for (int i = 0; i < array.Length; i++)
		{
			listFloors.Add(array[i]);
			landedOn[array[i]] = false;
			if (array[i].unstable)
			{
				array[i].SetColor(wobblyFloor);
				array[i].MoveToBack();
			}
		}
		scrSpike[] array2 = UnityEngine.Object.FindObjectsByType(typeof(scrSpike), FindObjectsSortMode.None) as scrSpike[];
		for (int j = 0; j < array2.Length; j++)
		{
			listSpikes.Add(array2[j]);
		}
	}

	private void Start()
	{
		GCS.puzzle = true;
	}

	private void Update()
	{
		foreach (scrSpike listSpike in listSpikes)
		{
			if (controller.currentState != States.PlayerControl)
			{
				continue;
			}
			foreach (scrPlanet planet in controller.planetarySystem.planetList)
			{
				if (!listSpike.hit && listSpike.hittable && Vector3.Distance(planet.transform.position, listSpike.pos) < 0.6f && !RDC.auto && planet.iFrames <= 0f)
				{
					listSpike.hit = true;
					if (!controller.freeroamInvulnerability)
					{
						planet.Die();
						planet.player.DieByHitbox();
						DOTween.Sequence().Append(listSpike.transform.DOScale(Vector3.one * 0.2f, 0f).SetRelative(isRelative: true)).Append(listSpike.transform.DOScale(Vector3.one * -0.2f, 0.5f).SetEase(Ease.OutCubic).SetRelative(isRelative: true));
						continue;
					}
					listSpike.Die();
					float f = (float)controller.chosenPlanet.angle - 1.570796f * (float)(controller.playerOne.planetarySystem.isCW ? 1 : (-1));
					scatter = Vector3.left * 2f * Mathf.Sin(f) + Vector3.down * 2f * Mathf.Cos(f) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
					listSpike.ballSprite.render.DOColor(whiteClear, (float)conductor.crotchetAtStart / conductor.song.pitch).SetEase(Ease.Linear);
					listSpike.transform.DOLocalMove(scatter, (float)conductor.crotchetAtStart / conductor.song.pitch).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
					listSpike.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), (float)conductor.crotchetAtStart / conductor.song.pitch, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
					scrSfx.instance.PlaySfx(SfxSound.ModifierActivate, MixerGroup.InterfaceParent, 0.8f);
				}
			}
		}
		if (!landedOn[controller.currFloor])
		{
			landedOn[controller.currFloor] = true;
			if (controller.currFloor.unstable)
			{
				DOTween.Sequence().Append(controller.currFloor.transform.DOLocalMoveY(controller.currFloor.startPos.y - 0.3f, 0f).SetEase(Ease.InCubic)).Append(controller.currFloor.transform.DOLocalMoveY(0.3f, 1f).SetRelative(isRelative: true).SetEase(Ease.OutElastic));
				DOTween.Sequence().Append(controller.currFloor.transform.DOScale(Vector3.one * 0.9f, 0f).SetEase(Ease.InCubic)).Append(controller.currFloor.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic));
			}
		}
		foreach (scrFloor listFloor in listFloors)
		{
			listFloor.stickToFloor = true;
			if (listFloor.unstable && !landedOn[listFloor])
			{
				ypos = Mathf.Min(0.47f + 0.5f * Mathf.Sin(Time.time * (float)Math.PI - listFloor.transform.position.x * 0.25f * (float)Math.PI), 0f);
				listFloor.transform.localPosition = listFloor.startPos + Vector3.up * ypos * 2f;
				listFloor.transform.localScale = Vector3.one * (1f + ypos);
			}
		}
	}
}
