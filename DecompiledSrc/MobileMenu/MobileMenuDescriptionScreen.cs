using System;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuDescriptionScreen : MobileMenuScreen
{
	public Action purchaseAction;

	public Action restoreAction;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = new GameObject("DescriptionScreen").transform;
	}

	public override void Interact(bool fromKeyboard)
	{
		if (fromKeyboard)
		{
			purchaseAction?.Invoke();
		}
	}
}
