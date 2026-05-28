using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class scrMisc
{
	public static void Appear(MonoBehaviour go)
	{
		Material material = go.GetComponent<Renderer>().material;
		Color color = material.color;
		color.a = 1f;
		material.color = color;
	}

	public static GameObject DebugLineColored(Vector3 pos1, Vector3 pos2, Color color1, Color color2)
	{
		GameObject gameObject = new GameObject();
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material.shader = Shader.Find("ADOFAI/Overlay");
		lineRenderer.positionCount = 2;
		lineRenderer.startColor = color1;
		lineRenderer.endColor = color2;
		lineRenderer.SetPosition(0, pos1);
		lineRenderer.SetPosition(1, pos2);
		lineRenderer.startWidth = 0.15f;
		lineRenderer.endWidth = 0.15f;
		lineRenderer.name = "Debug line haha";
		return gameObject;
	}

	public static GameObject DebugLine(Vector3 pos1, Vector3 pos2)
	{
		return DebugLineColored(pos1, pos2, new Color(1f, 0f, 0f, 1f), new Color(1f, 0.5f, 0f, 1f));
	}

	public static void Rotate2D(Transform trans, float rotation)
	{
		trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, rotation);
	}

	public static void Rotate2DCW(Transform trans, float rotation)
	{
		trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, 0f - rotation);
	}

	public static void RotateWorld2DCW(Transform trans, float rotation)
	{
		trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, 0f - rotation);
	}

	public static void ScaleSymmetric2D(Transform trans, float scale)
	{
		trans.localScale = new Vector3(scale, scale, trans.localScale.z);
	}

	public static bool IsFloorHere(float _x, float _y)
	{
		if (Physics2D.OverlapPointAll(new Vector2(_x, _y), 1 << LayerMask.NameToLayer("Floor")).Length != 0)
		{
			return true;
		}
		return false;
	}

	public static void Vibrate(long ms)
	{
		if (GCS.d_vibrate)
		{
			GameServices.Instance.Vibrate(ms);
		}
	}

	public static int GetNewRandInt(int max, int current)
	{
		if (max <= 0)
		{
			Debug.Log("tried to get a different int when max was zero :/");
			return 0;
		}
		int num;
		for (num = Mathf.FloorToInt(UnityEngine.Random.Range(0, max + 1)); num == current; num = Mathf.FloorToInt(UnityEngine.Random.Range(0, max + 1)))
		{
		}
		return num;
	}

	public static int GetRandInt(int max)
	{
		return Mathf.FloorToInt(UnityEngine.Random.Range(0, max + 1));
	}

	public static void WriteFloatToStatus(float _num)
	{
		if (RDC.debug)
		{
			scrController.instance.txtLevelName.text = _num.ToString();
		}
	}

	public static void WriteStringToStatus(string stringy)
	{
		if (RDC.debug)
		{
			scrController.instance.txtLevelName.text = stringy;
		}
	}

	public static Vector3 incrementX(Vector3 ori, float amount)
	{
		Vector3 result = ori;
		result.x = ori.x + amount;
		WriteFloatToStatus(result.x);
		return result;
	}

	public static Vector3 incrementY(Vector3 ori, float amount)
	{
		Vector3 result = ori;
		result.y = ori.y + amount;
		WriteFloatToStatus(result.y);
		return result;
	}

	public static Vector3 incrementZ(Vector3 ori, float amount)
	{
		Vector3 result = ori;
		result.z = ori.z + amount;
		WriteFloatToStatus(result.z);
		return result;
	}

	public static float incrementFloat(float ori, float amount)
	{
		float num = ori + amount;
		WriteFloatToStatus(num);
		return num;
	}

	public static int setInt(int _int)
	{
		WriteFloatToStatus(_int);
		return _int;
	}

	public static Vector3 setX(Vector3 pos, float x)
	{
		Vector3 result = pos;
		result.x = x;
		return result;
	}

	public static Vector3 setY(Vector3 pos, float y)
	{
		Vector3 result = pos;
		result.y = y;
		return result;
	}

	public static Vector3 setZ(Vector3 pos, float z)
	{
		Vector3 result = pos;
		result.z = z;
		return result;
	}

	public static bool isAcuteDiffInMargin(float x, float y, float margin)
	{
		float num = Mathf.Abs(x - y);
		return Mathf.Min(num, (float)Math.PI * 2f - num) < margin;
	}

	public static bool isDiffInMargin(double x, double y, double margin)
	{
		return Math.Abs(x - y) < margin;
	}

	public static double GetAdjustedAngleBoundaryInDeg(HitMarginGeneral marginType, double bpmTimesSpeed, double conductorPitch, double marginMult = 1.0)
	{
		float num = 0.065f;
		if (GCS.difficulty == Difficulty.Lenient)
		{
			num = 0.091f;
		}
		if (GCS.difficulty == Difficulty.Normal)
		{
			num = 0.065f;
		}
		if (GCS.difficulty == Difficulty.Strict)
		{
			num = 0.04f;
		}
		bool isMobile = ADOBase.isMobile;
		num = (isMobile ? 0.09f : (num / GCS.currentSpeedTrial));
		float a = (isMobile ? 0.07f : (0.03f / GCS.currentSpeedTrial));
		float a2 = (isMobile ? 0.05f : (0.02f / GCS.currentSpeedTrial));
		num = Mathf.Max(num, 0.025f);
		a = Mathf.Max(a, 0.025f);
		float num2 = Mathf.Max(a2, 0.025f);
		double val = TimeToAngleInRad(num, bpmTimesSpeed, conductorPitch) * 57.295780181884766;
		double val2 = TimeToAngleInRad(a, bpmTimesSpeed, conductorPitch) * 57.295780181884766;
		double val3 = TimeToAngleInRad(num2, bpmTimesSpeed, conductorPitch) * 57.295780181884766;
		double num3 = Math.Max((double)GCS.HITMARGIN_COUNTED * marginMult, val);
		double num4 = Math.Max(45.0 * marginMult, val2);
		double num5 = Math.Max(30.0 * marginMult, val3);
		return marginType switch
		{
			HitMarginGeneral.Counted => num3, 
			HitMarginGeneral.Perfect => num4, 
			HitMarginGeneral.Pure => num5, 
			_ => num3, 
		};
	}

	public static float GetHitMarginIncrease(float highestBpm)
	{
		float num = 0.065f;
		if (GCS.difficulty == Difficulty.Lenient)
		{
			num = 0.091f;
		}
		if (GCS.difficulty == Difficulty.Normal)
		{
			num = 0.065f;
		}
		if (GCS.difficulty == Difficulty.Strict)
		{
			num = 0.04f;
		}
		return (1f / 3f * (60f / highestBpm) - num) * 2f;
	}

	public static HitMargin GetHitMargin(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale = 1.0)
	{
		float num = (hitangle - refangle) * (float)(isCW ? 1 : (-1));
		HitMargin result = HitMargin.TooEarly;
		float num2 = num;
		num2 = 57.29578f * num2;
		double adjustedAngleBoundaryInDeg = GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch, marginScale);
		double adjustedAngleBoundaryInDeg2 = GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch, marginScale);
		double adjustedAngleBoundaryInDeg3 = GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch, marginScale);
		if ((double)num2 > 0.0 - adjustedAngleBoundaryInDeg)
		{
			result = HitMargin.VeryEarly;
		}
		if ((double)num2 > 0.0 - adjustedAngleBoundaryInDeg2)
		{
			result = HitMargin.EarlyPerfect;
		}
		if ((double)num2 > 0.0 - adjustedAngleBoundaryInDeg3)
		{
			result = HitMargin.Perfect;
		}
		if ((double)num2 > adjustedAngleBoundaryInDeg3)
		{
			result = HitMargin.LatePerfect;
		}
		if ((double)num2 > adjustedAngleBoundaryInDeg2)
		{
			result = HitMargin.VeryLate;
		}
		if ((double)num2 > adjustedAngleBoundaryInDeg)
		{
			result = HitMargin.TooLate;
		}
		return result;
	}

	public static bool IsValidHit(HitMargin margin)
	{
		if (margin == HitMargin.Auto)
		{
			return true;
		}
		switch (GCS.hitMarginLimit)
		{
		case HitMarginLimit.PerfectsOnly:
			if (HitMargin.EarlyPerfect <= margin)
			{
				return margin <= HitMargin.LatePerfect;
			}
			return false;
		case HitMarginLimit.PurePerfectOnly:
			return margin == HitMargin.Perfect;
		default:
			if (HitMargin.VeryEarly <= margin)
			{
				return margin <= HitMargin.VeryLate;
			}
			return false;
		}
	}

	public static bool IsFirstAngleOnLeft(float x, float y)
	{
		return mod(y - x, 6.2831854820251465) < 3.1415927410125732;
	}

	public static double getAcuteAngle(double a, double b)
	{
		double num = Math.Abs(a - b);
		return mod(Math.Min(num, Math.PI * 2.0 - num), Math.PI * 2.0);
	}

	public static double mod(double x, double m)
	{
		return (x % m + m) % m;
	}

	public static int ModInt(int x, int m)
	{
		return (x % m + m) % m;
	}

	public static Vector3 getVectorFromAngle(double angle, double radius)
	{
		return new Vector3((float)(Math.Sin(angle) * radius), (float)(Math.Cos(angle) * radius), 0f);
	}

	public static float getXFromAngleAndRadius(float angle, float radius)
	{
		return Mathf.Sin(angle) * radius;
	}

	public static float getYFromAngleAndRadius(float angle, float radius)
	{
		return Mathf.Cos(angle) * radius;
	}

	public static double incrementAngle(double startangle, double increment)
	{
		return mod(startangle + increment, 6.2831854820251465);
	}

	public static double GetTimeBetweenAngles(double entryAngle, double exitAngle, double speed, double bpm, bool isCW)
	{
		double num = (isCW ? 1f : (-1f));
		return mod((exitAngle - entryAngle) * num, 6.2831854820251465) / 3.1415927410125732 * (bpm2crotchet(bpm) / speed);
	}

	public static double GetInverseAnglePerBeatMultiplanet(double planets)
	{
		return 3.1415926 * (planets - 2.0) / planets;
	}

	public static double GetAnglePerBeatMultiplanet(double planets)
	{
		return 3.1415926 - GetInverseAnglePerBeatMultiplanet(planets);
	}

	public static double GetBeatFromSongTimeAndBPMs(double songtime, List<Tuple<double, double>> bpms, double offset = 0.0)
	{
		songtime += offset;
		for (int i = 0; i < bpms.Count; i++)
		{
			double item = bpms[i].Item1;
			bool num = i == bpms.Count - 1;
			double num2 = 1000000.0;
			if (!num)
			{
				num2 = bpms[i + 1].Item1;
			}
			double num3 = (num2 - item) / (bpms[i].Item2 / 60.0);
			if (num || songtime <= num3)
			{
				return item + songtime * (bpms[i].Item2 / 60.0);
			}
			songtime -= num3;
		}
		return -1.0;
	}

	public static double GetAngleMoved(double entryAngle, double exitAngle, bool isCW)
	{
		float num = (isCW ? 1f : (-1f));
		return mod((exitAngle - entryAngle) * (double)num, 6.2831854820251465);
	}

	public static double GetAngleMovedNoMod(double entryAngle, double exitAngle, bool isCW)
	{
		float num = (isCW ? 1f : (-1f));
		return (exitAngle - entryAngle) * (double)num;
	}

	public static double AngleToTime(double angle, double bpm)
	{
		angle = mod(angle, 6.2831854820251465);
		return angle / 3.1415927410125732 * bpm2crotchet(bpm);
	}

	public static float EasedAngle(float entryAngle, float exitAngle, float angle, Ease ease, int easeParts = 1, EasePartBehavior easePartBehavior = EasePartBehavior.Mirror)
	{
		scrController instance = scrController.instance;
		if (scrConductor.instance.songposition_minusi - instance.chosenPlanet.currfloor.entryTime < 0.0)
		{
			return angle;
		}
		if (easeParts <= 0)
		{
			return angle;
		}
		float num = (float)GetAngleMovedNoMod(entryAngle, exitAngle, instance.playerOne.planetarySystem.isCW);
		float num2 = (float)GetAngleMovedNoMod(entryAngle, angle, instance.playerOne.planetarySystem.isCW);
		if ((double)Math.Abs(num) <= Math.Pow(10.0, -6.0))
		{
			num = (float)Math.PI * 2f;
		}
		float num3 = Mathf.Abs(num2 / num);
		num3 *= (float)easeParts;
		num /= (float)easeParts;
		int num4 = Mathf.FloorToInt(Mathf.Abs(num3));
		float num5 = 0f;
		num5 = ((num4 % 2 != 0 && easePartBehavior != EasePartBehavior.Repeat) ? (num - DOVirtual.EasedValue(0f, num, 1f - (num3 - (float)num4), ease)) : DOVirtual.EasedValue(0f, num, num3 - (float)num4, ease));
		return entryAngle + (num * (float)num4 + num5) * (instance.playerOne.planetarySystem.isCW ? 1f : (-1f));
	}

	public static double TimeToAngleInRad(double timeinAbsoluteSpace, double bpmTimesSpeed, double conductorPitch, bool shrinkMarginsForHigherPitch = false)
	{
		double num = ((!shrinkMarginsForHigherPitch) ? (timeinAbsoluteSpace * conductorPitch) : timeinAbsoluteSpace);
		return num * 3.1415927410125732 / bpm2crotchet(bpmTimesSpeed);
	}

	public static DifficultyUIMode DetermineDifficultyUIMode(float highestBPM)
	{
		if (ADOBase.isMobile)
		{
			return DifficultyUIMode.DontShow;
		}
		if ((bool)ADOBase.customLevel && !ADOBase.isOfficialLevel)
		{
			if (highestBPM >= 310f)
			{
				return DifficultyUIMode.ShowAll;
			}
			if (highestBPM >= 220f)
			{
				return DifficultyUIMode.ShowLenientAndNormal;
			}
		}
		else
		{
			string currentLevel = ADOBase.currentLevel;
			string key = currentLevel.Split('-', StringSplitOptions.None)[0];
			if (currentLevel.IsBossLevel())
			{
				return GCNS.worldData[key].availableDifficulties;
			}
		}
		return DifficultyUIMode.DontShow;
	}

	public static double bpm2crotchet(double bpm)
	{
		return 60.0 / bpm;
	}

	public static bool ApproximatelyFloor(double a, double b)
	{
		return Math.Abs(a - b) < 0.001;
	}

	public static bool InputNoArrows()
	{
		if (!Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
		{
			return !Input.GetKeyDown(KeyCode.RightArrow);
		}
		return false;
	}

	public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
	{
		float num = from - fromMin;
		float num2 = fromMax - fromMin;
		float num3 = num / num2;
		return (toMax - toMin) * num3 + toMin;
	}

	public static void AnchorPosX(this RectTransform rt, float x)
	{
		rt.anchoredPosition = rt.anchoredPosition.WithX(x);
	}

	public static void AnchorPosY(this RectTransform rt, float y)
	{
		rt.anchoredPosition = rt.anchoredPosition.WithY(y);
	}

	public static void SizeDeltaX(this RectTransform rt, float x)
	{
		rt.sizeDelta = rt.sizeDelta.WithX(x);
	}

	public static void SizeDeltaY(this RectTransform rt, float y)
	{
		rt.sizeDelta = rt.sizeDelta.WithY(y);
	}

	public static void PositionX(this Transform t, float x)
	{
		t.position = t.position.WithX(x);
	}

	public static void PositionY(this Transform t, float y)
	{
		t.position = t.position.WithY(y);
	}

	public static double ReflectAngle(double input, double normal)
	{
		return (input - normal) * -1.0 + normal;
	}

	public static bool ColorsAreCloseEnough(Color a, Color b, float epsilon = 0.01f)
	{
		Vector3 a2 = new Vector3(a.r, a.g, a.b);
		Vector3 b2 = new Vector3(b.r, b.g, b.b);
		return Vector3.Distance(a2, b2) < epsilon;
	}
}
