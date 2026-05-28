using UnityEngine;

public class scrOpacityChangeOnBeat : ADOBase
{
	public float[] arrOpacity;

	private int counter;

	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		ADOBase.conductor.onBeats.Add(this);
	}

	public override void OnBeat()
	{
		counter++;
		int num = counter % arrOpacity.Length;
		if (sr != null)
		{
			Color color = sr.color;
			color.a = arrOpacity[num];
			sr.color = color;
		}
	}
}
