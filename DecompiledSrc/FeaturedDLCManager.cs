public class FeaturedDLCManager : DLCManager
{
	public static FeaturedDLCManager instance;

	static FeaturedDLCManager()
	{
		DLCManager.Setup();
	}

	public FeaturedDLCManager()
	{
		instance = this;
		own = true;
		groupName = "Featured Levels";
	}

	public override string GetMenuScene()
	{
		return GCNS.sceneLevelSelect;
	}

	public override void CheckInstalled()
	{
		installed = false;
	}

	public override bool IsDLCScene(string name)
	{
		return false;
	}

	public override bool IsDLCLevel(string name)
	{
		return false;
	}
}
