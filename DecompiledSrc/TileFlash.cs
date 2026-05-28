using UnityEngine;

public class TileFlash : MonoBehaviour
{
	private float duration = 0.5f;

	private float time;

	private Color color = Color.white;

	private void Start()
	{
		GetComponent<SpriteRenderer>().sprite = base.transform.parent.GetComponent<SpriteRenderer>().sprite;
		GetComponent<SpriteRenderer>().sortingOrder = base.transform.parent.GetComponent<SpriteRenderer>().sortingOrder;
		Debug.Log("flash!");
		time = duration;
	}

	private void Update()
	{
		if (time > 0f)
		{
			time -= 0.02f;
			float alpha = time / duration;
			GetComponent<SpriteRenderer>().color = color.WithAlpha(alpha);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
