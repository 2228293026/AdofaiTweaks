using System.Collections.Generic;

public class DecorationsArray<T> : List<T>
{
	public new void Add(T item)
	{
		base.Add(item);
		CallDecorationUpdate();
	}

	public new void Insert(int index, T item)
	{
		base.Insert(index, item);
		CallDecorationUpdate();
	}

	private static void CallDecorationUpdate()
	{
		if (scnEditor.instance != null)
		{
			scnEditor.instance.propertyControlDecorationsList.OnDecorationUpdate();
		}
	}
}
