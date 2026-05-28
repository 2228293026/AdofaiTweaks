using System;

public class ActionEntry
{
	public double fTime;

	public Action aFunc;

	public double fPersist;

	public ActionEntry(double t, Action a, double p)
	{
		fTime = t;
		aFunc = a;
		fPersist = p;
	}
}
