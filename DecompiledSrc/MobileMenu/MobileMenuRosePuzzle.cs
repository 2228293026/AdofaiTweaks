using System;
using DG.Tweening;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuRosePuzzle : ADOBase
{
	public MobileMenuController menuController;

	public Transform hareContainer;

	public MobileMenuHare[] hares;

	public MobileMenuGrabbableRose[] roses;

	public Transform[] roseContainers;

	private int roseIndex;

	private int stage;

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
		bool unlockedXR = Persistence.unlockedXR;
		if (Persistence.IsWorldComplete(ADOBase.worldData["XH"].index) && !unlockedXR)
		{
			base.gameObject.SetActive(value: true);
			MobileMenuGroup mobileMenuGroup = menuController.map.groupLUT["mainGroup"];
			Transform[] array = roseContainers;
			foreach (Transform obj in array)
			{
				MobileMenuScreen mobileMenuScreen = mobileMenuGroup[UnityEngine.Random.Range(1, mobileMenuGroup.visibleScreens.Count)];
				obj.SetParent(mobileMenuScreen.transform, worldPositionStays: false);
				obj.localScale = Vector2.one / mobileMenuScreen.transform.localScale;
			}
			hareContainer.SetParent(ADOBase.controller.camy.camobj.transform, worldPositionStays: false);
			MobileMenuGrabController grabController = menuController.grabController;
			grabController.onGrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onGrab, new Action<MobileMenuGrabbable>(OnGrab));
			grabController.onUngrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onUngrab, new Action<MobileMenuGrabbable>(OnUngrab));
		}
	}

	private void OnGrab(MobileMenuGrabbable obj)
	{
		if (obj is MobileMenuGrabbableRose)
		{
			MobileMenuGrabbable[] array = roses;
			roseIndex = Array.IndexOf(array, obj);
			hares[roseIndex].Show(show: true);
		}
	}

	private void OnUngrab(MobileMenuGrabbable obj)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (obj is MobileMenuGrabbableRose)
		{
			Collider2D component = obj.GetComponent<Collider2D>();
			Transform spriteTransform = hares[roseIndex].spriteTransform;
			Collider2D component2 = spriteTransform.GetComponent<Collider2D>();
			ColliderDistance2D val = component.Distance(component2);
			if (((ColliderDistance2D)(ref val)).isOverlapped)
			{
				obj.DOKill();
				obj.grabbable = false;
				obj.transform.SetParent(spriteTransform.transform, worldPositionStays: true);
				AdvanceStage();
			}
			hares[roseIndex].Show(show: false);
		}
	}

	private void UnlockXR()
	{
		scrFlash.Flash();
		Persistence.unlockedXR = true;
		menuController.map.portalLUT["XR"].visible = true;
		menuController.map.Build(instantiate: true);
	}

	private void AdvanceStage()
	{
		MobileMenuController.PlayPuzzleSfx((stage == 0) ? SfxSound.MobileMenuXR1 : ((stage == 1) ? SfxSound.MobileMenuXR2 : SfxSound.MobileMenuXR3));
		if (stage == roses.Length - 1)
		{
			DOVirtual.DelayedCall(2.6f, UnlockXR);
		}
		stage++;
	}
}
