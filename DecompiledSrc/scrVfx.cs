using UnityEngine;

public class scrVfx : ADOBase
{
	public ColourScheme currentColourScheme;

	public Color[] arrTileFlashColours;

	public TileFlashStyle tileFlashStyle;

	public bool overrideTileSprites;

	public TileOverrideStyle overrideStyle;

	public Sprite[] arrLitTiles;

	public Sprite unlitTile;

	public bool overrideGlowSprites;

	public Sprite[] arrTopGlowSprites;

	public bool overrideScale;

	public float bottomGlowScale = 1f;

	private static scrVfx _instance;

	public static scrVfx instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindAnyObjectByType<scrVfx>();
			}
			return _instance;
		}
	}

	private void Start()
	{
		if (GCS.lofiVersion)
		{
			int num = 0;
			if (ADOBase.controller.gameworld)
			{
				num = scrController.currentWorld;
			}
			ColourScheme[] worldColourScheme = RDConstants.data.worldColourScheme;
			if (num < worldColourScheme.Length)
			{
				currentColourScheme = worldColourScheme[num];
			}
			ADOBase.controller.camy.SetBgColour();
		}
	}
}
