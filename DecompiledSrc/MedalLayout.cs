using System.Collections.Generic;
using UnityEngine;

public class MedalLayout : MonoBehaviour
{
	public float medalSpacing;

	public GameObject medalPrefab;

	public List<Mawaru_Medal> medals;

	public void Generate(int medals)
	{
		this.medals = new List<Mawaru_Medal>();
		if (medals <= 8)
		{
			Vector2 vector = new Vector2((float)(medals - 1) * 0.5f / 2f, -0.25f);
			for (int i = 0; i < medals; i++)
			{
				bool flag = i % 2 == 1;
				Transform transform = Object.Instantiate(medalPrefab, base.transform).transform;
				this.medals.Add(transform.GetComponent<Mawaru_Medal>());
				transform.localPosition = new Vector2((float)i * medalSpacing * 0.5f, (flag ? (-0.5f) : 0f) * medalSpacing) - vector * medalSpacing;
			}
			return;
		}
		float num = (float)medals / 3f;
		int num2 = Mathf.FloorToInt(num);
		bool flag2 = num % 1f < 0.5f;
		Vector2 vector2 = new Vector2((float)num2 / 2f, -0.25f);
		for (int j = 0; j < 3; j++)
		{
			int num3 = num2;
			bool flag3 = j % 2 == 1;
			if (flag3 == flag2 || num % 1f == 0f)
			{
				num3++;
			}
			for (int k = 0; k < num3; k++)
			{
				Transform transform2 = Object.Instantiate(medalPrefab, base.transform).transform;
				this.medals.Add(transform2.GetComponent<Mawaru_Medal>());
				transform2.localPosition = new Vector2((float)k * medalSpacing + medalSpacing * ((flag3 == flag2) ? 0f : 0.5f), (float)j * medalSpacing * -0.5f) - vector2 * medalSpacing;
			}
		}
	}
}
