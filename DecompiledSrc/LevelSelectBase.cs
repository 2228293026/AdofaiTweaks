using System;

public abstract class LevelSelectBase : ADOBase
{
	public static LevelSelectBase instance;

	[NonSerialized]
	public scrPlanet lastPlanetLanded;

	[NonSerialized]
	public int _menuPhase;

	public int menuPhase
	{
		get
		{
			return _menuPhase;
		}
		set
		{
			if (value != 0 || !scrController.coopMode)
			{
				_menuPhase = value;
			}
		}
	}

	protected virtual void Awake()
	{
		instance = this;
	}

	public virtual void PlanetLandedOnFloor(scrPlanet planet, scrFloor floor)
	{
	}
}
