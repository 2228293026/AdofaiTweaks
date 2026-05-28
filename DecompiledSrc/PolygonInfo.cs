using UnityEngine;

public struct PolygonInfo(RangeInt ccwCurve, RangeInt cwCurve)
{
	public RangeInt ccwCurve = ccwCurve;

	public RangeInt cwCurve = cwCurve;
}
