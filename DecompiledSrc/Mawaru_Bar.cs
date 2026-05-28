using UnityEngine;

public class Mawaru_Bar : MonoBehaviour
{
	public Mawaru_Sprite backBar;

	public Mawaru_Sprite frontBar;

	public GameObject barFill;

	public void Enable()
	{
		backBar.render.enabled = true;
		backBar.render.material.SetColor("_Color", Color.white);
		frontBar.render.enabled = true;
		frontBar.render.material.SetColor("_Color", Color.white);
	}
}
