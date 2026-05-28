using UnityEngine;

namespace ByteSheep.Events;

public class EventFilterAttribute : PropertyAttribute
{
	public TargetFilter targetFilters = TargetFilter.Static | TargetFilter.Dynamic;

	public TargetTypeFilter typeFilters = TargetTypeFilter.Any;

	public EventFilterAttribute(TargetFilter targetFilters)
	{
		this.targetFilters = targetFilters;
	}

	public EventFilterAttribute(TargetTypeFilter typeFilters)
	{
		this.typeFilters = typeFilters;
	}

	public EventFilterAttribute(TargetFilter targetFilters, TargetTypeFilter typeFilters)
	{
		this.targetFilters = targetFilters;
		this.typeFilters = typeFilters;
	}
}
