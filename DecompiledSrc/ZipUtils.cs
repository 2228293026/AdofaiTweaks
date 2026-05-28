using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ZipUtils
{
	private static readonly Encoding[] Encodings = new Encoding[2]
	{
		Encoding.UTF8,
		Encoding.GetEncoding(949)
	};

	public static void Unzip(string sourceArchiveFileName, string destinationDirectoryName)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		if (Directory.Exists(destinationDirectoryName))
		{
			if (Directory.EnumerateFileSystemEntries(destinationDirectoryName).Any())
			{
				Debug.LogWarning("Unzip: Extracting to non-empty directory at '" + destinationDirectoryName + "'.");
			}
			else
			{
				Debug.Log("Unzip: Extracting to existing empty directory at '" + destinationDirectoryName + "'.");
			}
		}
		else
		{
			Debug.Log("Unzip: Creating directory to extract to at '" + destinationDirectoryName + "'.");
			Directory.CreateDirectory(destinationDirectoryName);
		}
		Encoding entryNameEncoding = Encoding.UTF8;
		using (FileStream fileStream = File.OpenRead(sourceArchiveFileName))
		{
			Encoding[] encodings = Encodings;
			foreach (Encoding encoding in encodings)
			{
				ZipArchive val = new ZipArchive((Stream)fileStream, (ZipArchiveMode)0, true, encoding);
				try
				{
					bool flag = true;
					foreach (ZipArchiveEntry entry in val.Entries)
					{
						if (entry.Name.Contains('\ufffd'))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						entryNameEncoding = encoding;
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		string text = Path.GetFullPath(destinationDirectoryName);
		if (!text.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			text += Path.DirectorySeparatorChar;
		}
		ZipArchive val2 = ZipFile.Open(sourceArchiveFileName, (ZipArchiveMode)0, entryNameEncoding);
		try
		{
			if (val2.Entries.Count > 10000)
			{
				throw new IOException($"Zip extraction aborted: too many entries ({val2.Entries.Count} > {10000}).");
			}
			long num = 0L;
			foreach (ZipArchiveEntry entry2 in val2.Entries)
			{
				num += entry2.Length;
				if (num > 2097152000)
				{
					throw new IOException($"Zip extraction aborted: uncompressed size exceeds {2000L} MB limit.");
				}
			}
			long num2 = 0L;
			byte[] array = new byte[81920];
			foreach (ZipArchiveEntry entry3 in val2.Entries)
			{
				string fullPath = Path.GetFullPath(Path.Combine(destinationDirectoryName, entry3.FullName));
				if (!fullPath.StartsWith(text, StringComparison.Ordinal))
				{
					throw new IOException("Zip extraction blocked: entry '" + entry3.FullName + "' resolves outside destination directory.");
				}
				if (string.IsNullOrEmpty(entry3.Name))
				{
					Directory.CreateDirectory(fullPath);
					continue;
				}
				string directoryName = Path.GetDirectoryName(fullPath);
				if (!string.IsNullOrEmpty(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using Stream stream = entry3.Open();
				using FileStream fileStream2 = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
				int num3;
				while ((num3 = stream.Read(array, 0, array.Length)) > 0)
				{
					num2 += num3;
					if (num2 > 2097152000)
					{
						fileStream2.Close();
						File.Delete(fullPath);
						throw new IOException($"Zip extraction aborted mid-stream: actual uncompressed size exceeds {2000L} MB limit.");
					}
					fileStream2.Write(array, 0, num3);
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static void Zip(string zipFileName, params string[] files)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		string directoryName = Path.GetDirectoryName(zipFileName);
		if (File.Exists(zipFileName))
		{
			Debug.Log("Zip: Overwriting existing file at '" + zipFileName + "'.");
		}
		else if (!Directory.Exists(directoryName))
		{
			Debug.Log("Zip: Creating directory at '" + directoryName + "'.");
			Directory.CreateDirectory(directoryName);
		}
		using FileStream fileStream = new FileStream(zipFileName, FileMode.Create);
		ZipArchive val = new ZipArchive((Stream)fileStream, (ZipArchiveMode)1);
		try
		{
			foreach (string text in files)
			{
				val.CreateEntryFromFile(text, Path.GetFileName(text));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
