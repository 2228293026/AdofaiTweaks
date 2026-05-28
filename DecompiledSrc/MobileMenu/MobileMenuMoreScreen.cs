using System;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuMoreScreen : MobileMenuScreen
{
	public MobileMenuMorePage morePage;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = GameObject.Find("MoreScreen").transform;
		morePage = transform.GetComponent<MobileMenuMorePage>();
		morePage.descriptionScreen = this;
		onSelect = (Action<bool, bool>)Delegate.Combine(onSelect, new Action<bool, bool>(morePage.OnSelectDescription));
	}
}
