public class NXManager_Editor : NXManager
{
	public override bool CanSave => false;

	public override string SaveDiskName => string.Empty;

	public override string SavePath => string.Empty;

	public override string GetPath(string path)
	{
		return string.Empty;
	}

	public override void EnterNotification()
	{
	}

	public override void LeaveNotification()
	{
	}

	public override void ShowShop()
	{
	}

	public override void Init()
	{
	}

	public override bool OnControllerDisconnect()
	{
		return false;
	}

	public override bool ShowControllerSupport(int numPlayers)
	{
		return true;
	}

	public override void UnmountAllDisk()
	{
	}

	public override void UpdateControllersType()
	{
	}
}
