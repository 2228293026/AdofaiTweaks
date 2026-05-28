using UnityEngine;

namespace MobileMenu;

public class PuzzleRoomsScreen : MobileMenuScreen
{
	public override void Instantiate()
	{
		base.Instantiate();
		transform = Object.Instantiate(RDConstants.data.prefab_puzzleRoomsScreen).transform;
	}

	public override float GetWidth()
	{
		return base.GetWidth() * 1.5f;
	}

	public override string GetDescription()
	{
		return RDString.Get("TP.title");
	}

	public override void Select(bool select = true, bool instant = false)
	{
	}

	public override void Interact(bool fromKeyboard)
	{
		base.Instantiate();
		scrController.instance.PortalTravelAction(Portal.Puzzle1);
	}
}
