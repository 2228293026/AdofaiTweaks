using UnityEngine;

public class ChineseDLCLogo : MonoBehaviour
{
	public Mawaru_Sprite spr;

	private void Start()
	{
		spr.SetState(RDString.isChinese ? 1 : 0);
	}
}
