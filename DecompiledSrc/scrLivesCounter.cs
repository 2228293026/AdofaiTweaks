public class scrLivesCounter : ADOBase
{
	public int lives = -1;

	public scrLifePlanet[] lifePlanets;

	public static scrLivesCounter instance;

	private void Awake()
	{
		instance = this;
		Reset();
		if (lives < 0 || !scrController.instance.gameworld)
		{
			Hide();
		}
	}

	public void Hide()
	{
		for (int i = 0; i < lifePlanets.Length; i++)
		{
			lifePlanets[i].gameObject.SetActive(value: false);
		}
	}

	public void SetLives(int lives)
	{
		this.lives = lives;
		for (int num = lifePlanets.Length; num > 0; num--)
		{
			scrLifePlanet scrLifePlanet2 = lifePlanets[num - 1];
			if (lives >= num)
			{
				scrLifePlanet2.Revive();
			}
			else
			{
				scrLifePlanet2.Kill();
			}
		}
	}

	public void Reset()
	{
		SetLives(ADOBase.isExpo ? 3 : (-1));
	}
}
