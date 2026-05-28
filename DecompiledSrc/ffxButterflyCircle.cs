using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ffxButterflyCircle : ffxPlusBase
{
	private GameObject butterflyPrefab;

	public int butterflyCount = 12;

	public Color butterflyColor = Color.white;

	public bool randomColor;

	public bool planetColors;

	public override void Awake()
	{
		base.Awake();
		butterflyPrefab = ADOBase.gc.prefab_butterfly;
	}

	public override void StartEffect(scrPlanet planet)
	{
		List<scrPlayer> activePlayers = ADOBase.controller.playerManager.GetActivePlayers();
		float num = 1f / scrConductor.instance.song.pitch;
		for (int i = 0; i < butterflyCount; i++)
		{
			Color color = ((!randomColor) ? ((!planetColors) ? butterflyColor : ((!scrController.coopMode || activePlayers.Count <= 0) ? ADOBase.controller.playerOne.planetarySystem.allPlanets[i % 2].planetRenderer.planetColor.ToRealColor() : activePlayers[i % activePlayers.Count].planetarySystem.chosenPlanet.planetRenderer.planetColor.ToRealColor())) : Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f));
			Quaternion quaternion = Quaternion.Euler(0f, 0f, 360f / (float)butterflyCount * (float)i);
			GameObject newButterfly = Object.Instantiate(butterflyPrefab, base.transform.position, Quaternion.identity);
			newButterfly.GetComponent<SpriteRenderer>().color = color;
			Vector3 vector = newButterfly.transform.position + quaternion * new Vector3(3f, 0f, 0f);
			DOTween.Sequence().Append(newButterfly.transform.DOMove(vector, 0.75f * num).SetEase(Ease.OutExpo)).Append(newButterfly.transform.DOMoveY((newButterfly.transform.position + vector).y + 50f, 3f * num).SetEase(Ease.InSine))
				.AppendCallback(delegate
				{
					Object.Destroy(newButterfly);
				});
		}
	}
}
