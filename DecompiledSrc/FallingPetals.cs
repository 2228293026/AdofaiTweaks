using UnityEngine;

public class FallingPetals : MonoBehaviour
{
	public ParticleSystem[] petalParticles;

	public GameObject petalsHolder;

	private void LateUpdate()
	{
		if (petalsHolder != null && petalsHolder.activeSelf)
		{
			UpdatePetalGravity();
		}
	}

	private void OnDisable()
	{
		TogglePetals(enabled: false);
	}

	public void TogglePetals(bool enabled, float startTime = 0f)
	{
		petalsHolder.SetActive(enabled);
		if (enabled)
		{
			UpdatePetalGravity();
			ParticleSystem[] array = petalParticles;
			foreach (ParticleSystem obj in array)
			{
				obj.Simulate(startTime);
				obj.Play();
			}
		}
	}

	private void UpdatePetalGravity()
	{
		ParticleSystem[] array = petalParticles;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.MainModule main = array[i].main;
			main.gravityModifierMultiplier = petalsHolder.transform.localScale.x / 10f;
		}
	}
}
