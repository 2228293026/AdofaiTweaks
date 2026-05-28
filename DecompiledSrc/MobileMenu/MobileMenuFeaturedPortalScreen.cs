using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu;

public class MobileMenuFeaturedPortalScreen : MobileMenuScreen
{
	private FeaturedPortal featuredPortal;

	private scnCLS.Category category;

	private bool isFeatured => category == scnCLS.Category.Featured;

	private bool isTech => category == scnCLS.Category.Tech;

	public override void Decode(Dictionary<string, object> dict)
	{
		base.Decode(dict);
		category = RDUtils.ParseEnum((string)dict["category"], scnCLS.Category.Selection);
	}

	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_featuredPortal).transform;
		featuredPortal = transform.GetComponent<FeaturedPortal>();
		featuredPortal.portalChanger.portalType = ((!isFeatured) ? (isTech ? scrCLSPortalChanger.PortalType.Tech : scrCLSPortalChanger.PortalType.Workshop) : scrCLSPortalChanger.PortalType.Classic);
		featuredPortal.textChanger.desktopText = (isFeatured ? "cls.classicFeatured" : (isTech ? "cls.techFeatured" : "cls.workshop"));
	}

	public override void Interact(bool fromKeyboard)
	{
		MobileMenuController.instance.OpenFeaturedLevels(category);
	}

	public override string GetDescription()
	{
		if (!isFeatured)
		{
			if (!isTech)
			{
				return "";
			}
			return RDString.Get("levelSelect.techFeaturedDescription");
		}
		return RDString.Get("levelSelect.classicFeaturedDescription");
	}
}
