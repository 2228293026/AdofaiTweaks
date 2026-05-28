using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Glyphs;

[Serializable]
public class GlyphSetCollection : ScriptableObject
{
	[Tooltip("The list of glyph sets.")]
	[SerializeField]
	private List<GlyphSet> _sets;

	[Tooltip("The list of glyph set collections.")]
	[SerializeField]
	private List<GlyphSetCollection> _collections;

	public List<GlyphSet> sets
	{
		get
		{
			return _sets;
		}
		set
		{
			_sets = value;
		}
	}

	public List<GlyphSetCollection> collections
	{
		get
		{
			return _collections;
		}
		set
		{
			if (value != null && value.Contains(this))
			{
				LogCircularDependency();
				Debug.LogWarning("Rewired: Set collections aborted due to circular dependency.");
			}
			else
			{
				_collections = value;
			}
		}
	}

	public virtual IEnumerable<GlyphSet> IterateSetsRecursively()
	{
		return IterateSetsRecursively(new List<GlyphSetCollection> { this });
	}

	protected virtual IEnumerable<GlyphSet> IterateSetsRecursively(List<GlyphSetCollection> processedCollections)
	{
		if (processedCollections == null)
		{
			throw new ArgumentNullException("processedCollections");
		}
		if (_sets != null)
		{
			int setCount = _sets.Count;
			for (int i = 0; i < setCount; i++)
			{
				if (!(_sets[i] == null))
				{
					yield return sets[i];
				}
			}
		}
		if (_collections == null)
		{
			yield break;
		}
		int collectionCount = _collections.Count;
		for (int i = 0; i < collectionCount; i++)
		{
			if (_collections[i] == null)
			{
				continue;
			}
			if (processedCollections.Contains(_collections[i]))
			{
				LogCircularDependency();
				continue;
			}
			processedCollections.Add(_collections[i]);
			foreach (GlyphSet item in _collections[i].IterateSetsRecursively(processedCollections))
			{
				yield return item;
			}
		}
	}

	private static void LogCircularDependency()
	{
		Debug.LogError("Rewired: Circular dependency detected. This collection is referenced in a child collection. This is not allowed.");
	}
}
