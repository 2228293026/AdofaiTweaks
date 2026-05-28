using UnityEngine;

public class ffxMenuFoolSwirl : ffxPlusBase
{
	public ParticleSystem qhar;

	private scrFloor[] floorArray;

	private scrHoldRenderer[] holdArray;

	public override void Awake()
	{
		base.Awake();
		floor.topGlow.gameObject.SetActive(value: false);
		bool flag = false;
		if (GCS.FOOL_SWIRL)
		{
			flag = true;
		}
		if (!ADOBase.IsAprilFools() && !flag)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Start()
	{
		floor.topGlow.gameObject.SetActive(value: false);
		floor.bottomGlow?.gameObject.SetActive(value: false);
		floor.SetIconSprite(ADOBase.gc.sprIconSwirlRed);
		floor.SetIconFlipped(flipped: true);
		floorArray = Object.FindObjectsByType(typeof(scrFloor), FindObjectsSortMode.None) as scrFloor[];
		holdArray = Object.FindObjectsByType(typeof(scrHoldRenderer), FindObjectsSortMode.None) as scrHoldRenderer[];
		if (GCS.FOOL_SWIRL)
		{
			floor.SetIconSprite(ADOBase.gc.sprIconSwirlBlue);
			floor.SetIconFlipped(flipped: false);
			ADOBase.controller.playerOne.planetarySystem.isCW = false;
			scrFloor[] array = floorArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isCCW = true;
			}
			scrHoldRenderer[] array2 = holdArray;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].UpdateFoolDir();
			}
		}
		UpdateEmissionActivity();
		qhar.Play();
	}

	public void Update()
	{
		if (GCS.FOOL_SWIRL)
		{
			base.gameObject.SetActive(value: true);
			qhar.transform.position = scrController.instance.chosenPlanet.transform.position;
		}
		else
		{
			qhar.transform.position = floor.transform.position;
		}
		UpdateEmissionActivity();
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (!GCS.FOOL_SWIRL)
		{
			GCS.FOOL_SWIRL = true;
			floor.SetIconSprite(ADOBase.gc.sprIconSwirlBlue);
			floor.SetIconFlipped(flipped: false);
			ADOBase.controller.playerOne.planetarySystem.isCW = false;
			scrSfx.instance.PlaySfx(SfxSound.YouFool, MixerGroup.SfxParent, 0.5f);
		}
		else
		{
			GCS.FOOL_SWIRL = false;
			floor.SetIconSprite(ADOBase.gc.sprIconSwirlRed);
			floor.SetIconFlipped(flipped: true);
			ADOBase.controller.playerOne.planetarySystem.isCW = true;
			scrSfx.instance.PlaySfx(SfxSound.YouAbsoluteBuffoon, MixerGroup.SfxParent, 0.5f);
		}
		scrFloor[] array = floorArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].isCCW = GCS.FOOL_SWIRL;
		}
		scrHoldRenderer[] array2 = holdArray;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].UpdateFoolDir();
		}
	}

	private void UpdateEmissionActivity()
	{
		if (!(ADOBase.sceneName != "scnLevelSelect"))
		{
			ParticleSystem.EmissionModule emission = qhar.emission;
			emission.enabled = ADOBase.levelSelectBase.menuPhase == 1;
		}
	}
}
