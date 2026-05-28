using UnityEngine;

public class scrColorChangeOnBeat : ADOBase
{
	public Color[] arrcolors;

	private int currcolor;

	private void Start()
	{
		ADOBase.conductor.onBeats.Add(this);
	}

	public override void OnBeat()
	{
		currcolor = scrMisc.GetNewRandInt(arrcolors.Length - 1, currcolor);
		GetComponent<SpriteRenderer>().color = arrcolors[currcolor];
	}
}
