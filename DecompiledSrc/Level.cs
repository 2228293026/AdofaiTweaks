using UnityEngine;

public class Level : ADOClass
{
	public virtual void Init()
	{
	}

	public virtual void Hit(int floor)
	{
	}

	public static T FindDecorationComponent<T>(string decorationTag) where T : Component
	{
		scrDecorationManager instance = scrDecorationManager.instance;
		if (instance == null)
		{
			return null;
		}
		foreach (scrDecoration taggedDecoration in instance.GetTaggedDecorations(decorationTag))
		{
			if (taggedDecoration != null)
			{
				T componentInChildren = taggedDecoration.transform.GetComponentInChildren<T>();
				if (componentInChildren != null)
				{
					return componentInChildren;
				}
			}
		}
		return null;
	}

	public void HideDecorations(string tag)
	{
		SetDecorationVisibility(tag, visible: false);
	}

	public void ShowDecorations(string tag)
	{
		SetDecorationVisibility(tag, visible: true);
	}

	private void SetDecorationVisibility(string tag, bool visible)
	{
		if (base.decorationManager == null)
		{
			return;
		}
		foreach (scrDecoration taggedDecoration in base.decorationManager.GetTaggedDecorations(tag))
		{
			taggedDecoration.gameObject.SetActive(visible);
		}
	}
}
