namespace ADOFAI;

public class MinimizedFloorData
{
	public double entryangle;

	public double exitangle;

	public double angleLength;

	public bool ccw;

	public bool midspin;

	public bool turnaround;

	public int numPlanets;

	public int holdLength = -1;

	public MinimizedFloorData(double entryangle, double exitangle, bool ccw, bool midspin, bool turnaround, int numPlanets, int holdLength)
	{
		this.entryangle = entryangle;
		this.exitangle = exitangle;
		this.ccw = ccw;
		this.midspin = midspin;
		this.turnaround = turnaround;
		this.numPlanets = numPlanets;
		this.holdLength = holdLength;
	}
}
