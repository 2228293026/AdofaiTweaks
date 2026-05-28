using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
	public static Vector2 Add(this Vector2 origin, float angle, float distance)
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance + origin;
	}

	public static void AddMany<T>(this List<T> list, params T[] values)
	{
		list.AddRange(values);
	}

	public static float ToDeg(this float angle)
	{
		return angle * 57.29578f;
	}

	public static int WithinArray(this int value, ICollection array)
	{
		return Mathf.Clamp(value, 0, array.Count - 1);
	}

	public static int WithinArray(this int value, Array array)
	{
		return Mathf.Clamp(value, 0, array.Length - 1);
	}

	public static int Conn(this Vector3 v)
	{
		return (int)v.z;
	}

	public static string[] GetFilesWithExtension(this string directoryPath, string extension, SearchOption searchOption)
	{
		return Array.FindAll(Directory.GetFiles(directoryPath, "*", searchOption), (string filePath) => IsFileWithExtension(filePath, extension));
	}

	private static bool IsFileWithExtension(string filePath, string extension)
	{
		return string.Equals(Path.GetExtension(filePath), "." + extension, StringComparison.OrdinalIgnoreCase);
	}

	public static bool GetFileBuffer(string filePath, int offset, int length, out byte[] buffer)
	{
		buffer = null;
		try
		{
			using BinaryReader binaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open));
			binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			buffer = new byte[length];
			binaryReader.Read(buffer, 0, length);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		return buffer != null;
	}

	public static void CopyDirectory(string sourceDirectory, string targetDirectory)
	{
		DirectoryInfo source = new DirectoryInfo(sourceDirectory);
		DirectoryInfo target = new DirectoryInfo(targetDirectory);
		CopyAll(source, target);
	}

	private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		Directory.CreateDirectory(target.FullName);
		FileInfo[] files = source.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			Console.WriteLine("Copying {0}\\{1}", target.FullName, fileInfo.Name);
			fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), overwrite: true);
		}
		DirectoryInfo[] directories = source.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			DirectoryInfo target2 = target.CreateSubdirectory(directoryInfo.Name);
			CopyAll(directoryInfo, target2);
		}
	}

	public static void SetDirectoryAttributes(string directoryPath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
		if (directoryInfo.Exists)
		{
			SetAttributesNormal(directoryInfo);
		}
	}

	private static void SetAttributesNormal(DirectoryInfo dir)
	{
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo obj in directories)
		{
			SetAttributesNormal(obj);
			obj.Attributes = FileAttributes.Normal;
		}
		FileInfo[] files = dir.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			files[i].Attributes = FileAttributes.Normal;
		}
	}

	public static async Task<string> TryDeleteDirectory(string directoryPath, int maxRetries = 10, int millisecondsDelay = 30)
	{
		string errorMessage = string.Empty;
		int i = 0;
		while (i < maxRetries)
		{
			try
			{
				Directory.Delete(directoryPath, recursive: true);
				return string.Empty;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				Debug.Log("Error try deleting: " + directoryPath + ", " + ex.Message);
				await Task.Delay(millisecondsDelay);
			}
			int num = i + 1;
			i = num;
		}
		Debug.Log("FAILED Finished deleting: " + directoryPath + ", " + errorMessage);
		return errorMessage;
	}
}
