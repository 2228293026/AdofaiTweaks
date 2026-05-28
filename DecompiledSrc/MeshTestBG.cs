using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MeshTestBG : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	private List<FloorMeshRenderer> floorMeshes = new List<FloorMeshRenderer>();

	private List<FloorMeshRenderer> floorMeshes2 = new List<FloorMeshRenderer>();

	private float[] dragon = new float[48]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 90f, -240f, 90f, 60f,
		30f, 90f, -240f, -180f, -150f, -180f, -210f, -240f, 60f, 0f,
		30f, 30f, 45f, 0f, 30f, 30f, 0f, -150f, -150f, -120f,
		30f, 0f, -120f, 0f, -120f, -150f, 0f, -120f, -60f, -90f,
		-90f, -120f, -90f, -60f, -30f, 0f, 0f, 0f
	};

	private float[] pensive = new float[13]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 60f, 60f, 120f, 120f,
		60f, 60f, 60f
	};

	private List<float> angleList = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f, 0f };

	private List<float> angleList2 = new List<float> { -60f, -60f, -60f, -60f, -60f };

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		for (int i = 5; i < 12; i++)
		{
			floorMeshes.Add(scrLevelMaker.instance.listFloors[i].gameObject.GetComponent<FloorMeshRenderer>());
		}
		for (int j = 14; j < 19; j++)
		{
			floorMeshes2.Add(scrLevelMaker.instance.listFloors[j].gameObject.GetComponent<FloorMeshRenderer>());
		}
		mb(3f, SetupFloors);
		mpf(3f, AnimateFloors, 30f);
		mb(9f, SetupFloors2);
		mb(-999f, CurlUpFloors2);
		SortTables();
	}

	private void SetupFloors()
	{
		Debug.Log("Yee haw!");
		float[] array = new float[7] { 60f, 60f, 120f, 120f, 60f, 60f, 60f };
		for (int i = 0; i < angleList.Count; i++)
		{
			int j = i;
			float duration = beats(3f - (float)i * 0.25f);
			DOTween.Sequence().AppendInterval(beats(0.5f * (float)j)).Append(DOTween.To(() => angleList[j], delegate(float x)
			{
				angleList[j] = x;
			}, array[j], duration).SetEase(Ease.InOutBack));
		}
	}

	private void SetupFloors2()
	{
		Debug.Log("Yee haw! (2)");
		float[] array = new float[5];
		for (int i = 0; i < angleList2.Count; i++)
		{
			int j = i;
			float duration = beats(3f - (float)i * 0.25f);
			DOTween.Sequence().AppendInterval(beats(0.5f * (float)j)).Append(DOTween.To(() => angleList2[j], delegate(float x)
			{
				angleList2[j] = x;
			}, array[j], duration).SetEase(Ease.InOutBack));
		}
	}

	private void CurlUpFloors2()
	{
		UpdateFloorAngles(floorMeshes2, angleList2);
	}

	private void AnimateFloors()
	{
		float num = (float)songBeat;
		if (num > 3f && num < 12f)
		{
			UpdateFloorAngles(floorMeshes, angleList);
		}
		if (num > 9f && num < 18f)
		{
			UpdateFloorAngles(floorMeshes2, angleList2);
		}
	}

	private void UpdateFloorAngles(List<FloorMeshRenderer> fm, List<float> anglesToSet)
	{
		float[] array = new float[anglesToSet.Count];
		float num = 0f;
		for (int i = 0; i < anglesToSet.Count; i++)
		{
			float num2 = anglesToSet[i] + num;
			num += anglesToSet[i];
			if (num2 < 0f)
			{
				num2 += 360f;
			}
			if (num2 > 360f)
			{
				num2 -= 360f;
			}
			array[i] = num2;
		}
		int num3 = array.Length;
		float num4 = 1.5f;
		float num5 = 0f;
		Vector2 vector = default(Vector2);
		for (int j = 0; j < num3; j++)
		{
			FloorMesh floorMesh = fm[j].floorMesh;
			float f = (float)Math.PI / 180f * num5;
			if (j > 0)
			{
				floorMesh.transform.localPosition = new Vector3(vector.x + Mathf.Cos(f) * num4, vector.y + Mathf.Sin(f) * num4, (float)j * 0.01f);
			}
			floorMesh._angle0 = num5 - 180f;
			floorMesh._angle1 = array[j];
			vector = floorMesh.transform.localPosition;
			if (j != num3)
			{
				num5 = floorMesh._angle1;
			}
		}
	}
}
