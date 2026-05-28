public class VegaDLCManager : DLCManager
{
	public static VegaDLCManager instance;

	static VegaDLCManager()
	{
		DLCManager.Setup();
	}

	public VegaDLCManager()
	{
		instance = this;
		steamAppId = 2016430u;
		winDepotId = 2016431u;
		win64DepotId = 2016432u;
		macDepotId = 2016433u;
		linuxDepotId = 2016434u;
		groupName = "Team Vega";
	}

	public override string GetMenuScene()
	{
		return "scnVegaMenu";
	}

	public override bool IsDLCScene(string name)
	{
		return name.ToLower().Contains("vega");
	}

	public override bool IsDLCLevel(string name)
	{
		if (!string.IsNullOrEmpty(name) && name[0] != 'T')
		{
			return name.EndsWith("EX");
		}
		return false;
	}
}
