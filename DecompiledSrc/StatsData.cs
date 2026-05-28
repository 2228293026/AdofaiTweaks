using System;

[Serializable]
public class StatsData
{
	public string globalStatField = "";

	public double[] value;

	public StatsData(string _field)
	{
		globalStatField = _field;
	}
}
