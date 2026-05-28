using System;
using System.Collections.Generic;

[Serializable]
public class TextEvent
{
	public int iTime;

	public int iEvent;

	public List<string> args;

	public TextEvent(int t, int e, List<string> l)
	{
		iTime = t;
		iEvent = e;
		args = l;
	}
}
