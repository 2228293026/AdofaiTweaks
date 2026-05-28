public class WorldLightsDisplay_Generic : WorldLightsDisplay
{
	public override void UpdateStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true)
	{
		lights[0].gameObject.SetActive(isWorldComplete);
		lights[1].gameObject.SetActive(isWorldPerfect);
		lights[2].gameObject.SetActive(isWorldSpeedTrial);
	}
}
