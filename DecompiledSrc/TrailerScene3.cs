using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailerScene3 : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	public GameObject charlie;

	public GameObject sef;

	public GameObject sefInner;

	public GameObject quadFloor;

	private List<FloorMeshRenderer> animatedFloors = new List<FloorMeshRenderer>();

	private List<float> angleList = new List<float>();

	private float beat;

	private float localTimer;

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		for (int i = 0; i < scrLevelMaker.instance.listFloors.Count; i++)
		{
			animatedFloors.Add(scrLevelMaker.instance.listFloors[i].gameObject.GetComponent<FloorMeshRenderer>());
			angleList.Add(0f);
		}
		mb(5f, Runner);
		mb(8.5f, Curl);
		mb(11f, Taur);
		mpf(-1f, Float, 999f);
		SortTables();
	}

	private void Runner()
	{
		charlie.transform.DOMoveX(60f, beats(40f)).SetEase(Ease.Linear);
	}

	private void Taur()
	{
		sef.transform.DOMoveX(60f, beats(40f)).SetEase(Ease.Linear);
	}

	private void Floor()
	{
		quadFloor.transform.DOMoveX(60f, beats(40f)).SetEase(Ease.Linear);
	}

	private void Curl()
	{
		quadFloor.transform.DOMoveX(60f, beats(40f)).SetEase(Ease.Linear);
		for (int i = 0; i < angleList.Count - 1; i++)
		{
			int j = i;
			DOTween.Sequence().AppendInterval(beats(1f * (float)j)).Append(DOTween.To(() => angleList[j], delegate(float x)
			{
				angleList[j] = x;
			}, 60f, beats(2f)).SetEase(Ease.Linear));
		}
	}

	private void Float()
	{
		localTimer += Time.deltaTime * 1f;
		float num = 0.45f;
		Vector3 vector = Vector3.up * 0.015f * Mathf.Sin(localTimer * (float)Math.PI * 2f * 0.8f) + Vector3.right * 0.01f * Mathf.Cos(localTimer * (float)Math.PI * 2f * 0.8f);
		sefInner.transform.localPosition = Vector3.right * 0.3f * Mathf.Sin(localTimer * 2f * (float)Math.PI * 0.5f) + Vector3.up * 0.3f * Mathf.Cos(localTimer * 2f * (float)Math.PI * 0.5f);
		sefInner.transform.localScale = Vector3.right * num + Vector3.up * num + Vector3.forward + vector;
		sefInner.transform.eulerAngles = Vector3.forward * 2f * Mathf.Sin(localTimer * 2f * (float)Math.PI * 0.5f);
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
		FloorMesh floorMesh = fm[0].floorMesh;
		for (int num7 = num4 - 1; num7 >= 0; num7--)
		{
			FloorMesh floorMesh2 = fm[num7].floorMesh;
			float f = (float)Math.PI / 180f * num6;
			if (num7 < num4 - 1)
			{
				floorMesh2.transform.localPosition = floorMesh.transform.localPosition + new Vector3(vector.x - Mathf.Cos(f) * num5, vector.y + Mathf.Sin(f) * num5, (float)num7 * 0.01f);
			}
			floorMesh2._angle1 = 0f - num6;
			floorMesh2._angle0 = 0f - (array[num7] - 180f);
			vector = floorMesh2.transform.localPosition - floorMesh.transform.localPosition;
			if (num7 != num4)
			{
				num6 = array[num7];
			}
		}
	}
}
