using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SingSing_Notefield : MonoBehaviour
{
	public Mawaru_Sprite backboard;

	public Mawaru_Sprite goal;

	public List<Mawaru_Sprite> midspins;

	private List<scrFloor> floors = new List<scrFloor>();

	public Mawaru_Sprite planet;

	private List<bool> hitChecks = new List<bool>();

	public float startBeat;

	public bool alive;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	public bool isTutorial;

	private Color c4th = new Color(1f, 0.7f, 0.7f, 0.8f);

	private Color c8th = new Color(0.7f, 0.7f, 1f, 0.8f);

	private Color c16th = new Color(1f, 1f, 0.7f, 0.8f);

	private Color redPlanetCol;

	private Color bluePlanetCol;

	private Vector3 errorMeterOn;

	private float xpos;

	private float size;

	public float speed => TaroBGScript.instance.speed;

	public float songBeat => (float)TaroBGScript.instance.songBeat;

	private float ic(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t /= d;
		return c * Mathf.Pow(t, 3f) + b;
	}

	private float oc(float t, float b = 0f, float c = 1f, float d = 1f)
	{
		t = t / d - 1f;
		return c * (Mathf.Pow(t, 3f) + 1f) + b;
	}

	private float beats(float b)
	{
		return b * 60f / 114f * speed;
	}

	public void Awake()
	{
		redPlanetCol = Persistence.GetPlayerColor(red: true).ToRealColor();
		bluePlanetCol = Persistence.GetPlayerColor(red: false).ToRealColor();
		if (TaroBGScript.instance.slumpo)
		{
			planet.transform.localScale = Vector3.zero;
		}
	}

	public void Spawn(float start, List<float> hitBeats, List<int> floorNums, bool hasGoal = false)
	{
		if (scrController.instance?.errorMeter?.gameObject.activeInHierarchy == true)
		{
			errorMeterOn = Vector3.up * 0.8f;
		}
		else
		{
			errorMeterOn = Vector3.zero;
		}
		startBeat = start;
		planet.render.enabled = true;
		if (Mathf.FloorToInt(isTutorial ? (start - 3f) : (start - 34f)) % 12 == 0)
		{
			planet.render.DOColor(redPlanetCol, 0f);
		}
		else
		{
			planet.render.DOColor(bluePlanetCol, 0f);
		}
		hitChecks.Clear();
		floors.Clear();
		foreach (int floorNum in floorNums)
		{
			hitChecks.Add(item: false);
			floors.Add(scrLevelMaker.instance.listFloors[floorNum]);
		}
		backboard.render.enabled = true;
		foreach (Mawaru_Sprite midspin in midspins)
		{
			midspin.render.enabled = false;
		}
		for (int i = 0; i < hitBeats.Count; i++)
		{
			if (Mathf.Abs(hitBeats[i] - Mathf.Floor(hitBeats[i]) - 0f) < 0.005f)
			{
				midspins[i].render.DOColor(c4th, 0f);
			}
			else if (Mathf.Abs(hitBeats[i] - Mathf.Floor(hitBeats[i]) - 0.5f) < 0.005f)
			{
				midspins[i].render.DOColor(c8th, 0f);
			}
			else
			{
				midspins[i].render.DOColor(c16th, 0f);
			}
			midspins[i].render.enabled = true;
			midspins[i].transform.localPosition = (hitBeats[i] - start - 3f) * Vector3.right + Vector3.forward;
			midspins[i].transform.localScale = Vector3.one * 0.66f;
		}
		if (hasGoal)
		{
			goal.render.enabled = true;
			goal.transform.localPosition = 3f * Vector3.right + Vector3.forward;
			goal.transform.localScale = Vector3.one * 0.66f;
		}
		alive = true;
		base.transform.localScale = Vector3.zero;
		if (Mathf.Abs(isTutorial ? (start - 3f) : (start - 34f)) < 0.05f)
		{
			return;
		}
		if (Mathf.Abs(start - 76f) < 0.05f)
		{
			backboard.SetState(1);
			DOTween.Sequence().Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, 0f)).Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(4f))
				.Append(base.transform.DOLocalMoveY(-2.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(5f))
				.AppendCallback(delegate
				{
					alive = false;
				});
			DOTween.Sequence().Append(base.transform.DOScale(0f, 0f)).Append(base.transform.DOScale(Vector3.one * 0.6f, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(4f))
				.Append(base.transform.DOScale(Vector3.one, beats(1f)).SetEase(Ease.InOutCubic))
				.Append(backboard.render.DOColor(whiteClear, beats(4f)).SetEase(Ease.Linear));
		}
		else if (Mathf.Abs(start - 40f) < 0.05f)
		{
			DOTween.Sequence().Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, 0f)).AppendInterval(beats(1f))
				.Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(3f))
				.Append(base.transform.DOLocalMoveY(-2.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(5f))
				.Append(base.transform.DOLocalMoveY(-1.8f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendCallback(delegate
				{
					alive = false;
				});
			DOTween.Sequence().Append(base.transform.DOScale(0f, 0f)).AppendInterval(beats(1f))
				.Append(base.transform.DOScale(Vector3.one * 0.6f, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(3f))
				.Append(base.transform.DOScale(Vector3.one, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(5f))
				.Append(base.transform.DOScaleY(0f, beats(1f)).SetEase(Ease.InOutCubic));
		}
		else
		{
			DOTween.Sequence().Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, 0f)).Append(base.transform.DOLocalMoveY(-3.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(4f))
				.Append(base.transform.DOLocalMoveY(-2.2f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(5f))
				.Append(base.transform.DOLocalMoveY(-1.8f + errorMeterOn.y, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendCallback(delegate
				{
					alive = false;
				});
			DOTween.Sequence().Append(base.transform.DOScale(0f, 0f)).Append(base.transform.DOScale(Vector3.one * 0.6f, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(4f))
				.Append(base.transform.DOScale(Vector3.one, beats(1f)).SetEase(Ease.InOutCubic))
				.AppendInterval(beats(5f))
				.Append(base.transform.DOScaleY(0f, beats(1f)).SetEase(Ease.InOutCubic));
		}
	}

	public void Update()
	{
		if (!alive)
		{
			return;
		}
		for (int i = 0; i < floors.Count; i++)
		{
			if (floors[i].grade != HitMargin.TooEarly && !hitChecks[i])
			{
				hitChecks[i] = true;
				Color color = midspins[i].render.material.GetColor("_Color");
				Color endValue = new Color(color.r, color.g, color.b, 0f);
				DOTween.Sequence().Append(midspins[i].transform.DOLocalMoveY(0.4f, beats(0.5f)).SetRelative(isRelative: true).SetEase(Ease.OutExpo)).AppendInterval(0.1f * speed)
					.Append(midspins[i].render.DOColor(endValue, beats(0.5f)).SetEase(Ease.InQuad));
			}
		}
		if (songBeat < startBeat - 1f)
		{
			xpos = -4f;
		}
		else
		{
			xpos = -4f + 8f * (songBeat - (startBeat - 1f)) / 8f;
		}
		if (xpos < -3f)
		{
			size = oc(xpos + 4f);
		}
		else if (xpos >= -3f && xpos < 3f)
		{
			size = 1f;
		}
		else if (xpos >= 3f)
		{
			size = 1f - ic(xpos - 3f);
		}
		if (Mathf.Abs(startBeat - 76f) < 0.05f && xpos > 0f)
		{
			size = 1f - Mathf.Min(1f, xpos / 2f);
		}
		if (xpos >= 3.999f || xpos < -3.999f)
		{
			planet.render.enabled = false;
		}
		else
		{
			planet.render.enabled = true;
		}
		planet.transform.localPosition = Vector3.right * xpos + Vector3.forward;
		planet.transform.localScale = Vector3.one * size * 0.6f;
		if (Mathf.Abs(isTutorial ? (startBeat - 3f) : (startBeat - 34f)) < 0.05f)
		{
			if (songBeat < startBeat - 2f)
			{
				base.transform.localScale = Vector3.zero;
				base.transform.localPosition = Vector3.up * -3.2f + errorMeterOn;
			}
			else if (songBeat >= startBeat - 2f && songBeat < startBeat - 1f)
			{
				base.transform.localScale = Vector3.one * oc(songBeat - (startBeat - 2f));
				base.transform.localPosition = Vector3.up * (-3.2f + oc(songBeat - (startBeat - 2f))) + errorMeterOn;
			}
			else if (songBeat >= startBeat - 1f && songBeat < startBeat + 5f)
			{
				base.transform.localScale = Vector3.one;
				base.transform.localPosition = Vector3.up * -2.2f + errorMeterOn;
			}
			else if (songBeat >= startBeat + 5f && songBeat < startBeat + 6f)
			{
				base.transform.localScale = Vector3.one - Vector3.up * oc(songBeat - (startBeat + 5f));
				base.transform.localPosition = Vector3.up * (-2.2f + 0.4f * oc(songBeat - (startBeat + 5f))) + errorMeterOn;
			}
			else if (songBeat >= startBeat + 6f)
			{
				base.transform.localScale = Vector3.right + Vector3.forward;
				base.transform.localPosition = Vector3.up * -1.8f + errorMeterOn;
				alive = false;
			}
		}
	}
}
