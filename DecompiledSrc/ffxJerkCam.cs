using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ffxJerkCam : ffxPlusBase
{
	public float size;

	public bool shouldSkewAngle;

	private int awakeFrame;

	public override bool runOnHit => true;

	public override void Awake()
	{
		base.Awake();
		disableIfMinFx = true;
		cam.BGcam.GetComponent<Blur>().enabled = true;
		cam.BGcam.GetComponent<Grayscale>().enabled = true;
		cam.Bgcamstatic.GetComponent<Grayscale>().enabled = true;
		awakeFrame = Time.frameCount;
	}

	private void Update()
	{
		if (Time.frameCount - awakeFrame == 1)
		{
			cam.BGcam.GetComponent<Blur>().enabled = false;
			cam.BGcam.GetComponent<Grayscale>().enabled = false;
			cam.Bgcamstatic.GetComponent<Grayscale>().enabled = false;
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (!(planet == null))
		{
			cam.isPulsingOnHit = false;
			cam.isMoveTweening = false;
			cam.setCamSizeInstant(size);
			cam.setCamSizeLerp(size, size - 0.3f, 1f);
			cam.ViewObjectInstant(planet.transform);
			if (shouldSkewAngle)
			{
				float num = 2.5f;
				cam.SetRotAngleLerp(num, 5f, 1f);
			}
			if (ADOBase.controller.visualQuality == VisualQuality.High)
			{
				cam.BGcam.GetComponent<Grayscale>().enabled = true;
				Blur component = cam.BGcam.GetComponent<Blur>();
				component.iterations = 1;
				component.enabled = true;
				cam.Bgcamstatic.GetComponent<Grayscale>().enabled = true;
			}
		}
	}
}
