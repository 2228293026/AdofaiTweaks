using UnityEngine;

namespace BlendModes;

[ExtendedComponent(typeof(SpriteRenderer))]
public class SpriteRendererExtension : RendererExtension<SpriteRenderer>
{
	private static ShaderProperty[] cachedDefaultProperties;

	public override string[] GetSupportedShaderFamilies()
	{
		return new string[3] { "SpritesDefault", "SpritesHsbc", "SpritesVectorGradient" };
	}

	public override ShaderProperty[] GetDefaultShaderProperties()
	{
		object obj = cachedDefaultProperties;
		if (obj == null)
		{
			obj = new ShaderProperty[5]
			{
				new ShaderProperty("_Color", ShaderPropertyType.Color, Color.white),
				new ShaderProperty("_Hue", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Saturation", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Brightness", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Contrast", ShaderPropertyType.Float, 0)
			};
			cachedDefaultProperties = (ShaderProperty[])obj;
		}
		return (ShaderProperty[])obj;
	}

	protected override string GetDefaultShaderName()
	{
		return "Sprites/Default";
	}
}
