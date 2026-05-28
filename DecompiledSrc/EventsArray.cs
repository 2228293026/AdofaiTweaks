using System.Collections.Generic;

public class EventsArray<T> : List<T>
{
	public new void Add(T item)
	{
		base.Add(item);
	}
}
