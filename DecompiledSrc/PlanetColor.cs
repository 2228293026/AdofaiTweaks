using UnityEngine;

public struct PlanetColor
{
	public PlanetColorPreset preset;

	public Color? customColor;

	public PlanetColor(PlanetColorPreset preset)
	{
		this.preset = preset;
		customColor = null;
	}

	public PlanetColor(Color customColor)
	{
		preset = PlanetColorPreset.Custom;
		this.customColor = customColor;
	}

	public Color ToRealColor()
	{
		return customColor ?? preset.GetColor();
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(PlanetColor lhs, PlanetColor rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PlanetColor lhs, PlanetColor rhs)
	{
		return !(lhs == rhs);
	}
}
