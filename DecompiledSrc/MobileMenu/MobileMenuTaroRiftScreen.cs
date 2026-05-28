using UnityEngine;

namespace MobileMenu;

public class MobileMenuTaroRiftScreen : MobileMenuScreen
{
	public Crack crack;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_taroRiftScreen).transform;
		crack = transform.GetComponent<Crack>();
	}

	public override void Select(bool select = true, bool instant = false)
	{
	}
}
