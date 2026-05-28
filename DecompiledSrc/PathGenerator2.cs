using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator2 : MonoBehaviour
{
	[Header("GameObjects")]
	public GameObject floorPrefab;

	private Coroutine animation;

	private float length;

	public float customSpacing;

	private const float f = -30f;

	private float[] angles = new float[128];

	private float[] dragon = new float[48]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 90f, -240f, 90f, 60f,
		30f, 90f, -240f, -180f, -150f, -180f, -210f, -240f, 60f, 0f,
		30f, 30f, 45f, 0f, 30f, 30f, 0f, -150f, -150f, -120f,
		30f, 0f, -120f, 0f, -120f, -150f, 0f, -120f, -60f, -90f,
		-90f, -120f, -90f, -60f, -30f, 0f, 0f, 0f
	};

	private List<FloorMesh> floors = new List<FloorMesh>();

	private void Awake()
	{
		RDUtils.targetFrameRate = 75;
		length = floorPrefab.GetComponent<FloorMesh>()._length * 2f;
		Gen(angles, 0f);
		MonoBehaviour.print("length " + length);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Gen(angles, 1f);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Gen(dragon, 1f);
		}
	}

	private void Gen(float[] anglesToSet, float duration, float offset = 0f)
	{
		float num = 0f;
		foreach (FloorMesh floor in floors)
		{
			floor.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < anglesToSet.Length; i++)
		{
			_ = anglesToSet[i];
			if (i >= floors.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(floorPrefab, base.transform);
				gameObject.name = "floor" + i;
				floors.Add(gameObject.GetComponent<FloorMesh>());
			}
		}
		float[] array = new float[anglesToSet.Length];
		float[] array2 = new float[anglesToSet.Length];
		float[] array3 = new float[anglesToSet.Length];
		float[] array4 = new float[anglesToSet.Length];
		for (int j = 0; j < anglesToSet.Length; j++)
		{
			FloorMesh floorMesh = floors[j];
			floorMesh.gameObject.SetActive(value: true);
			array[j] = floorMesh._angle0;
			array2[j] = floorMesh._angle1;
			float num2 = num - 180f;
			float num3 = anglesToSet[j];
			if (num2 < 0f)
			{
				num2 += 360f;
			}
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			array3[j] = num2;
			array4[j] = num3;
			if (j != anglesToSet.Length)
			{
				num = anglesToSet[j];
			}
		}
		if (animation != null)
		{
			StopCoroutine(animation);
		}
		animation = StartCoroutine(Animate(array, array2, array3, array4, duration));
	}

	private IEnumerator Animate(float[] startAngles0, float[] startAngles1, float[] endAngles0, float[] endAngles1, float duration)
	{
		int count = startAngles0.Length;
		float t = 0f;
		float spacing = ((customSpacing == 0f) ? length : customSpacing);
		do
		{
			t += Time.unscaledDeltaTime;
			float num = 0f;
			Vector2 vector = default(Vector2);
			float t2 = Mathf.Min(t / duration, 1f);
			for (int i = 0; i < count; i++)
			{
				FloorMesh floorMesh = floors[i];
				float num2 = (float)Math.PI / 180f * num;
				floorMesh.transform.localPosition = new Vector3(vector.x + Mathf.Cos(num2) * spacing, vector.y + Mathf.Sin(num2) * spacing, (float)i * 0.01f);
				floorMesh._angle0 = num - 180f;
				floorMesh._angle1 = Mathf.LerpAngle(startAngles1[i], endAngles1[i], t2);
				vector = floorMesh.transform.localPosition;
				if (i != count)
				{
					num = floorMesh._angle1;
				}
			}
			yield return null;
		}
		while (t <= duration);
	}
}
