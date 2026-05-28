using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuTitleScreen : MobileMenuScreen
{
	public MobileMenuTitle title;

	private bool neoCosmos;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = UnityEngine.Object.Instantiate(RDConstants.data.prefab_titleScreen).transform;
		title = transform.GetComponent<MobileMenuTitle>();
		title.SetNeoCosmos(neoCosmos);
		title.ShowButtons(show: false, instant: true);
		MobileMenuController instance = MobileMenuController.instance;
		instance.onFinishLoading = (Action)Delegate.Combine(instance.onFinishLoading, new Action(Init));
	}

	private void Init()
	{
		title.ShowButtons(!ADOBase.isExpo, !scnMobileMenu.firstTimeLoadingScene);
		title.UpdateSubtitle();
	}

	public override void Select(bool select = true, bool instant = false)
	{
		if (!title.loading)
		{
			title.FadeSubtitle(select ? 1f : 0f, instant);
		}
	}

	public override void Decode(Dictionary<string, object> dict)
	{
		base.Decode(dict);
		dict.TryGetValueAs("neoCosmos", out neoCosmos, _default: false);
	}
}
