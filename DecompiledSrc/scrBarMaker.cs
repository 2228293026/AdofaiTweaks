using System.Collections.Generic;
using UnityEngine;

public class scrBarMaker : MonoBehaviour
{
	public enum BarStatus
	{
		idle,
		hit,
		miss,
		special
	}

	public scrController controller;

	public List<scrBgbarnew> listBgbars = new List<scrBgbarnew>();

	public List<GameObject> listObjbars = new List<GameObject>();

	public GameObject objBar;

	public Camera staticcam;

	public float barwidth = 1f;

	public float barborder;

	public bool horizontalplacement;

	private int minfreq = 50;

	private int maxfreq = 1000;

	public float[] spectrum = new float[1024];

	public AudioSource conductoraudio;

	private void Start()
	{
		conductoraudio = scrConductor.instance.GetComponent<AudioSource>();
		controller = GameObject.Find("Controller").GetComponent<scrController>();
		staticcam = controller.camy.Bgcamstatic;
		float num = staticcam.orthographicSize * staticcam.aspect * 2f;
		float num2 = (0f - num) / 2f + barwidth / 2f;
		int num3 = Mathf.FloorToInt(num / barwidth) + 1;
		for (int i = 0; i < num3; i++)
		{
			new Vector3(num2 + (float)i * barwidth, 0f, 0f);
			GameObject gameObject = Object.Instantiate(objBar);
			scrBgbarnew component = gameObject.GetComponent<scrBgbarnew>();
			listBgbars.Add(component);
			listObjbars.Add(gameObject);
			gameObject.SetActive(value: true);
			float x = num2 + (float)i * barwidth;
			gameObject.transform.position = scrMisc.setX(gameObject.transform.position, x);
			gameObject.transform.position = scrMisc.setY(gameObject.transform.position, base.transform.position.y);
			gameObject.GetComponent<scrParallax>().SetNewStartPosition(gameObject.transform.position);
			gameObject.transform.localScale = scrMisc.setX(gameObject.transform.localScale, barwidth - barborder);
			gameObject.GetComponent<scrParallax>().enabled = true;
			gameObject.transform.parent = base.gameObject.transform;
		}
		for (int j = 0; j < num3; j++)
		{
			listBgbars[j].freq = Mathf.RoundToInt((float)j / (float)num3 * (float)(maxfreq - minfreq) + (float)minfreq + 20f);
			listBgbars[j].barmaker = this;
		}
		objBar.SetActive(value: false);
	}

	private void Update()
	{
		if (!GCS.webVersion)
		{
			conductoraudio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		}
	}

	public void Flash(BarStatus status)
	{
		Color start = default(Color);
		ColourScheme currentColourScheme = scrVfx.instance.currentColourScheme;
		switch (status)
		{
		case BarStatus.hit:
			start = currentColourScheme.colourBarsHit;
			break;
		case BarStatus.miss:
			start = currentColourScheme.colourBarsMiss;
			break;
		}
		Color colourBarsIdle = currentColourScheme.colourBarsIdle;
		for (int i = 0; i < listBgbars.Count; i++)
		{
			listBgbars[i].Flash(start, colourBarsIdle);
		}
	}

	public void Damage()
	{
		Flash(BarStatus.miss);
	}

	public float GetFailMultiplier()
	{
		return (controller?.playerOne?.failBar.overloadCounter).GetValueOrDefault() * 10f;
	}
}
