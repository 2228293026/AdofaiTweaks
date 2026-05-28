using System;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuGrabController
{
	public MobileMenuGrabbable grabbedObject;

	public Action<MobileMenuGrabbable> onGrab;

	public Action<MobileMenuGrabbable> onUngrab;

	public bool TryGrabObjectAt(Vector2 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit2D val = Physics2D.Raycast(pos, Vector2.zero);
		if ((UnityEngine.Object)(object)((RaycastHit2D)(ref val)).collider != null && ((Component)(object)((RaycastHit2D)(ref val)).collider).TryGetComponent(out MobileMenuGrabbable component))
		{
			return GrabObject(component);
		}
		return false;
	}

	public bool GrabObject(MobileMenuGrabbable obj)
	{
		if (!obj.grabbable)
		{
			return false;
		}
		grabbedObject = obj;
		bool num = grabbedObject.Grab();
		if (num && onGrab != null)
		{
			onGrab(grabbedObject);
		}
		return num;
	}

	public void ToggleGrabObject(MobileMenuGrabbable obj)
	{
		if (grabbedObject == obj)
		{
			UngrabObject();
		}
		else
		{
			GrabObject(obj);
		}
	}

	public void UpdateGrabbedObject(Vector2 pos)
	{
		if ((bool)grabbedObject)
		{
			grabbedObject.Move(pos);
		}
	}

	public void UngrabObject()
	{
		if ((bool)grabbedObject)
		{
			grabbedObject.Ungrab();
			if (onUngrab != null)
			{
				onUngrab(grabbedObject);
			}
			grabbedObject = null;
		}
	}
}
