using UnityEngine;

public class VirtualAvatarCanvas : MonoBehaviour
{
	public static VirtualAvatarCanvas instance;

	public VirtualAvatar[] avatars;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (scrPlayerManager.instance != null && scrPlayerManager.playerCount == 1)
		{
			avatars[1].Show(show: false, instant: true);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			avatars[0].Show(show: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			avatars[0].Show(show: true);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			avatars[1].Show(show: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			avatars[1].Show(show: true);
		}
	}

	public void Win()
	{
		VirtualAvatar[] array = avatars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Win();
		}
	}

	public void Hit(int index)
	{
		if (index < avatars.Length)
		{
			avatars[index].Hit();
		}
	}

	public void Lose(int index)
	{
		if (index < avatars.Length)
		{
			avatars[index].Lose();
		}
	}

	public void Revive(int index)
	{
		if (index < avatars.Length)
		{
			avatars[index].Revive();
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
