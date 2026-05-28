using System;
using UnityEngine;

public class ArtistDisclaimer
{
	public int id;

	public string text;

	public SystemLanguage language;

	public DateTime createdAt;

	public DateTime updatedAt;

	public override string ToString()
	{
		return $"disclaimer ({language})\n{text}";
	}
}
