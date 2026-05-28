using UnityEngine;

namespace MobileMenu;

public class MobileMenuBlankScreen : MobileMenuScreen
{
	public override void Instantiate()
	{
		base.Instantiate();
		transform = new GameObject("Blank Screen").transform;
	}
}
