using UnityEngine;

public static class GOExtensions
{
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return gameObject.AddComponent<T>();
	}

	public static T GetOrAddComponent<T>(this Behaviour gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return gameObject.gameObject.AddComponent<T>();
	}

	public static void DisableComponent<T>(this GameObject gameObject) where T : Behaviour
	{
		T component = gameObject.GetComponent<T>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	public static void DisableComponent<T>(this Behaviour gameObject) where T : Behaviour
	{
		T component = gameObject.GetComponent<T>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	public static GameObject Instantiate(this GameObject gameObject, string name, bool makeContainer, string customContainer = null, int room = -1)
	{
		GameObject gameObject2 = Object.Instantiate(gameObject);
		gameObject2.name = name;
		if (makeContainer)
		{
			string text = ((customContainer != null) ? customContainer : (name + " Container"));
			if (room != -1)
			{
				text = text + " " + room;
			}
			GameObject gameObject3 = RDUtils.SpawnIfNotFound(text);
			gameObject2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		}
		return gameObject2;
	}

	public static GameObject Instantiate(this GameObject gameObject, Transform parent, string name)
	{
		GameObject gameObject2 = Object.Instantiate(gameObject);
		gameObject2.name = name;
		gameObject2.transform.parent = parent;
		return gameObject2;
	}
}
