using DG.Tweening;
using UnityEngine;

public class ffxMDGhosts : ffxPlusBase
{
	private GameObject ghostRingPrefab;

	private GameObject ghostPopupPrefab;

	private GameObject ghostPopupPrefabL;

	private GameObject batPlanetMimic;

	private GameObject batPlanetMimicMoving;

	private GameObject batMimicBlue;

	private GameObject batMimicRed;

	public string prefabType = "ring";

	public float xOffset;

	public float yOffset;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		if (ADOBase.controller.visualQuality == VisualQuality.High)
		{
			ghostRingPrefab = ADOBase.gc.prefab_MD_ghostRing;
			ghostPopupPrefab = ADOBase.gc.prefab_MD_ghostPopup;
			ghostPopupPrefabL = ADOBase.gc.prefab_MD_ghostPopupL;
			batPlanetMimic = Resources.Load<GameObject>("PublicPrefabs/BatPlanetMimic");
			batMimicBlue = Resources.Load<GameObject>("PublicPrefabs/BatMimicBlue");
			batMimicRed = Resources.Load<GameObject>("PublicPrefabs/BatMimicRed");
			batPlanetMimicMoving = Resources.Load<GameObject>("PublicPrefabs/BatPlanetMimicMoving");
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		Vector3 vector = new Vector3(xOffset, yOffset, 0f);
		Vector3 position = base.transform.localPosition + vector;
		switch (prefabType)
		{
		case "popup":
			_ = Object.Instantiate(ghostPopupPrefab, position, base.transform.rotation).transform;
			return;
		case "popupL":
			_ = Object.Instantiate(ghostPopupPrefabL, position, base.transform.rotation).transform;
			return;
		case "mimic":
			_ = Object.Instantiate(batPlanetMimic, position, base.transform.rotation).transform;
			return;
		case "mimicM":
			_ = Object.Instantiate(batPlanetMimicMoving, position, base.transform.rotation).transform;
			return;
		case "mimicP":
			_ = Object.Instantiate(batMimicBlue, position, base.transform.rotation).transform;
			_ = Object.Instantiate(batMimicRed, position, base.transform.rotation).transform;
			return;
		}
		float pitch = scrConductor.instance.song.pitch;
		Ease ease = Ease.OutSine;
		Ease ease2 = Ease.OutExpo;
		float num = 1.8f;
		float z = 250f;
		Transform obj = Object.Instantiate(ghostRingPrefab, position, base.transform.rotation).transform;
		obj.ScaleXY(0f, 0f);
		obj.DOScale(Vector2.one * num, pitch * 2f).SetEase(ease2);
		obj.DORotate(Vector3.zero.WithZ(z), pitch, RotateMode.LocalAxisAdd).SetEase(ease);
		SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
		component.color = component.color.WithAlpha(0f);
		component.DOFade(1f, pitch / 3f).SetEase(Ease.Linear);
		component.DOFade(0f, pitch / 4f).SetEase(Ease.Linear).SetDelay(pitch * 2f / 4f);
	}
}
