using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialBackground : ADOBase
{
	public Transform tile;

	public SpriteRenderer tileRenderer;

	public List<Transform> elements;

	public List<SpriteRenderer> elementRenderers;

	public MeshRenderer triangleRenderer;

	public List<Color> elementOriginalColors;

	public Color triangleOriginalColor;

	public BGShapeType bgShapeType;

	private Vector3[] startScales;

	private int lastFloorID;

	private void Awake()
	{
		tile = base.transform.Find("opt_background_tile");
		tileRenderer = tile.GetComponent<SpriteRenderer>();
		elements = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeSelf && item.name.Contains("opt_tutorial"))
			{
				elements.Add(item);
				item.TryGetComponent<SpriteRenderer>(out var component);
				if (component == null)
				{
					triangleRenderer = item.GetComponent<MeshRenderer>();
					triangleOriginalColor = triangleRenderer.material.color;
				}
				else
				{
					elementRenderers.Add(component);
					elementOriginalColors.Add(component.color);
				}
			}
		}
		startScales = new Vector3[elements.Count];
		int num = 0;
		foreach (Transform element in elements)
		{
			startScales[num] = element.localScale;
			num++;
		}
	}

	private void LateUpdate()
	{
		bool flag = ADOBase.controller.state == States.Countdown && Time.frameCount == ADOBase.conductor.onBeatFrame;
		if (!(ADOBase.controller.currentFloorID > lastFloorID || flag))
		{
			return;
		}
		int num = 0;
		foreach (Transform element in elements)
		{
			if (Random.value < 0.3f)
			{
				float num2 = (float)ADOBase.conductor.crotchetAtStart / 2f;
				if (num2 > 0f)
				{
					element.DOPunchScale(startScales[num] * 0.1f, num2, 5);
				}
			}
			num++;
		}
		lastFloorID = ADOBase.controller.currentFloorID;
	}

	public void SetTileColor(Color color)
	{
		tileRenderer.color = color;
	}

	public void SetShapeColor(Color color)
	{
		foreach (SpriteRenderer elementRenderer in elementRenderers)
		{
			elementRenderer.color = color.WithAlpha(elementRenderer.color.a);
		}
		triangleRenderer.material.color = color.WithAlpha(triangleRenderer.material.color.a);
	}

	public void ResetShapeColor()
	{
		for (int i = 0; i < elementRenderers.Count; i++)
		{
			elementRenderers[i].color = elementOriginalColors[i];
		}
		triangleRenderer.material.color = triangleOriginalColor;
	}

	private void SetShapeActive(bool active)
	{
		foreach (SpriteRenderer elementRenderer in elementRenderers)
		{
			elementRenderer.gameObject.SetActive(active);
		}
		triangleRenderer.gameObject.SetActive(active);
	}

	public void SetShapeType(BGShapeType type)
	{
		if (bgShapeType != type)
		{
			if (type == BGShapeType.Default)
			{
				ResetShapeColor();
			}
			SetShapeActive(type != BGShapeType.Disabled);
		}
		bgShapeType = type;
	}
}
