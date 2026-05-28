using System;
using System.IO;
using System.Text;
using ADOFAI;
using UnityEngine;

public class RDFile_Default : RDFile
{
	protected override Encoding DefaultEncoding => PlayerPrefsJson.DefaultLevelEncoding;

	protected override void InternalWriteAllLines(string path, string[] content, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = DefaultEncoding;
		}
		File.WriteAllLines(path, content, encoding);
	}

	protected override void InternalWriteAllText(string path, string data, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = DefaultEncoding;
		}
		File.WriteAllText(path, data, encoding);
	}

	protected override void InternalWriteAllBytes(string path, byte[] bytes)
	{
		File.WriteAllBytes(path, bytes);
	}

	protected override string InternalReadAllText(string path, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = DefaultEncoding;
		}
		return File.ReadAllText(path, encoding);
	}

	protected override byte[] InternalReadAllBytes(string path, out LoadResult loadResult)
	{
		loadResult = LoadResult.Error;
		try
		{
			byte[] result = File.ReadAllBytes(path);
			loadResult = LoadResult.Successful;
			return result;
		}
		catch (UnauthorizedAccessException ex)
		{
			loadResult = LoadResult.UnauthorizedAccess;
			Debug.LogError("RDIO ReadAllBytes: UnauthorizedAccessException - " + ex.Message);
			return Array.Empty<byte>();
		}
		catch (Exception ex2)
		{
			Debug.LogError("RDIO ReadAllBytes: Error - " + ex2.Message);
			return Array.Empty<byte>();
		}
	}

	protected override bool InternalExists(string path)
	{
		return File.Exists(path);
	}

	protected override void InternalCopy(string sourceFileName, string destFileName, bool overwrite = false)
	{
		File.Copy(sourceFileName, destFileName, overwrite);
	}

	protected override void InternalDelete(string path)
	{
		File.Delete(path);
	}

	protected override void InternalMove(string sourceFileName, string destFileName)
	{
		File.Move(sourceFileName, destFileName);
	}

	protected override void InternalCreate(string path)
	{
		File.Create(path);
	}
}
