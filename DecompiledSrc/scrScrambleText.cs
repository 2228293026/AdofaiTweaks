using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class scrScrambleText : ADOBase
{
	private RectTransform rectTransform;

	public TMP_Text text;

	private float currTimer;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		char[] array = new char[2000];
		for (int i = 0; i < 2000; i++)
		{
			int value = UnityEngine.Random.Range(33, 127);
			array[i] = Convert.ToChar(value);
		}
		text.text = new string(array);
	}

	private void Update()
	{
		if (ADOBase.controller.visualEffects != VisualEffects.Minimum)
		{
			ChangeText();
		}
	}

	private void ChangeText()
	{
		currTimer += Time.deltaTime;
		if (currTimer >= 1f / 60f)
		{
			float y = UnityEngine.Random.Range(0f, 40f);
			rectTransform.AnchorPosY(y);
			currTimer = 0f;
		}
	}
}
