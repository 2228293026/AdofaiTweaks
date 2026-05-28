using Rewired;
using UnityEngine;

public class RewiredControllerTest : MonoBehaviour
{
	private void Start()
	{
	}

	private void LateUpdate()
	{
		int num = 0;
		ReInput.players.GetPlayer(num);
	}
}
