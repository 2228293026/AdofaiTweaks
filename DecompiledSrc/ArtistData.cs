using System.Collections.Generic;

public class ArtistData
{
	public int id;

	public string name;

	public string nameLowercase;

	public string[] evidenceURLs;

	public string link1;

	public string link2;

	public ApprovalLevel approvalLevel;

	public int daysSinceRequest;

	public ArtistDisclaimer[] disclaimers;

	public override string ToString()
	{
		string text = $"{id}: {name}\nlink1: {link1}\nlink2: {link2}\napprovalLevel: {approvalLevel}\nevidenceURLs:";
		string[] array = evidenceURLs;
		foreach (string text2 in array)
		{
			text = text + "\n" + text2;
		}
		if (disclaimers != null)
		{
			ArtistDisclaimer[] array2 = disclaimers;
			foreach (ArtistDisclaimer arg in array2)
			{
				text += $"\n{arg}";
			}
		}
		return text;
	}

	public List<string> GetLinks()
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(link1))
		{
			list.Add(link1);
		}
		if (!string.IsNullOrEmpty(link2))
		{
			list.Add(link2);
		}
		return list;
	}
}
