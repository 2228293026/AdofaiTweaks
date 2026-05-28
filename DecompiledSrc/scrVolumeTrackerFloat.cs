using System;
using UnityEngine;

public class scrVolumeTrackerFloat : MonoBehaviour
{
	public int freq = 20;

	[NonSerialized]
	public float output = 1f;

	public AudioSource conductoraudio;

	private float[] spectrum = new float[1024];

	public float maxmultiplier = 1.2f;

	public float minmultiplier = 1f;

	private void Start()
	{
		conductoraudio = scrConductor.instance.gameObject.GetComponent<AudioSource>();
	}

	private void Update()
	{
		conductoraudio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		output = Mathf.Log(spectrum[freq]) / 10f * (maxmultiplier - minmultiplier) + minmultiplier;
	}
}
