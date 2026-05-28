using UnityEngine;

public class ColorCloud : ADOBase
{
	public GameObject goldSparks;

	public GameObject overseerParticles;

	private ParticleSystem particleSystem;

	private void Awake()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	public void SetSortingOrder(int order)
	{
		particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = order;
	}
}
