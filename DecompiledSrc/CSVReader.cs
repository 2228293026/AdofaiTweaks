using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
	public string[,] grid;

	public CSVReader(TextAsset csvFile)
	{
		grid = SplitCsvGrid(csvFile.text);
	}

	private string[,] SplitCsvGrid(string csvText)
	{
		string[] array = csvText.Split("\n"[0], StringSplitOptions.None);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = SplitCsvLine(array[i]);
			num = Mathf.Max(num, array2.Length);
		}
		string[,] array3 = new string[num + 1, array.Length + 1];
		for (int j = 0; j < array.Length; j++)
		{
			string[] array4 = SplitCsvLine(array[j]);
			for (int k = 0; k < array4.Length; k++)
			{
				array3[k, j] = array4[k];
				array3[k, j] = array3[k, j].Replace("\"\"", "\"");
			}
		}
		return array3;
	}

	private void DebugOutputGrid(string[,] grid)
	{
		string text = "";
		for (int i = 0; i < grid.GetUpperBound(1); i++)
		{
			for (int j = 0; j < grid.GetUpperBound(0); j++)
			{
				text += grid[j, i];
				text += "|";
			}
			text += "\n";
		}
		Debug.Log(text);
	}

	private string[] SplitCsvLine(string line)
	{
		return (from Match m in Regex.Matches(line, "(((?<x>(?=[,\\r\\n]+))|\"(?<x>([^\"]|\"\")+)\"|(?<x>[^,\\r\\n]+)),?)", RegexOptions.ExplicitCapture)
			select m.Groups[1].Value).ToArray();
	}
}
