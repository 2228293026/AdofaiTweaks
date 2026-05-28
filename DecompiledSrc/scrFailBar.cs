using System.Linq;
using UnityEngine;

public class scrFailBar : ADOClass
{
	private const float overloadCooldown = 0.4f;

	private const float overloadDamagePerMiss = 0.5f;

	private const float multipressCooldown = 0.2f;

	private const float multipressDamage = 0.35f;

	private const float multipressResetLimit = 6f;

	private readonly scrPlayer player;

	private float multipressCounter;

	private float multipressCooldownResetCounter;

	public float overloadCounter;

	public scrFailBar(scrPlayer player)
	{
		this.player = player;
		overloadCounter = 0f;
		multipressCounter = 0f;
	}

	public void Rewind()
	{
		overloadCounter = 0f;
		multipressCounter = 0f;
	}

	public void Update()
	{
		if (DidFail())
		{
			player.Die(overload: true, multipressCounter > 1f);
			overloadCounter = Mathf.Min(overloadCounter, 0.5f);
			multipressCounter = Mathf.Min(multipressCounter, 0.5f);
		}
		overloadCounter -= (float)(0.4000000059604645 * base.conductor.deltaSongPos / base.conductor.crotchetAtStart);
		overloadCounter = Mathf.Max(overloadCounter, 0f);
		multipressCounter -= (float)(0.20000000298023224 * base.conductor.deltaSongPos / base.conductor.crotchetAtStart);
		multipressCounter = Mathf.Max(multipressCounter, 0f);
		multipressCooldownResetCounter += (float)(1.0 * base.conductor.deltaSongPos / base.conductor.crotchetAtStart);
		if (multipressCooldownResetCounter > 6f)
		{
			multipressCounter = 0f;
			multipressCooldownResetCounter = 0f;
		}
	}

	public bool Damage(bool multipress = false)
	{
		if (!multipress)
		{
			Damage(0.5f);
		}
		else
		{
			multipressCounter += 0.35f;
			multipressCooldownResetCounter = 0f;
		}
		return DidFail();
	}

	public bool DidFail(bool checkForDuplicateDeath = true)
	{
		if (checkForDuplicateDeath && scrController.deathStates.Contains(base.controller.state))
		{
			return false;
		}
		bool num = overloadCounter > 1f || multipressCounter > 1f;
		bool flag = ADOBase.isOfficialLevel && (!base.controller.gameworld || base.controller.percentComplete >= 0.96f);
		if (num)
		{
			return !flag;
		}
		return false;
	}

	public void Damage(float amount)
	{
		if (GCS.d_drumcontroller)
		{
			amount /= 1.7f;
		}
		overloadCounter += amount;
	}
}
