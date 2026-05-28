using UnityEngine;

public class scrBgbarnew : MonoBehaviour
{
	public scrBarMaker barmaker;

	public int freq;

	public Vector3 oriscale;

	public AudioSource conductoraudio;

	private float[] spectrum = new float[1024];

	public float maxheight = 5f;

	public float minheight = 2f;

	public float runningmax;

	private Color colorStart = Color.white;

	private Color colorEnd = Color.clear;

	public float colortimer;

	public float colorduration = 0.4f;

	private void Start()
	{
		oriscale = base.transform.localScale;
		conductoraudio = scrConductor.instance.GetComponent<AudioSource>();
	}

	private void Update()
	{
		float num = barmaker.spectrum[freq];
		if (num == 0f)
		{
			base.transform.localScale = Vector3.zero;
			return;
		}
		runningmax = Mathf.Max(runningmax, num);
		base.transform.localScale = new Vector3(oriscale.x, base.transform.localScale.y * 0.96f, oriscale.z);
		float num2 = Mathf.Max(base.transform.localScale.y - minheight, num / runningmax * maxheight);
		base.transform.localScale = new Vector3(oriscale.x, num2 + minheight, oriscale.z);
	}

	public void Flash(Color start, Color end)
	{
	}
}
