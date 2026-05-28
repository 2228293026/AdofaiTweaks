using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BBManager : ADOBase
{
	public RectTransform levelNameBackground;

	public RectTransform countdownBackground;

	public PathGeneratorOld pathGenerator;

	public MeshRenderer[] lavaFloors;

	public Material floorGlow;

	public Image flash;

	public Font font;

	public Texture greenLava;

	private bool levelNameShowing;

	private bool countdownShowing;

	private static BBManager _instance;

	public static BBManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindAnyObjectByType<BBManager>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		GCS.bb = true;
	}

	private void Start()
	{
		ShowLevelName(show: true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			DoFancyEffect();
		}
	}

	public void ShowLevelName(bool show)
	{
		if (levelNameShowing != show)
		{
			levelNameShowing = show;
			float endValue = (show ? 4f : 450f);
			levelNameBackground.DOAnchorPosX(endValue, 0.2f).SetUpdate(isIndependentUpdate: true);
			if (!show)
			{
				Image component = levelNameBackground.GetComponent<Image>();
				Color color = component.color;
				component.color = new Color(0.5f, 0.5f, 0.5f, color.a);
				component.DOColor(color, 0.2f).SetEase(Ease.OutQuad);
			}
		}
	}

	public void ShowCountdown(bool show)
	{
		if (countdownShowing != show)
		{
			countdownShowing = show;
			float endValue = (show ? 4f : 350f);
			countdownBackground.DOAnchorPosX(endValue, 0.2f).SetUpdate(isIndependentUpdate: true);
		}
	}

	public void LightUpFloor(int index)
	{
		FloorMeshOld floorMeshOld = pathGenerator.floors[index];
		floorMeshOld.material = floorGlow;
		floorMeshOld.UpdateMaterial();
		ShortcutExtensions.DOScale(endValue: new Vector3(1f, 1.6139f, 0.07f), target: floorMeshOld.transform, duration: 0.05f).SetLoops(2, LoopType.Yoyo);
		if (index == 33)
		{
			DoFancyEffect();
		}
	}

	private void DoFancyEffect()
	{
		flash.color = Color.white;
		flash.DOColor(Color.white.WithAlpha(0f), 0.3f);
		MeshRenderer[] array = lavaFloors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetTexture("_LavaTex", greenLava);
		}
	}
}
