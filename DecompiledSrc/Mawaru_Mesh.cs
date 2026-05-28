using UnityEngine;

public class Mawaru_Mesh : MonoBehaviour
{
	public MeshRenderer render;

	public float miscf;

	private void Awake()
	{
		render = base.gameObject.GetComponent<MeshRenderer>();
	}
}
