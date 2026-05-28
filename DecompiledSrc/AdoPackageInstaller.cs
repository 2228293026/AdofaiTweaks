using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdoPackageInstaller : ADOBase
{
	public static bool cancelDownload;

	public static PackageInstallerResult<bool> CheckFileIsZip(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				return PackageInstallerResult<bool>.Failure("File in path: " + filePath + " doesn't exist.", value: false);
			}
			if (!Utils.GetFileBuffer(filePath, 0, 4, out var buffer))
			{
				return PackageInstallerResult<bool>.Failure(RDString.Get("packageInstaller.fileAccessError"), value: false);
			}
			string text = Encoding.ASCII.GetString(buffer).Trim();
			char[] array = text.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			char[] array2 = array;
			foreach (char c in array2)
			{
				if (!char.IsControl(c))
				{
					stringBuilder.Append(c);
				}
			}
			if (stringBuilder.ToString() != "PK")
			{
				return PackageInstallerResult<bool>.Failure(RDString.Get("packageInstaller.fakeZipError"), value: false);
			}
		}
		catch (Exception ex)
		{
			return PackageInstallerResult<bool>.Failure(ex.Message, value: false);
		}
		return PackageInstallerResult<bool>.Success(value: true);
	}

	public static IEnumerator DownloadPackage(string url, string resultFilePath, Text levelProgress, Action<PackageInstallerResult<string>> callback)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			string text = RDString.Get("cls.addFromURLInvalid");
			Debug.Log("DownloadPackage " + url + ", " + resultFilePath + ", " + text);
			callback(PackageInstallerResult<string>.Failure(text));
			yield break;
		}
		UnityWebRequest uwr = new UnityWebRequest(result);
		try
		{
			try
			{
				DownloadHandlerFile val = new DownloadHandlerFile(resultFilePath);
				val.removeFileOnAbort = true;
				uwr.method = "GET";
				uwr.downloadHandler = (DownloadHandler)(object)val;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				callback(PackageInstallerResult<string>.Failure(ex.Message));
				yield break;
			}
			uwr.SendWebRequest();
			while (!uwr.isDone)
			{
				if (cancelDownload)
				{
					cancelDownload = false;
					uwr.Abort();
					callback(PackageInstallerResult<string>.Failure("Download was canceled."));
					yield break;
				}
				if (levelProgress != null)
				{
					levelProgress.text = Mathf.RoundToInt(uwr.downloadProgress * 100f) + "%";
				}
				yield return null;
			}
			if (uwr.AnyErrors())
			{
				string error = uwr.error;
				Debug.LogError(error);
				callback(PackageInstallerResult<string>.Failure(error));
			}
			else if (!RDFile.Exists(resultFilePath))
			{
				string text2 = RDString.Get("packageInstaller.downloadError");
				Debug.LogError(text2);
				callback(PackageInstallerResult<string>.Failure(text2));
			}
			else
			{
				callback(PackageInstallerResult<string>.Success(resultFilePath));
			}
		}
		finally
		{
			((IDisposable)uwr)?.Dispose();
		}
	}

	public static PackageInstallerResult<string> FindLevelFile(string levelUnzippedDirectory)
	{
		string text = Path.Combine(levelUnzippedDirectory, "main.adofai");
		if (RDFile.Exists(text))
		{
			return PackageInstallerResult<string>.Success(text);
		}
		try
		{
			string[] filesWithExtension = levelUnzippedDirectory.GetFilesWithExtension("adofai", SearchOption.AllDirectories);
			if (filesWithExtension.Length == 0)
			{
				return PackageInstallerResult<string>.Failure(RDString.Get("packageInstaller.adofaiLevelNotFound"));
			}
			string mainLevelWithoutExtension = Path.GetFileNameWithoutExtension("main.adofai");
			string value = Array.Find(filesWithExtension, (string adoLevel) => Path.GetFileNameWithoutExtension(adoLevel) == mainLevelWithoutExtension);
			if (string.IsNullOrEmpty(value))
			{
				string backupLevelWithoutExtension = Path.GetFileNameWithoutExtension("backup.adofai");
				value = Array.Find(filesWithExtension, (string adoLevel) => Path.GetFileNameWithoutExtension(adoLevel) != backupLevelWithoutExtension && !Path.GetFileName(adoLevel).StartsWith("."));
				if (string.IsNullOrEmpty(value) && filesWithExtension.Length != 0)
				{
					value = filesWithExtension[0];
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return PackageInstallerResult<string>.Failure("No suitable adofai level file could be found.");
			}
			return PackageInstallerResult<string>.Success(value);
		}
		catch (Exception ex)
		{
			return PackageInstallerResult<string>.Failure(ex.Message);
		}
	}

	public static PackageInstallerResult<string> MoveAdofaiLevelFolder(string sourceFolderPath, string targetFolderPath)
	{
		string empty = string.Empty;
		try
		{
			string[] directories = Directory.GetDirectories(sourceFolderPath, "*", SearchOption.TopDirectoryOnly);
			foreach (string obj in directories)
			{
				Utils.SetDirectoryAttributes(obj);
				Directory.Delete(obj, recursive: true);
			}
			string path = new DirectoryInfo(sourceFolderPath).Name;
			empty = Path.Combine(targetFolderPath, path);
			empty = RDUtils.GetAvailableDirectoryName(empty);
			try
			{
				Utils.SetDirectoryAttributes(sourceFolderPath);
				Directory.Move(sourceFolderPath, empty);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Error moving directory: " + ex.Message + ". Copying folder instead from:  " + sourceFolderPath + "   to:  " + targetFolderPath);
				Utils.CopyDirectory(sourceFolderPath, empty);
				try
				{
					Utils.SetDirectoryAttributes(sourceFolderPath);
					Directory.Delete(sourceFolderPath, recursive: true);
				}
				catch (Exception ex2)
				{
					Debug.LogWarning("Couldn't delete source after cross-volume copy: " + ex2.Message);
				}
			}
			return PackageInstallerResult<string>.Success(empty);
		}
		catch (Exception ex3)
		{
			Debug.LogError("Error moving directory: " + ex3.Message + ". Tried moving folder from:  " + sourceFolderPath + "   to:  " + targetFolderPath);
			return PackageInstallerResult<string>.Failure(ex3.Message);
		}
	}

	public static IEnumerator GetFileNameFromUrl(string url, Action<PackageInstallerResult<string>> callback)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			string text = "URL failed: " + url;
			Debug.Log(text);
			callback?.Invoke(PackageInstallerResult<string>.Failure(text, "DownloadedLevel"));
			yield break;
		}
		if (result.Host.Contains("drive.google.com"))
		{
			Match match = Regex.Match(result.AbsoluteUri, "id=([a-zA-Z0-9_-]+)");
			if (match.Success)
			{
				string value = match.Groups[1].Value + ".zip";
				callback?.Invoke(PackageInstallerResult<string>.Success(value));
				yield break;
			}
		}
		string defaultFilePath = Path.Combine(scnCLS.tempLevelsFolder, "DownloadedLevel");
		defaultFilePath = RDUtils.GetAvailableDirectoryName(defaultFilePath);
		UnityWebRequest uwr = UnityWebRequest.Head(result);
		try
		{
			uwr.method = "HEAD";
			uwr.downloadHandler = (DownloadHandler)new DownloadHandlerFile(defaultFilePath)
			{
				removeFileOnAbort = true
			};
			yield return uwr.SendWebRequest();
			if (RDFile.Exists(defaultFilePath) && new FileInfo(defaultFilePath).Length == 0L)
			{
				_ = string.Empty;
			}
			if ((int)uwr.result != 1)
			{
				string error = "Request failed: " + url;
				callback?.Invoke(PackageInstallerResult<string>.Failure(error));
				yield break;
			}
			string filename = (TryGetContentDisposition(uwr, out var filename2) ? filename2 : GenerateValidFileName(url));
			filename = TrimInvalidCharacters(filename);
			if (filename.EndsWith(", attachment"))
			{
				string text2 = filename;
				int length = ", attachment".Length;
				filename = text2.Substring(0, text2.Length - length);
			}
			if (string.IsNullOrEmpty(filename))
			{
				filename = "DownloadedLevel";
			}
			callback?.Invoke(PackageInstallerResult<string>.Success(filename));
		}
		finally
		{
			((IDisposable)uwr)?.Dispose();
		}
		static string GenerateValidFileName(string text3)
		{
			if (Uri.TryCreate(text3, UriKind.Absolute, out var result2))
			{
				string fileName = Path.GetFileName(result2.LocalPath);
				if (!string.IsNullOrEmpty(fileName))
				{
					return fileName;
				}
			}
			foreach (string item in text3.Split('/', StringSplitOptions.None).Reverse())
			{
				string text4 = new string(item.Where((char c) => !char.IsControl(c)).ToArray());
				if (!string.IsNullOrEmpty(text4) && text4.Length > 1)
				{
					return text4;
				}
			}
			return "DownloadedLevel";
		}
		static string ParseContentDisposition(string contentDisposition)
		{
			if (string.IsNullOrEmpty(contentDisposition))
			{
				return string.Empty;
			}
			string text3 = (from param in contentDisposition.Split(';', StringSplitOptions.None)
				select param.TrimStart()).ToList().FirstOrDefault((string param) => param.StartsWith("filename"))?.Substring("filename".Length);
			if (string.IsNullOrEmpty(text3))
			{
				return string.Empty;
			}
			if (text3.StartsWith("*="))
			{
				string text4 = text3;
				int num = text3.IndexOf("'") + 2;
				text3 = text4.Substring(num, text4.Length - num);
			}
			else if (text3.StartsWith("="))
			{
				string text4 = text3;
				int num = text3.IndexOf('=') + 1;
				text3 = text4.Substring(num, text4.Length - num);
			}
			return text3.Trim('"');
		}
		static string TrimInvalidCharacters(string text3)
		{
			return string.Join("_", text3.Split(Path.GetInvalidFileNameChars()));
		}
		static bool TryGetContentDisposition(UnityWebRequest request, out string reference)
		{
			reference = ParseContentDisposition(request.GetResponseHeader("content-disposition")) ?? ParseContentDisposition(request.GetResponseHeader("Content-Disposition"));
			return !string.IsNullOrEmpty(reference);
		}
	}
}
