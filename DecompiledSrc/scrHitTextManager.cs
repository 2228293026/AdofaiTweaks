using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class scrHitTextManager : ADOClass
{
	private struct HitTextOffset(Vector2 position, Vector2 borderOffset = default(Vector2), float size = 1f, float angle = 0f)
	{
		public Vector3 position = position;

		public Vector3 borderOffset = borderOffset;

		public float size = size;

		public float angle = angle;
	}

	public scrPlayerManager playerManager;

	private Dictionary<HitMargin, scrHitTextMesh[]> cachedHitTexts;

	public GameObject hitTextContainer;

	private const float SmallTextSize = 0.75f;

	private static readonly HitTextOffset[][] hitTextOffsetSets = new HitTextOffset[4][]
	{
		new HitTextOffset[1]
		{
			new HitTextOffset(Vector2.up)
		},
		new HitTextOffset[2]
		{
			new HitTextOffset(Vector2.up, new Vector2(0f, 0.2f)),
			new HitTextOffset(Vector2.down, new Vector2(0f, -0.2f))
		},
		new HitTextOffset[3]
		{
			new HitTextOffset(new Vector2(-0.5f, 0.8f), new Vector2(-0.5f, 0.2f), 0.75f, 5f),
			new HitTextOffset(new Vector2(0f, -0.8f), new Vector2(0f, -0.2f), 0.8f),
			new HitTextOffset(new Vector2(0.5f, 0.8f), new Vector2(-0.5f, 0.2f), 0.75f, -5f)
		},
		new HitTextOffset[4]
		{
			new HitTextOffset(new Vector2(-0.5f, 0.8f), new Vector2(-0.5f, 0.2f), 0.75f, 5f),
			new HitTextOffset(new Vector2(-0.5f, -0.8f), new Vector2(-0.5f, -0.2f), 0.75f, -5f),
			new HitTextOffset(new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.2f), 0.75f, -5f),
			new HitTextOffset(new Vector2(0.5f, -0.8f), new Vector2(0.5f, -0.2f), 0.75f, 5f)
		}
	};

	public scrHitTextManager(scrPlayerManager playerManager)
	{
		this.playerManager = playerManager;
		hitTextContainer = new GameObject("HitTexts");
		Transform transform = hitTextContainer.transform;
		HitMargin[] obj = (HitMargin[])Enum.GetValues(typeof(HitMargin));
		cachedHitTexts = new Dictionary<HitMargin, scrHitTextMesh[]>();
		HitMargin[] array = obj;
		foreach (HitMargin hitMargin in array)
		{
			if (hitMargin != HitMargin.Auto)
			{
				scrHitTextMesh[] array2 = new scrHitTextMesh[100];
				for (int j = 0; j < 100; j++)
				{
					scrHitTextMesh componentInChildren = UnityEngine.Object.Instantiate(base.gc.hitTextPrefab, transform).GetComponentInChildren<scrHitTextMesh>();
					componentInChildren.Init(hitMargin);
					array2[j] = componentInChildren;
				}
				cachedHitTexts[hitMargin] = array2;
			}
		}
	}

	public void ShowHitText(HitMargin hitMargin, scrPlanet planet, float missAngle = 0f)
	{
		if (RDC.noHud && !RDC.partialNoHud)
		{
			return;
		}
		scrHitTextMesh scrHitTextMesh2 = cachedHitTexts[hitMargin].FirstOrDefault((scrHitTextMesh x) => x.dead);
		if (scrHitTextMesh2 == null)
		{
			return;
		}
		if (scrController.coopMode)
		{
			scrPlayer player = planet.player;
			if (!player.doingRevivalCountdown)
			{
				List<scrPlayer> activePlayers = playerManager.GetActivePlayers();
				int a = activePlayers.FindIndex((scrPlayer x) => x == player);
				int a2 = activePlayers.Count();
				a = Mathf.Max(a, 0);
				a2 = Mathf.Max(a2, 1);
				HitTextOffset hitTextOffset = hitTextOffsetSets[a2 - 1][a];
				float fadeSpeed = ((a2 > 2) ? 3f : 1f);
				Vector3 vector = base.controller.camy.transform.TransformPoint(hitTextOffset.position) - base.controller.camy.transform.position;
				Vector3 borderOffset = base.controller.camy.transform.TransformPoint(hitTextOffset.borderOffset) - base.controller.camy.transform.position;
				scrHitTextMesh2.Show(planet.transform.position + vector, borderOffset, missAngle, hitTextOffset.size, hitTextOffset.angle, fadeSpeed);
				scrHitTextMesh2.text.color = planet.planetRenderer.planetColor.ToRealColor();
			}
		}
		else
		{
			scrHitTextMesh2.Show(planet.transform.position + base.controller.camy.transform.up);
		}
	}
}
