using CrazyMinnow.AmplitudeWebGL;
using UnityEngine;

public class scrBgbar : MonoBehaviour
{
	private Vector3 oriscale;

	private AudioSource audioSource;

	private float[] spectrum = new float[1024];

	public int freq;

	private Amplitude amplitude;

	public int index;

	public float heightMultiplier = 1000f;

	public float failHeightMultiplier = 20f;

	private scrBackgroundBars bgbars;

	private Color colorStart = Color.white;

	private Color colorEnd = Color.clear;

	public float colortimer;

	public float colorduration = 0.4f;

	private void Awake()
	{
		audioSource = scrConductor.instance.GetComponent<AudioSource>();
		amplitude = scrConductor.instance.GetComponent<Amplitude>();
	}

	private void Start()
	{
		bgbars = GetComponentInParent<scrBackgroundBars>();
		oriscale = base.transform.localScale;
		freq = Mathf.RoundToInt(base.transform.localPosition.x / 18f * 600f + 20f);
		GetComponent<Renderer>().material.color = colorEnd;
		audioSource = scrConductor.instance.GetComponent<AudioSource>();
		amplitude = scrConductor.instance.GetComponent<Amplitude>();
	}

	private void Update()
	{
		Renderer component = GetComponent<Renderer>();
		component.material.color = colorEnd;
		base.transform.localScale = new Vector3(oriscale.x, base.transform.localScale.y * 0.9f, oriscale.z);
		float a = Mathf.Max(b: amplitude.sample[index] * heightMultiplier * (bgbars.GetFailMultiplier() + 1f), a: base.transform.localScale.y - oriscale.y);
		a = Mathf.Max(a, 0f);
		base.transform.localScale = new Vector3(oriscale.x, a + oriscale.y, oriscale.z);
		colortimer += Time.deltaTime;
		component.material.color = Color.Lerp(colorStart, colorEnd, colortimer / colorduration);
	}

	public void Flash(Color start, Color end)
	{
		colorStart = start;
		colorEnd = end;
		colortimer = 0f;
	}
}
