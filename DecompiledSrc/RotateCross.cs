using UnityEngine;

public class RotateCross : ADOBase
{
	public Vector2 speed;

	public MeshRenderer meshRenderer;

	private Material material;

	private void Awake()
	{
		meshRenderer.material = new Material(meshRenderer.material);
		material = meshRenderer.material;
	}

	private void Update()
	{
		float unscaledTime = Time.unscaledTime;
		material.SetTextureOffset("_MainTex", new Vector2(speed.x * unscaledTime, speed.y * unscaledTime));
	}
}
