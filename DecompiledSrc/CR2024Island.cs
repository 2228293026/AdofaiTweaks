using System;
using UnityEngine;

public class CR2024Island : ADOBase
{
	[Serializable]
	private class CR2024LevelSet
	{
		public CR2024Level classic;

		public CR2024Level tech;
	}

	[Serializable]
	private class CR2024Level
	{
		public GameObject portal;

		public GameObject playFloor;

		public GameObject speedFloor;
	}

	[SerializeField]
	private CR2024LevelSet[] levels;

	[SerializeField]
	private GameObject classicGem;

	[SerializeField]
	private GameObject techGem;

	private bool techMode;

	private float portalY;

	private void Start()
	{
		if (GCS.FOOL_JOKER)
		{
			return;
		}
		portalY = levels[0].classic.portal.transform.position.y;
		ChangeDifficulty(GCS.worldEntrance.IsTechWorld());
		CR2024LevelSet[] array = levels;
		foreach (CR2024LevelSet cR2024LevelSet in array)
		{
			if ((bool)cR2024LevelSet.tech.portal)
			{
				scrGfxFloat componentInChildren = cR2024LevelSet.classic.portal.GetComponentInChildren<scrGfxFloat>();
				cR2024LevelSet.tech.portal.GetComponentInChildren<scrGfxFloat>().swingoffsetpct = componentInChildren.swingoffsetpct;
			}
		}
	}

	private void ChangeDifficulty(bool tech = false)
	{
		techMode = tech;
		ADOBase.controller.camy.FlashPlus();
		bool flag = true;
		CR2024LevelSet[] array = levels;
		foreach (CR2024LevelSet cR2024LevelSet in array)
		{
			scrPortal component = cR2024LevelSet.classic.portal.GetComponent<scrPortal>();
			scrPortal scrPortal2 = (cR2024LevelSet.tech.portal ? cR2024LevelSet.tech.portal.GetComponent<scrPortal>() : null);
			bool flag2 = Persistence.IsWorldComplete(component.world);
			bool flag3 = (bool)scrPortal2 && Persistence.IsWorldComplete(scrPortal2.world);
			cR2024LevelSet.classic.portal.transform.MoveY((!techMode) ? portalY : (portalY + 10f));
			cR2024LevelSet.classic.playFloor.SetActive(flag && !techMode);
			cR2024LevelSet.classic.speedFloor.SetActive(flag && !techMode && flag2);
			component.LockWorld(!flag);
			flag = flag2;
			if ((bool)cR2024LevelSet.tech.portal)
			{
				cR2024LevelSet.tech.portal.transform.MoveY(techMode ? portalY : (portalY + 10f));
				cR2024LevelSet.tech.playFloor.SetActive(flag2 && techMode);
				cR2024LevelSet.tech.speedFloor.SetActive(flag2 && techMode && flag3);
				scrPortal2.LockWorld(!flag2);
			}
		}
		classicGem.SetActive(techMode);
		techGem.SetActive(Persistence.clearedTechFeatured && !techMode);
	}

	public void ToggleDifficulty()
	{
		ChangeDifficulty(!techMode);
	}
}
