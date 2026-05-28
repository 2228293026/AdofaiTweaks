using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailerScene2 : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	private List<FloorMeshRenderer> animatedFloors = new List<FloorMeshRenderer>();

	private List<float> angleList = new List<float>();

	private float beat;

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		mb(1f, FunnyText);
		for (int i = 0; i < scrLevelMaker.instance.listFloors.Count; i++)
		{
			animatedFloors.Add(scrLevelMaker.instance.listFloors[i].gameObject.GetComponent<FloorMeshRenderer>());
			angleList.Add(0f);
		}
		mpf(-1f, Roll, 999f);
		mb(2f, Curl);
		SortTables();
	}

	private void FunnyText()
	{
		Debug.Log("Yee haw!");
	}

	private void Curl()
	{
		for (int i = 0; i < angleList.Count - 1; i++)
		{
			int j = i;
			DOTween.Sequence().AppendInterval(beats(1f * (float)j)).Append(DOTween.To(() => angleList[j], delegate(float x)
			{
				angleList[j] = x;
			}, 60f, beats(2f)).SetEase(Ease.Linear));
		}
	}

	private void Roll()
	{
		beat = (float)songBeat;
		UpdateFloorAngles(animatedFloors, angleList);
	}

	private void UpdateFloorAngles(List<FloorMeshRenderer> fm, List<float> anglesToSet, float initAngle = 0f)
	{
		float[] array = new float[anglesToSet.Count];
		float num = initAngle;
		for (int num2 = anglesToSet.Count - 1; num2 >= 0; num2--)
		{
			float num3 = anglesToSet[num2] + num;
			num += anglesToSet[num2];
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			if (num3 > 360f)
			{
				num3 -= 360f;
			}
			array[num2] = num3;
		}
		int num4 = array.Length;
		float num5 = 1.5f;
		float num6 = initAngle;
		Vector2 vector = default(Vector2);
		for (int num7 = num4 - 1; num7 >= 0; num7--)
		{
			FloorMesh floorMesh = fm[num7].floorMesh;
			float f = (float)Math.PI / 180f * num6;
			if (num7 > 0)
			{
				floorMesh.transform.localPosition = new Vector3(vector.x + Mathf.Cos(f) * num5, vector.y + Mathf.Sin(f) * num5, (float)num7 * 0.01f);
			}
			floorMesh._angle1 = num6 - 180f;
			floorMesh._angle0 = array[num7];
			vector = floorMesh.transform.localPosition;
			if (num7 != num4)
			{
				num6 = floorMesh._angle0;
			}
		}
	}

	private void Camera()
	{
	}
}
