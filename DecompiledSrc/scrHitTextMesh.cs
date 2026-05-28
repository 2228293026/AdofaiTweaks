using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class scrHitTextMesh : ADOBase
{
	private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

	[NonSerialized]
	public bool dead;

	[NonSerialized]
	public TextMeshPro text;

	private CanvasGroup sf;

	private DOTweenAnimation anim;

	public HitMargin hitMargin;

	private float timer;

	private int frameShown;

	private bool forceOnScreen;

	private float minBorderDistance;

	private Camera gameCam;

	private Vector3 textPos;

	private Renderer meshRenderer;

	private Vector3 borderOffset;

	private Color lastColor;

	public float startingSize;

	public float sizeUp;

	public float duration;

	public int vibrato;

	public float elasticity;

	public void Init(HitMargin hitMargin)
	{
		this.hitMargin = hitMargin;
		base.gameObject.SetActive(value: false);
		text = GetComponent<TextMeshPro>();
		meshRenderer = GetComponent<Renderer>();
		text.SetLocalizedFont();
		if (RDString.language != SystemLanguage.Korean && text.font == RDConstants.data.latinFont)
		{
			text.fontSize = Mathf.RoundToInt(text.fontSize * 1.1f);
		}
		dead = true;
		ColourSchemeHitMargin hitMarginColours = RDConstants.data.hitMarginColours;
		text.text = RDString.Get("HitMargin." + hitMargin);
		TextMeshPro textMeshPro = text;
		Color color = (textMeshPro.color = hitMargin switch
		{
			HitMargin.TooEarly => hitMarginColours.colourTooEarly, 
			HitMargin.VeryEarly => hitMarginColours.colourVeryEarly, 
			HitMargin.EarlyPerfect => hitMarginColours.colourLittleEarly, 
			HitMargin.Perfect => hitMarginColours.colourPerfect, 
			HitMargin.LatePerfect => hitMarginColours.colourLittleLate, 
			HitMargin.VeryLate => hitMarginColours.colourVeryLate, 
			HitMargin.TooLate => hitMarginColours.colourTooLate, 
			HitMargin.Multipress => hitMarginColours.colourMultipress, 
			HitMargin.FailMiss => hitMarginColours.colourFail, 
			HitMargin.FailOverload => hitMarginColours.colourFail, 
			HitMargin.OverPress => hitMarginColours.colourFail, 
			_ => Color.gray, 
		});
		lastColor = color;
		scrController instance = scrController.instance;
		gameCam = instance.camy.GetComponent<Camera>();
		forceOnScreen = instance.forceHitTextOnScreen;
		minBorderDistance = instance.hitTextMinBorderDistance;
		meshRenderer.sortingOrder = 100;
	}

	public void Show(Vector3 position, Vector3 borderOffset = default(Vector3), float missAngle = 0f, float scale = 1f, float angle = 0f, float fadeSpeed = 1f)
	{
		frameShown = Time.frameCount;
		timer = 0f;
		base.transform.localPosition = position;
		base.transform.gameObject.SetActive(value: true);
		dead = false;
		text.DOKill();
		text.color = lastColor;
		text.fontMaterial.SetColor(GlowColor, Color.black);
		text.DOFade(0f, 0.7f / fadeSpeed).SetDelay(0.5f / fadeSpeed).SetEase(Ease.OutQuad)
			.OnComplete(delegate
			{
				text.DOKill();
			});
		scrMisc.Rotate2D(base.transform, scrController.instance.camy.transform.rotation.eulerAngles.z);
		base.transform.DOKill();
		base.transform.localScale = new Vector3(startingSize * scale, startingSize * scale, 1f);
		base.transform.DOPunchScale(new Vector3(sizeUp, sizeUp, 1f), duration, vibrato, elasticity);
		base.transform.localEulerAngles = base.transform.localEulerAngles + Vector3.forward * angle;
		if (hitMargin != HitMargin.Perfect)
		{
			base.transform.DOLocalRotate(new Vector3(0f, 0f, missAngle * 20f), 2f, RotateMode.LocalAxisAdd);
		}
		textPos = position;
		if (scrController.coopMode)
		{
			_ = ADOBase.controller.playerManager.GetActivePlayers().Count;
			this.borderOffset = borderOffset;
		}
	}

	private void Update()
	{
		if (!dead)
		{
			if (forceOnScreen)
			{
				float num = gameCam.orthographicSize * 2f;
				float num2 = num * (float)Screen.width / (float)Screen.height;
				Vector3 position = gameCam.transform.position;
				Vector3 vector = textPos - position;
				Vector3 localPosition = textPos;
				localPosition.x = position.x + Mathf.Clamp(vector.x, (0f - num2) / 2f + minBorderDistance + borderOffset.x, num2 / 2f - minBorderDistance - borderOffset.x);
				localPosition.y = position.y + Mathf.Clamp(vector.y, (0f - num) / 2f + minBorderDistance + borderOffset.y, num / 2f - minBorderDistance - borderOffset.y);
				base.transform.localPosition = localPosition;
			}
			timer += Time.deltaTime;
			if (timer > 1.25f)
			{
				dead = true;
				base.transform.DOKill();
				text.DOKill();
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
