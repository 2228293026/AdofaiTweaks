using UnityEngine;
using UnityEngine.UI;

public class scrLogoText : MonoBehaviour
{
	public static scrLogoText instance;

	public Image fireImage;

	public Image fireLight;

	public Image iceImage;

	public Image iceLight;

	public Image middleImage;

	public bool logoIsChinese;

	private Color? fireColor;

	private Color? iceColor;

	private void Awake()
	{
		bool isChinese = RDString.isChinese;
		if ((isChinese && logoIsChinese) || (!isChinese && !logoIsChinese))
		{
			instance = this;
		}
		UpdateColors();
	}

	private void LoadLogoColor(bool isFire)
	{
		PlanetColor playerColor = Persistence.GetPlayerColor(isFire);
		Color? color = playerColor.ToRealColor();
		if (playerColor.preset == PlanetColorPreset.Rainbow)
		{
			color = null;
		}
		if (isFire)
		{
			fireColor = color;
		}
		else
		{
			iceColor = color;
		}
	}

	public void UpdateColors()
	{
		LoadLogoColor(isFire: false);
		LoadLogoColor(isFire: true);
		ColorLogo(iceColor, isFire: false);
		ColorLogo(fireColor, isFire: true);
	}

	public void ColorLogo(Color? col, bool isFire)
	{
		float H;
		float S;
		float V;
		if (!col.HasValue)
		{
			H = PlanetRenderer.rainbowHue;
			S = 1f;
			V = 1f;
		}
		else
		{
			Color.RGBToHSV(col.Value, out H, out S, out V);
		}
		Color color = Color.HSVToRGB(H, S * 0.6f, V);
		Image image = (isFire ? fireImage : iceImage);
		Image image2 = (isFire ? fireLight : iceLight);
		image.color = color.WithAlpha(image.color.a);
		image2.color = color.WithAlpha(image2.color.a);
	}

	public void LateUpdate()
	{
		if (!iceColor.HasValue)
		{
			ColorLogo(iceColor, isFire: false);
		}
		if (!fireColor.HasValue)
		{
			ColorLogo(fireColor, isFire: true);
		}
	}

	public void Enable(bool enabled)
	{
		Image image = fireImage;
		Image image2 = fireLight;
		Image image3 = iceImage;
		Image image4 = iceLight;
		bool flag = (middleImage.enabled = enabled);
		bool flag3 = (image4.enabled = flag);
		bool flag5 = (image3.enabled = flag3);
		bool flag7 = (image2.enabled = flag5);
		image.enabled = flag7;
	}
}
