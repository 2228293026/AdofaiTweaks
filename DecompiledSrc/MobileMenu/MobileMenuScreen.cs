using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu;

public abstract class MobileMenuScreen : ADOClass
{
	public Transform transform;

	public Vector3 transformPosition;

	public Transform transformParent;

	public bool visible = true;

	public string[] visibilityConditions;

	public MobileMenuGroup parentGroup;

	public Action<bool, bool> onSelect;

	public Action<bool> onInteract;

	public static MobileMenuScreen New(string screenType)
	{
		return screenType switch
		{
			"portal" => new MobileMenuPortalScreen(), 
			"title" => new MobileMenuTitleScreen(), 
			"credits" => new MobileMenuCreditsScreen(), 
			"colors" => new MobileMenuColorScreen(), 
			"puzzleRooms" => new PuzzleRoomsScreen(), 
			"blank" => new MobileMenuBlankScreen(), 
			"rift" => new MobileMenuTaroRiftScreen(), 
			"dlcPortal" => new MobileMenuDLCTransitionScreen(), 
			"gallery" => new MobileMenuGalleryScreen(), 
			"description" => new MobileMenuDescriptionScreen(), 
			"more" => new MobileMenuMoreScreen(), 
			"featuredPortal" => new MobileMenuFeaturedPortalScreen(), 
			_ => null, 
		};
	}

	public virtual float GetWidth()
	{
		return GetBaseWidth(parentGroup.zoom);
	}

	public static float GetBaseWidth(float zoom)
	{
		Camera camobj = scrController.instance.camy.camobj;
		return 2f * zoom * camobj.aspect;
	}

	public static float GetAspect()
	{
		return scrController.instance.camy.camobj.aspect;
	}

	public virtual void Select(bool select = true, bool instant = false)
	{
	}

	public virtual void Instantiate()
	{
		onSelect = Select;
		onInteract = Interact;
	}

	public virtual string GetDescription()
	{
		return "";
	}

	public virtual int GetDifficulty()
	{
		return 0;
	}

	public virtual void Interact(bool fromKeyboard)
	{
	}

	public virtual void Decode(Dictionary<string, object> dict)
	{
		if (dict.TryGetValueAs<string, object, List<object>>("visibleIf", out var valueAs))
		{
			visibilityConditions = valueAs.OfType<string>().ToArray();
		}
	}

	public void RepositionTransform(Vector3 position, Transform parent)
	{
		transformPosition = position;
		transformParent = parent;
		RepositionTransform();
	}

	public virtual void RepositionTransform()
	{
		if ((bool)transform)
		{
			transform.position = transformPosition;
			transform.SetParent(transformParent);
		}
	}
}
