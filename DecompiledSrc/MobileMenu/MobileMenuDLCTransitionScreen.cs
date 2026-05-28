using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuDLCTransitionScreen : MobileMenuScreen
{
	public MobileMenuDLCTransitionPortal portal;

	private bool neoCosmos;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_DLCTransitionScreen).transform;
		portal = transform.GetComponent<MobileMenuDLCTransitionPortal>();
		portal.screen = this;
	}

	public override void Select(bool select = true, bool instant = false)
	{
		portal.EnterPortal(neoCosmos);
	}

	public override void Decode(Dictionary<string, object> dict)
	{
		base.Decode(dict);
		dict.TryGetValueAs("neoCosmos", out neoCosmos, _default: false);
	}
}
