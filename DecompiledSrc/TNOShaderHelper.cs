using UnityEngine;
using UnityEngine.Rendering;

public class TNOShaderHelper : MonoBehaviour
{
	public bool additiveBlend;

	public bool noBlend;

	private Material material;

	private void Awake()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (!(component == null))
		{
			material = component.material;
			BlendMode value = (noBlend ? BlendMode.One : (additiveBlend ? BlendMode.One : BlendMode.SrcAlpha));
			BlendMode value2 = ((!noBlend) ? (additiveBlend ? BlendMode.One : BlendMode.OneMinusSrcAlpha) : BlendMode.Zero);
			material.SetInt("_SrcBlend", (int)value);
			material.SetInt("_DstBlend", (int)value2);
		}
	}
}
