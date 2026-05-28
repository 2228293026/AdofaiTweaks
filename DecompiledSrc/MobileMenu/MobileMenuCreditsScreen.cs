using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuCreditsScreen : MobileMenuScreen
{
	public scrCreditsText credits;

	private bool neoCosmos;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_creditsScreen).transform;
		credits = transform.GetComponentInChildren<scrCreditsText>();
		if (neoCosmos)
		{
			credits.creditsType = scrCreditsText.CreditsType.NeoCosmosCredits;
			credits.pigStatue.SetActive(value: false);
			credits.os.gameObject.SetActive(Persistence.IsWorldComplete("T5"));
			credits.title.GetComponent<scrTextChanger>().desktopText = "credits.neoCosmosTitle";
		}
		credits.Setup();
		credits.Reset(instant: true);
		credits.transform.LocalMoveY(4.5f);
		credits.pigStatue.GetComponent<scrGfxFloat>().transform.TranslateY(4.5f);
	}

	public override void Decode(Dictionary<string, object> dict)
	{
		base.Decode(dict);
		dict.TryGetValueAs("neoCosmos", out neoCosmos, _default: false);
	}

	public override void Select(bool select = true, bool instant = false)
	{
		credits.SetScroll(select);
	}
}
