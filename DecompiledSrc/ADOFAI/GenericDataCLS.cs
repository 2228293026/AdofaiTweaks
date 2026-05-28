using UnityEngine;

namespace ADOFAI;

public abstract class GenericDataCLS
{
	public string parentFolderName;

	private string _hash;

	public abstract string title { get; }

	public abstract string artist { get; }

	public abstract string author { get; }

	public abstract string description { get; }

	public abstract int difficulty { get; }

	public abstract string previewImage { get; }

	public abstract string previewIcon { get; }

	public abstract Color previewIconColor { get; }

	public abstract string[] tags { get; }

	public bool isLevel => this is LevelDataCLS;

	public bool isFolder => this is FolderDataCLS;

	public LevelDataCLS level => this as LevelDataCLS;

	public FolderDataCLS folder => this as FolderDataCLS;

	public string Hash
	{
		get
		{
			if (string.IsNullOrEmpty(_hash))
			{
				_hash = MD5Hash.GetHash(author + artist + title);
			}
			return _hash;
		}
	}
}
