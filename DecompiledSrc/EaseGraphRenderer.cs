using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class EaseGraphRenderer : ADOBase
{
	[Header("Components")]
	public RectTransform rectTransform;

	public UnityEngine.UI.Extensions.UILineRenderer lineRenderer;

	[Header("Settings")]
	public Ease ease = Ease.Linear;

	public int pointCount = 100;

	private TweakableDropdownItem item;

	private Vector2 size => rectTransform.sizeDelta;

	private void Awake()
	{
		item = GetComponentInParent<TweakableDropdownItem>();
		if (Enum.TryParse<Ease>(item.value, out ease))
		{
			DrawGraph();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void DrawGraph()
	{
		List<Vector2> list = new List<Vector2>();
		Dictionary<float, float> dictionary = new Dictionary<float, float>();
		float num = 0f;
		for (int i = 0; i < pointCount; i++)
		{
			float num2 = (float)i / (float)(pointCount - 1);
			float num3 = DOVirtual.EasedValue(0f, 1f, num2, ease);
			dictionary.Add(num2, num3);
			if (num3 > num)
			{
				num = num3;
			}
		}
		num *= size.y;
		float num4 = ((num > size.y) ? (size.y / num) : 1f);
		foreach (KeyValuePair<float, float> item in dictionary)
		{
			float key = item.Key;
			float value = item.Value;
			Vector2 vector = new Vector2(key * size.x, value * size.y * num4);
			list.Add(vector);
		}
		lineRenderer.Points = list.ToArray();
	}
}
