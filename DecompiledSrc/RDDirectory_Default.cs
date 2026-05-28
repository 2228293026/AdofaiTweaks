using System.IO;
using UnityEngine;

public class RDDirectory_Default : RDDirectory
{
	protected override bool InternalExists(string path)
	{
		return Directory.Exists(path);
	}

	protected override void InternalCreateDirectory(string path)
	{
		Directory.CreateDirectory(path);
	}

	protected override void InternalCopy(string sourceDirName, string destDirName, bool recursive = false)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
		if (!directoryInfo.Exists)
		{
			Debug.LogError("RDDirectory.Copy: sourceDir not found: " + sourceDirName);
			return;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		InternalCreateDirectory(destDirName);
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			string destFileName = Path.Combine(destDirName, fileInfo.Name);
			fileInfo.CopyTo(destFileName);
		}
		if (recursive)
		{
			DirectoryInfo[] array = directories;
			foreach (DirectoryInfo directoryInfo2 in array)
			{
				string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
				InternalCopy(directoryInfo2.FullName, destDirName2, recursive: true);
			}
		}
	}
}
