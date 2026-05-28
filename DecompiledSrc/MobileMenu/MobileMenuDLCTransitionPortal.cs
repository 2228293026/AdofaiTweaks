using DG.Tweening;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuDLCTransitionPortal : ADOBase
{
	public Transform portalContainer;

	public MobileMenuDLCTransitionScreen screen;

	[Header("Neo Cosmos references")]
	public ParticleSystem screenParticlesNeoCosmos;

	public ParticleSystem portalParticlesNeoCosmos;

	public SpriteRenderer portalGlowNeoCosmos;

	[Header("ADOFAI references")]
	public ParticleSystem screenParticlesADOFAI;

	public ParticleSystem portalParticlesADOFAI;

	public SpriteRenderer portalGlowADOFAI;

	[Header("Shared references")]
	public SpriteRenderer screenGlow;

	public SpriteRenderer screenGlowCenter;

	public SpriteRenderer portalCenter;

	public Gradient portalGradient;

	public SpinningPlanets planets;

	public SpriteRenderer fader;

	private MobileMenuScreen titleScreen;

	private ParticleSystem screenParticles;

	private ParticleSystem portalParticles;

	private SpriteRenderer portalGlow;

	private bool entering;

	private bool isMainMenu => ADOBase.sceneName == "scnMobileMenu";

	private void Awake()
	{
		if (ADOBase.isExpo || GCS.FOOL_JOKER)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void EnterPortal(bool toNeo)
	{
		entering = true;
		MobileMenuController.instance.Enable(enable: false);
		float duration = 0.5f;
		portalCenter.DOFade(1f, duration);
		portalGlow.DOFade(1f, duration);
		portalParticles.GetComponent<ParticleSystemRenderer>().material.DOFade(1f, duration);
		screenGlowCenter.DOFade(0f, duration);
		screenGlow.DOFade(0f, duration);
		screenGlow.transform.DOScale(screenGlow.transform.localScale / 2f, duration);
		screenGlowCenter.transform.DOScale(screenGlowCenter.transform.localScale / 2f, duration);
		screenParticles.transform.DOScale(screenParticles.transform.localScale / 2f, duration);
		screenParticles.GetComponent<ParticleSystemRenderer>().material.DOFade(0f, duration);
		scrSfx.instance.PlaySfx(SfxSound.EnterNeoCosmos, MixerGroup.InterfaceParent);
		planets.gameObject.SetActive(value: true);
		planets.planets[0].LoadPlanetColor(isRed: true);
		planets.planets[1].LoadPlanetColor(isRed: false);
		float num = 1f;
		float num2 = 1f;
		Camera.main.DOOrthoSize(0.025f, num).SetDelay(num2).SetEase(Ease.InBack);
		DOTween.To(() => planets.radius, delegate(float x)
		{
			planets.radius = x;
		}, 0f, 2f);
		DOTween.To(() => planets.speed, delegate(float x)
		{
			planets.speed = x;
		}, planets.speed * 4f, 2f).SetEase(Ease.InSine);
		planets.planets[0].transform.DOScale(0f, 2f).SetEase(Ease.InSine);
		planets.planets[1].transform.DOScale(0f, 2f).SetEase(Ease.InSine);
		Portal destination = (toNeo ? Portal.TaroDLCMap : Portal.TaroDLCMapExit);
		fader.DOFade(1f, 0.75f).SetDelay(num2 + num - 0.33f).OnComplete(delegate
		{
			DOVirtual.DelayedCall(0.15f, delegate
			{
				ADOBase.controller.PortalTravelAction(destination);
			});
		});
	}

	private void Start()
	{
		titleScreen = FindTitleScreen();
		screenParticles = (isMainMenu ? screenParticlesNeoCosmos : screenParticlesADOFAI);
		portalParticles = (isMainMenu ? portalParticlesNeoCosmos : portalParticlesADOFAI);
		portalGlow = (isMainMenu ? portalGlowNeoCosmos : portalGlowADOFAI);
		portalParticlesADOFAI.transform.parent.gameObject.SetActive(!isMainMenu);
		portalParticlesNeoCosmos.transform.parent.gameObject.SetActive(isMainMenu);
		if (Persistence.GetOverallProgressStage() >= 1)
		{
			screenParticles.Play();
			portalParticles.Play();
		}
		else
		{
			screenGlow.transform.parent.gameObject.SetActive(value: false);
		}
		portalCenter.color = portalCenter.color.WithAlpha(0f);
		portalGlow.color = portalGlow.color.WithAlpha(0f);
		planets.gameObject.SetActive(value: false);
		Material material = portalParticles.GetComponent<ParticleSystemRenderer>().material;
		material.color = material.color.WithAlpha(0f);
	}

	private void Update()
	{
		if (!entering)
		{
			UpdateGlowPortal();
		}
	}

	private void UpdateGlowPortal()
	{
		if (Persistence.GetOverallProgressStage() >= 1)
		{
			float time = Mathf.Repeat(Time.time / 4f, 1f);
			float alpha = Mathf.Lerp(0.05f, 0.4f, (Time.time * 1.4f).NormalizedSin());
			screenGlow.color = portalGradient.Evaluate(time).WithAlpha(alpha);
			screenGlowCenter.color = (portalGradient.Evaluate(time) * 0.7f).WithAlpha(0.9f);
		}
	}

	private void LateUpdate()
	{
		if (!entering)
		{
			MobileMenuScreen mobileMenuScreen = FindScreenBelow();
			bool flag = mobileMenuScreen != titleScreen;
			Camera camobj = ADOBase.controller.camy.camobj;
			float y = titleScreen.transform.position.y;
			float max = mobileMenuScreen.transform.position.y;
			if (flag)
			{
				max = float.MaxValue;
			}
			float num = Mathf.Clamp(camobj.transform.position.y, y, max);
			if (flag)
			{
				num = (num - y) * 1.25f + y;
			}
			portalContainer.MoveY(num);
			screen.parentGroup.inaccessible = flag;
		}
	}

	private MobileMenuScreen FindTitleScreen()
	{
		MobileMenuGroup mobileMenuGroup = screen.parentGroup;
		MobileMenuGroup value;
		while (mobileMenuGroup.linkedGroup.TryGetValue(MoveDirection.Down, out value))
		{
			foreach (MobileMenuScreen visibleScreen in value.visibleScreens)
			{
				if ((isMainMenu && ADOBase.isSwitch) ? (visibleScreen is MobileMenuMoreScreen) : (visibleScreen is MobileMenuTitleScreen))
				{
					return visibleScreen;
				}
			}
			mobileMenuGroup = value;
		}
		return null;
	}

	private MobileMenuScreen FindScreenBelow()
	{
		return screen.parentGroup.linkedGroup[MoveDirection.Down][0];
	}
}
