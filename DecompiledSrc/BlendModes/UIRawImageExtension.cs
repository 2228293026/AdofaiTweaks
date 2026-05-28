using UnityEngine.UI;

namespace BlendModes;

[ExtendedComponent(typeof(RawImage))]
public class UIRawImageExtension : MaskableGraphicExtension<RawImage>
{
	private static ShaderProperty[] cachedDefaultProperties;

	public override string[] GetSupportedShaderFamilies()
	{
		return new string[2] { "UIDefault", "UIHsbc" };
	}

	public override ShaderProperty[] GetDefaultShaderProperties()
	{
		object obj = cachedDefaultProperties;
		if (obj == null)
		{
			obj = new ShaderProperty[4]
			{
				new ShaderProperty("_Hue", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Saturation", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Brightness", ShaderPropertyType.Float, 0),
				new ShaderProperty("_Contrast", ShaderPropertyType.Float, 0)
			};
			cachedDefaultProperties = (ShaderProperty[])obj;
		}
		return (ShaderProperty[])obj;
	}
}
