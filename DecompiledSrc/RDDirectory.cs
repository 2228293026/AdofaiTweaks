public abstract class RDDirectory
{
	private static readonly RDDirectory Instance = new RDDirectory_Default();

	public static bool Exists(string path)
	{
		return Instance.InternalExists(path);
	}

	public static void CreateDirectory(string path)
	{
		Instance.InternalCreateDirectory(path);
	}

	public static void Copy(string sourceDirName, string destDirName, bool recursive = false)
	{
		Instance.InternalCopy(sourceDirName, destDirName, recursive);
	}

	protected abstract bool InternalExists(string path);

	protected abstract void InternalCreateDirectory(string path);

	protected abstract void InternalCopy(string sourceDirName, string destDirName, bool recursive = false);
}
