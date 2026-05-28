using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMedal : MonoBehaviour
{
	public List<Sprite> medalSprites;

	public Image medalFront;

	public Image medalBack;

	public int id;

	private void Awake()
	{
		medalFront.alphaHitTestMinimumThreshold = 0.5f;
		medalBack.alphaHitTestMinimumThreshold = 0.5f;
	}

	private void Update()
	{
		float t = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) / 2f;
		float alpha = Mathf.Lerp(0.75f, 1f, t);
		medalBack.color = medalBack.color.WithAlpha(alpha);
	}

	public void SetState(int state)
	{
		medalFront.enabled = true;
		medalFront.sprite = medalSprites[state];
	}

	public void TintBack(Color c)
	{
		medalBack.color = c;
	}

	public void OnClick()
	{
		scrController.instance.pauseMenu.ChangeMedalFixed(id);
	}
}
