using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class scrEnableIfCNY : ADOBase
{
	private string[] animals = new string[12]
	{
		"monkey", "rooster", "dog", "pig", "rat", "ox", "tiger", "rabbit", "dragon", "snake",
		"horse", "goat"
	};

	private void OnEnable()
	{
		if (ADOBase.IsCNY())
		{
			int year = DateTime.Now.Year;
			string text = animals[year % 12];
			Sprite sprite = Resources.Load<Sprite>("CNY Constellations/" + text);
			GetComponent<SpriteRenderer>().sprite = sprite;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
