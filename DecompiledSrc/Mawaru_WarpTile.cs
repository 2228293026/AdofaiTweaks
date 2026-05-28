using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Mawaru_WarpTile : MonoBehaviour
{
	public Mawaru_Sprite ring;

	public Mawaru_Sprite tile;

	public Mawaru_Sprite shape;

	public Color tint = Color.white;

	public List<Mawaru_Sprite> objectsToFade;

	public void Start()
	{
		ring.render.material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
		tile.render.material.SetColor("_Color", tint);
	}

	public void Trip()
	{
		ring.transform.localScale = Vector3.one;
		ring.transform.DOScale(Vector3.one * 6f, 0.5f).SetEase(Ease.OutExpo);
		ring.render.material.SetColor("_Color", Color.white);
		ring.render.material.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f).SetEase(Ease.Linear);
	}
}
