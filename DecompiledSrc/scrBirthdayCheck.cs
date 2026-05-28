using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrBirthdayCheck : ADOBase
{
	public GameObject birthdayParticles;

	private void Update()
	{
		if (DiscordController.isBirthday && !birthdayParticles.activeSelf)
		{
			GetComponent<Text>().text = RDString.Get("levelSelect.happyBirthday", new Dictionary<string, object> { 
			{
				"user",
				DiscordController.currentUsername
			} });
			birthdayParticles.SetActive(value: true);
		}
	}
}
