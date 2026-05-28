using UnityEngine;

public class scrVolumeTrackerFade : MonoBehaviour
{
	public int freq = 20;

	private float orifade;

	public AudioSource conductoraudio;

	private SpriteRenderer spr;

	private float[] spectrum = new float[1024];

	private float maxmultiplier = 1.2f;

	private float minmultiplier = 1f;

	private void Start()
	{
		spr = GetComponent<SpriteRenderer>();
		orifade = spr.color.a;
		conductoraudio = scrConductor.instance.gameObject.GetComponent<AudioSource>();
	}

	private void Update()
	{
		conductoraudio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float num = Mathf.Log(spectrum[freq]) / 10f * (maxmultiplier - minmultiplier) + minmultiplier;
		spr.color = spr.color.WithAlpha(num * orifade);
	}
}
