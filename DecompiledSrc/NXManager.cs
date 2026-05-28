public abstract class NXManager
{
	public static readonly NXManager Instance = new NXManager_Editor();

	public abstract bool CanSave { get; }

	public abstract string SaveDiskName { get; }

	public abstract string SavePath { get; }

	public abstract string GetPath(string path);

	public abstract void EnterNotification();

	public abstract void LeaveNotification();

	public abstract void ShowShop();

	public abstract void Init();

	public abstract void UnmountAllDisk();

	public abstract bool OnControllerDisconnect();

	public abstract bool ShowControllerSupport(int numPlayers);

	public abstract void UpdateControllersType();
}
