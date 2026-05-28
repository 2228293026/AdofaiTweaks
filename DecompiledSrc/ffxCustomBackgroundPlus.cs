using System;
using System.Collections.Generic;
using ADOFAI;
using UnityEngine;
using UnityEngine.Serialization;

public class ffxCustomBackgroundPlus : ffxPlusBase
{
	[NonSerialized]
	public scrCustomBackgroundSprite custBG;

	public Color color;

	public Color imageColor;

	public string filePath;

	public Camera bgCam;

	public Vector2 parallax;

	public bool tiled;

	public bool looping = true;

	public bool fitScreen = true;

	public bool lockRot;

	public bool imageSmoothing = true;

	[FormerlySerializedAs("unscaledSize")]
	public float scalingRatio;

	private TextureManager imgHolder;

	public override void Awake()
	{
		base.Awake();
		custBG = ADOBase.customLevel.custBG;
		bgCam = cam.Bgcamstatic;
		imgHolder = ADOBase.customLevel.imgHolder;
	}

	public override void StartEffect(scrPlanet planet)
	{
		scnGame instance = scnGame.instance;
		bgCam.backgroundColor = color;
		if (!string.IsNullOrEmpty(filePath))
		{
			TextureManager.CustomSprite valueOrDefault = CollectionExtensions.GetValueOrDefault<string, TextureManager.CustomSprite>((IReadOnlyDictionary<string, TextureManager.CustomSprite>)imgHolder.customSprites, filePath, (TextureManager.CustomSprite)null);
			if (valueOrDefault != null)
			{
				custBG.SetCustomBG(valueOrDefault, imageColor, tiled, looping, fitScreen, scalingRatio, lockRot, imageSmoothing);
				custBG.parallax.multiplier_x = parallax.x;
				custBG.parallax.multiplier_y = parallax.y;
				instance.ShowTutorialBackground(visible: false);
			}
		}
		else
		{
			custBG.SetCustomBG(null, Color.white);
			instance.ShowTutorialBackground(!((Component)(object)instance.videoBG).gameObject.activeSelf && instance.levelData.bgShowDefaultBGIfNoImage);
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		color = evnt.GetColor("color");
	}
}
