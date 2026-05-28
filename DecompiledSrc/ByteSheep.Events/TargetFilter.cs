using System;

namespace ByteSheep.Events;

[Flags]
public enum TargetFilter
{
	Static = 7,
	StaticField = 1,
	StaticProperty = 2,
	StaticMethod = 4,
	Dynamic = 0x38,
	DynamicField = 8,
	DynamicProperty = 0x10,
	DynamicMethod = 0x20
}
