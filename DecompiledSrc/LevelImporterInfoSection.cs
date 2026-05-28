using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelImporterInfoSection : MonoBehaviour
{
	public List<ImportLevel> levels;

	public void Add(ImportLevel level, string infoText, int siblingIndex, string errorText = "")
	{
		level.infoText.text = infoText;
		if (!string.IsNullOrEmpty(errorText))
		{
			string[] array = infoText.Split('\n', StringSplitOptions.None);
			if (array.Length > 1)
			{
				infoText = array[0];
			}
			if (infoText.Length > 70)
			{
				infoText = infoText.Substring(0, 37) + "...";
			}
			level.infoText.text = infoText;
			Text infoText2 = level.infoText;
			infoText2.text = infoText2.text + "\n<color=#FF0404>" + errorText + "</color>";
		}
		else
		{
			if (infoText.Length > 100)
			{
				infoText = infoText.Substring(0, 37) + "...";
			}
			level.infoText.text = infoText;
		}
		level.infoText.font = RDString.GetAppropiateFontForString(infoText);
		level.transform.SetSiblingIndex(siblingIndex);
		levels.Add(level);
		if (levels.Count == 1)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void Remove(ImportLevel level)
	{
		levels.Remove(level);
	}

	public void Clear()
	{
		foreach (ImportLevel level in levels)
		{
			if (level != null)
			{
				UnityEngine.Object.Destroy(level.gameObject);
			}
		}
		levels = new List<ImportLevel>();
	}
}
