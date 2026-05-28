using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuPigPuzzle : ADOBase
{
	public MobileMenuController menuController;

	public MobileMenuGrabbablePig pig;

	public Transform[] fanfarePigs;

	public Transform fanfarePigsTarget;

	public Transform container;

	private bool holdingDown;

	private float touchStartTime;

	private bool movingFanfare;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (GCS.FOOL_JOKER || ADOBase.isExpo)
		{
			return;
		}
		bool unlockedXF = Persistence.unlockedXF;
		if (Persistence.IsWorldComplete(ADOBase.worldData["6"].index) && !unlockedXF)
		{
			base.gameObject.SetActive(value: true);
			MobileMenuScreen mobileMenuScreen = menuController.map.rootGroup[0];
			container.SetParent(mobileMenuScreen.transform, worldPositionStays: false);
			Transform[] array = fanfarePigs;
			foreach (Transform transform in array)
			{
				Vector3 vector = fanfarePigsTarget.position - transform.position;
				float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				transform.gameObject.SetActive(value: false);
			}
			MobileMenuGrabController grabController = menuController.grabController;
			grabController.onGrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onGrab, new Action<MobileMenuGrabbable>(OnGrab));
			grabController.onUngrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onUngrab, new Action<MobileMenuGrabbable>(OnUngrab));
		}
	}

	private void Update()
	{
		if (movingFanfare)
		{
			Transform[] array = fanfarePigs;
			foreach (Transform transform in array)
			{
				transform.position += transform.right * Time.deltaTime * 20f;
				transform.gameObject.SetActive(value: true);
			}
		}
	}

	private void OnGrab(MobileMenuGrabbable obj)
	{
		if (!(obj != pig))
		{
			touchStartTime = Time.timeSinceLevelLoad;
			pig.grabbable = false;
			pig.transform.DOScale(0.9f, 0.2f);
			holdingDown = true;
		}
	}

	private void OnUngrab(MobileMenuGrabbable obj)
	{
		if (!holdingDown)
		{
			return;
		}
		float num = Mathf.Clamp01((Time.timeSinceLevelLoad - touchStartTime) / 2f);
		Camera camobj = scrController.instance.camy.camobj;
		float num2 = 2f * camobj.orthographicSize * camobj.aspect;
		float endValue = num * num2;
		TweenerCore<Vector3, Vector3, VectorOptions> t = pig.transform.DOMoveX(endValue, 0.5f).SetRelative(isRelative: true).OnComplete(delegate
		{
			pig.grabbable = true;
		});
		if (num >= 1f)
		{
			t.SetEase(Ease.Linear).OnComplete(delegate
			{
				pig.gameObject.SetActive(value: false);
			});
			DoFanfare();
		}
		else
		{
			t.SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
		}
		t.Play();
		pig.transform.DOScale(1f, 0.2f);
		holdingDown = false;
	}

	private void DoFanfare()
	{
		MobileMenuController.PlayPuzzleSfx(SfxSound.MobileMenuXF);
		DOVirtual.DelayedCall(1f, delegate
		{
			movingFanfare = true;
		});
		DOVirtual.DelayedCall(1.7f, UnlockXF);
	}

	private void UnlockXF()
	{
		scrFlash.Flash();
		Persistence.unlockedXF = true;
		menuController.map.portalLUT["XF"].visible = true;
		menuController.map.Build(instantiate: true);
	}
}
