namespace MobileMenu;

public class MobileMenuGrabbablePlanet : MobileMenuGrabbable
{
	public bool isRed;

	public PlanetRenderer planet;

	public override void Ungrab()
	{
		scrSfx.instance.PlaySfx(SfxSound.PlanetRelease, MixerGroup.InterfaceParent);
	}
}
