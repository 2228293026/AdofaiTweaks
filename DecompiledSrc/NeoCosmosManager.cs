public class NeoCosmosManager : DLCManager
{
	public static NeoCosmosManager instance;

	static NeoCosmosManager()
	{
		DLCManager.Setup();
	}

	public NeoCosmosManager()
	{
		instance = this;
		steamAppId = 1977570u;
		winDepotId = 1977571u;
		win64DepotId = 1977572u;
		macDepotId = 1977573u;
		linuxDepotId = 1977574u;
		steamWorkshopTag = "Neo Cosmos";
		groupName = "Neo Cosmos";
	}

	public override string GetMenuScene()
	{
		return scrController.instance.GetTaroMenuToGoTo();
	}

	public override void CheckInstalled()
	{
		base.CheckInstalled();
	}

	public override bool IsDLCScene(string name)
	{
		return name.StartsWith("scnTaro");
	}

	public override bool IsDLCLevel(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			return name[0] == 'T';
		}
		return false;
	}
}
