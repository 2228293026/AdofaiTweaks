using UnityEngine;

public class ffxMenuFoolJoker : ffxPlusBase
{
	public ParticleSystem particles;

	public Sprite iconSprite;

	public override void Awake()
	{
		base.Awake();
		bool flag = false;
		if (GCS.FOOL_JOKER)
		{
			flag = true;
		}
		if (!ADOBase.IsAprilFools() && !flag)
		{
			base.gameObject.SetActive(value: false);
		}
		if (GCS.FOOL_JOKER)
		{
			base.gameObject.SetActive(value: true);
			GetComponent<scrDisableIfOverallProgressStage>().enabled = false;
		}
	}

	private void Start()
	{
		floor.SetIconSprite(iconSprite);
		particles.Play();
	}

	private void Update()
	{
		if (GCS.FOOL_JOKER)
		{
			base.gameObject.SetActive(value: true);
			particles.transform.position = scrController.instance.chosenPlanet.transform.position;
		}
		else
		{
			particles.transform.position = floor.transform.position;
		}
		UpdateEmissionActivity();
	}

	public override void StartEffect(scrPlanet planet)
	{
		ADOBase.controller.PortalTravelAction(Portal.FoolJoker);
	}

	private void UpdateEmissionActivity()
	{
		if (!(ADOBase.sceneName != "scnLevelSelect"))
		{
			ParticleSystem.EmissionModule emission = particles.emission;
			emission.enabled = ADOBase.levelSelectBase.menuPhase == 1 || GCS.FOOL_JOKER;
		}
	}
}
