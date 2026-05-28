using System;
using System.Collections.Generic;

[Serializable]
public class ActionEntryArg
{
	public double fTime;

	public Action<List<float>> aFunc;

	public double fPersist;

	public List<float> args;

	public ActionEntryArg(double t, Action<List<float>> a, List<float> l, double p)
	{
		fTime = t;
		aFunc = a;
		args = l;
		fPersist = p;
	}
}
