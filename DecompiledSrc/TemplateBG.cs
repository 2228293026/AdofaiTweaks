using System;
using System.Collections.Generic;
using UnityEngine;

public class TemplateBG : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		mb(1f, FunnyText);
		SortTables();
	}

	private void FunnyText()
	{
		Debug.Log("Yee haw!");
	}
}
