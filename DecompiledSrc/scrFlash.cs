using UnityEngine;

public class scrFlash : MonoBehaviour
{
	public static Color colorStart = Color.white;

	public static Color colorEnd = Color.clear;

	public static float colortimer = 0f;

	public static float colorduration = 0.4f;

	public static float defaultcolorduration = 0.4f;

	public static bool damagemode;

	public scrController controller;

	private Renderer flashRenderer;

	private void Start()
	{
		colorStart = Color.white;
		colorEnd = Color.clear;
		colortimer = 0f;
		colorduration = 0.4f;
		flashRenderer = GetComponent<Renderer>();
		flashRenderer.material.color = Color.clear;
		controller = GameObject.Find("Controller").GetComponent<scrController>();
		colortimer = colorduration;
	}

	public void Update()
	{
		colortimer += Time.deltaTime;
		flashRenderer.material.color = Color.Lerp(colorStart, colorEnd, colortimer / colorduration);
	}

	public static void Flash(Color? _colorStart = null, float duration = -1f)
	{
		if (!_colorStart.HasValue)
		{
			_colorStart = Color.white.WithAlpha(0.5f);
		}
		if (duration == -1f)
		{
			duration = defaultcolorduration;
		}
		colorStart = _colorStart.Value;
		colorEnd = Color.clear;
		colortimer = 0f;
		colorduration = duration;
	}

	public static void FlashReverse(Color _colorEnd, float timeincrotchets)
	{
		colorStart = colorEnd;
		colorEnd = _colorEnd;
		colortimer = 0f;
		colorduration = (float)((double)timeincrotchets * scrConductor.instance.crotchetAtStart);
	}

	public static void FlashKill()
	{
		colortimer = 0f;
		colorStart = Color.clear;
		colorEnd = Color.clear;
		colorduration = defaultcolorduration;
	}

	public static void FlashBlackStay()
	{
		colorStart = Color.black;
		colorEnd = Color.black;
	}

	public static void FlashEx(Color _colorStart, Color _colorEnd, float duration = -1f)
	{
		if (duration == -1f)
		{
			duration = defaultcolorduration;
		}
		colorStart = _colorStart;
		colorEnd = _colorEnd;
		colortimer = 0f;
		colorduration = duration;
	}

	public static void OnDamage()
	{
		colortimer = colorduration / 2f;
		colorStart = Color.red.WithAlpha(0.5f);
	}

	public static void OffDamage()
	{
		damagemode = false;
		Flash(Color.green);
	}
}
