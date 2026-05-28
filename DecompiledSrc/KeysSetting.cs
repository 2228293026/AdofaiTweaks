using System;
using System.Collections.Generic;
using System.Linq;
using SkyHook;
using UnityEngine;

public class KeysSetting
{
	private string settingName;

	private HashSet<KeyCode> _unityKeysCache;

	private HashSet<KeyLabel> _asyncKeysCache;

	private string unityKeysName => settingName + "_unity";

	private string asyncKeysName => settingName + "_async";

	public HashSet<KeyCode> unityKeysCache => _unityKeysCache ?? (_unityKeysCache = unityKeys);

	private HashSet<KeyCode> unityKeys
	{
		get
		{
			return (from x in Persistence.generalPrefs.GetList(unityKeysName)
				select Enum.Parse<KeyCode>(x.ToString())).ToHashSet();
		}
		set
		{
			Persistence.generalPrefs.SetList(unityKeysName, value.Cast<object>().ToList());
			_unityKeysCache = value;
		}
	}

	public HashSet<KeyLabel> asyncKeysCache => _asyncKeysCache ?? (_asyncKeysCache = asyncKeys);

	private HashSet<KeyLabel> asyncKeys
	{
		get
		{
			return (from x in Persistence.generalPrefs.GetList(asyncKeysName)
				select Enum.Parse<KeyLabel>(x.ToString())).ToHashSet();
		}
		set
		{
			Persistence.generalPrefs.SetList(asyncKeysName, value.Cast<object>().ToList());
			_asyncKeysCache = value;
		}
	}

	public List<string> KeyLabels => (RDC.useAsyncInput ? asyncKeys.Select((KeyLabel x) => x.ToString()) : unityKeys.Select((KeyCode x) => x.ToString())).ToList();

	public int Count
	{
		get
		{
			if (!RDC.useAsyncInput)
			{
				return unityKeys.Count;
			}
			return asyncKeys.Count;
		}
	}

	public KeysSetting(string name)
	{
		settingName = name;
	}

	private void Add(KeyCode unityKey, KeyLabel asyncKey)
	{
		HashSet<KeyCode> hashSet = unityKeys;
		HashSet<KeyLabel> hashSet2 = asyncKeys;
		if (unityKey != KeyCode.None)
		{
			hashSet.Add(unityKey);
		}
		if (asyncKey != KeyLabel.Unknown)
		{
			hashSet2.Add(asyncKey);
		}
		unityKeys = hashSet;
		asyncKeys = hashSet2;
	}

	private void Remove(KeyCode unityKey, KeyLabel asyncKey)
	{
		HashSet<KeyCode> hashSet = unityKeys;
		HashSet<KeyLabel> hashSet2 = asyncKeys;
		hashSet.Remove(unityKey);
		hashSet2.Remove(asyncKey);
		unityKeys = hashSet;
		asyncKeys = hashSet2;
	}

	public void Clear()
	{
		HashSet<KeyCode> hashSet = unityKeys;
		HashSet<KeyLabel> hashSet2 = asyncKeys;
		hashSet.Clear();
		hashSet2.Clear();
		unityKeys = hashSet;
		asyncKeys = hashSet2;
	}

	public void Add(KeyCode unityKey)
	{
		KeyLabel asyncKey = AsyncKeyMapper.UnityKeyToAsyncKey(unityKey);
		Add(unityKey, asyncKey);
	}

	public void Add(KeyLabel asyncKey)
	{
		KeyCode unityKey = AsyncKeyMapper.AsyncKeyToUnityKey(asyncKey);
		Add(unityKey, asyncKey);
	}

	public void Remove(KeyCode unityKey)
	{
		KeyLabel asyncKey = AsyncKeyMapper.UnityKeyToAsyncKey(unityKey);
		Remove(unityKey, asyncKey);
	}

	public void Remove(KeyLabel asyncKey)
	{
		KeyCode unityKey = AsyncKeyMapper.AsyncKeyToUnityKey(asyncKey);
		Remove(unityKey, asyncKey);
	}

	public void Toggle(KeyCode unityKey)
	{
		if (unityKeys.Contains(unityKey))
		{
			Remove(unityKey);
		}
		else
		{
			Add(unityKey);
		}
	}

	public void Toggle(KeyLabel asyncKey)
	{
		if (asyncKeys.Contains(asyncKey))
		{
			Remove(asyncKey);
		}
		else
		{
			Add(asyncKey);
		}
	}
}
