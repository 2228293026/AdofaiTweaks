using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileMenu;

public class CR2024TechSecret : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public MobileMenuController menuController;

	public Image image;

	private Coroutine holdCoroutine;

	private float holdTime = 4f;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		MobileMenuController mobileMenuController = menuController;
		mobileMenuController.onFinishLoading = (Action)Delegate.Combine(mobileMenuController.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		if (ADOBase.isMobile)
		{
			if (Persistence.mobileTechUnlocked || !GCNS.crownWorlds.All(Persistence.IsWorldComplete))
			{
				base.enabled = false;
			}
			image.raycastTarget = true;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		holdCoroutine = StartCoroutine(Hold());
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		CancelHold();
	}

	private IEnumerator Hold()
	{
		yield return new WaitForSeconds(holdTime);
		OnHoldComplete();
	}

	private void CancelHold()
	{
		if (holdCoroutine != null)
		{
			StopCoroutine(holdCoroutine);
			holdCoroutine = null;
		}
	}

	private void OnHoldComplete()
	{
		image.raycastTarget = false;
		base.enabled = false;
		scrFlash.Flash();
		Persistence.mobileTechUnlocked = true;
		foreach (MobileMenuScreen screen in menuController.map.groupLUT["cosmicRadioTech2024Group"].screens)
		{
			screen.visible = true;
		}
		menuController.map.Build(instantiate: true);
	}
}
