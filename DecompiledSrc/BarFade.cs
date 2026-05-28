using UnityEngine;

public class BarFade : MonoBehaviour
{
	public SpriteRenderer render;

	private Transform ap;

	private void Start()
	{
		ap = NewLife.instance.barAlpha.transform;
	}

	private void Update()
	{
		render = base.gameObject.GetComponent<SpriteRenderer>();
		float x = ap.position.x;
		render.material.SetColor("_Color", new Color(1f, 1f, 1f, x));
	}
}
