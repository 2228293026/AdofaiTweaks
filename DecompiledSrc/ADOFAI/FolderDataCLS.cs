using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADOFAI;

public class FolderDataCLS : GenericDataCLS
{
	public Dictionary<string, GenericDataCLS> containingLevels = new Dictionary<string, GenericDataCLS>();

	private bool localized;

	private string _artist;

	private string _title;

	private string _author;

	private string _description;

	private int _difficulty;

	private string _previewImage;

	private string _previewIcon;

	private Color _previewIconColor;

	private string _tags;

	public object this[string key]
	{
		get
		{
			return containingLevels[key];
		}
		set
		{
			containingLevels[key] = (GenericDataCLS)value;
		}
	}

	public override string title
	{
		get
		{
			if (!localized)
			{
				return _title;
			}
			return RDString.Get(_title);
		}
	}

	public override string artist
	{
		get
		{
			if (!localized)
			{
				return _artist;
			}
			return RDString.Get(_artist);
		}
	}

	public override string author
	{
		get
		{
			if (!localized)
			{
				return _author;
			}
			return RDString.Get(_author);
		}
	}

	public override string description
	{
		get
		{
			if (!localized)
			{
				return _description;
			}
			return RDString.Get(_description);
		}
	}

	public override int difficulty => _difficulty;

	public override string previewImage => _previewImage;

	public override string previewIcon => _previewIcon;

	public override Color previewIconColor => _previewIconColor;

	public override string[] tags => _tags.Replace(", ", ",").Split(',', StringSplitOptions.None);

	public FolderDataCLS(string title, int difficulty = -1, string artist = "", string author = "", string description = "", string previewImage = "", string previewIcon = "", Color previewIconColor = default(Color), string tags = "")
	{
		_title = title;
		_artist = artist;
		_author = author;
		_difficulty = difficulty;
		_description = description;
		_previewImage = previewImage;
		_previewIcon = previewIcon;
		_previewIconColor = previewIconColor;
		_tags = tags;
	}

	public bool Rename(string newTitle)
	{
		if (_title == newTitle)
		{
			return false;
		}
		_title = newTitle;
		return true;
	}
}
