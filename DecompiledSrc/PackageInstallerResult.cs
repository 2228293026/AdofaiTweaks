public class PackageInstallerResult<T>
{
	public T Value { get; }

	public string Error { get; }

	public bool IsSuccess => string.IsNullOrEmpty(Error);

	private PackageInstallerResult(T value, string error)
	{
		Value = value;
		Error = error;
	}

	public static PackageInstallerResult<T> Success(T value)
	{
		return new PackageInstallerResult<T>(value, null);
	}

	public static PackageInstallerResult<T> Failure(string error, T value = default(T))
	{
		return new PackageInstallerResult<T>(value, error);
	}
}
