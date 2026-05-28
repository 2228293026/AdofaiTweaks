using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlendModes;

public static class ShaderUtilities
{
	public const string OverlayColorPropertyName = "_BLENDMODES_OverlayColor";

	public const string OverlayTexturePropertyName = "_BLENDMODES_OverlayTexture";

	public const string OverlayTextureSTPropertyName = "_BLENDMODES_OverlayTexture_ST";

	public const string StencilIdPropertyName = "_BLENDMODES_StencilId";

	public const string BlendStencilCompPropertyName = "_BLENDMODES_BlendStencilComp";

	public const string NormalStencilCompPropertyName = "_BLENDMODES_NormalStencilComp";

	public static readonly int OverlayColorPropertyId = Shader.PropertyToID("_BLENDMODES_OverlayColor");

	public static readonly int OverlayTexturePropertyId = Shader.PropertyToID("_BLENDMODES_OverlayTexture");

	public static readonly int OverlayTextureSTPropertyId = Shader.PropertyToID("_BLENDMODES_OverlayTexture_ST");

	public static readonly int StencilIdPropertyId = Shader.PropertyToID("_BLENDMODES_StencilId");

	public static readonly int BlendStencilCompPropertyId = Shader.PropertyToID("_BLENDMODES_BlendStencilComp");

	public static readonly int NormalStencilCompPropertyId = Shader.PropertyToID("_BLENDMODES_NormalStencilComp");

	private const string modeKeywordPrefix = "BLENDMODES_MODE_";

	private static Dictionary<BlendMode, string> cachedShaderKeywords;

	public static string BuildShaderName(string shaderFamily, RenderMode renderMode, bool maskEnabled, bool framebufferEnabled, bool unifiedGrabEnabled)
	{
		string text = string.Empty;
		switch (renderMode)
		{
		case RenderMode.SelfWithScreen:
			text = ((framebufferEnabled && Application.platform == RuntimePlatform.IPhonePlayer) ? "Framebuffer" : ((!unifiedGrabEnabled) ? "Grab" : "UnifiedGrab"));
			break;
		case RenderMode.TextureWithSelf:
			text = "Overlay";
			break;
		}
		if (maskEnabled)
		{
			text += "Masked";
		}
		return $"Hidden/BlendModes/{shaderFamily}/{text}";
	}

	public static string ToShaderKeyword(this BlendMode blendMode)
	{
		if (cachedShaderKeywords != null)
		{
			return cachedShaderKeywords[blendMode];
		}
		cachedShaderKeywords = new Dictionary<BlendMode, string>();
		BlendMode[] array = (BlendMode[])Enum.GetValues(typeof(BlendMode));
		foreach (BlendMode blendMode2 in array)
		{
			cachedShaderKeywords[blendMode2] = ("BLENDMODES_MODE_" + blendMode2).ToUpperInvariant();
		}
		return cachedShaderKeywords[blendMode];
	}

	public static void SelectBlendModeKeyword(Material material, BlendMode blendMode)
	{
		string[] shaderKeywords = material.shaderKeywords;
		foreach (string text in shaderKeywords)
		{
			if (text.StartsWith("BLENDMODES_MODE_", StringComparison.InvariantCulture))
			{
				material.DisableKeyword(text);
			}
		}
		material.EnableKeyword(blendMode.ToShaderKeyword());
	}

	public static void TransferMaterialProperty(int propertyId, ShaderPropertyType propertyType, Material fromMaterial, Material toMaterial)
	{
		if ((bool)fromMaterial && (bool)toMaterial && fromMaterial.HasProperty(propertyId) && toMaterial.HasProperty(propertyId))
		{
			switch (propertyType)
			{
			case ShaderPropertyType.Color:
				toMaterial.SetColor(propertyId, fromMaterial.GetColor(propertyId));
				break;
			case ShaderPropertyType.Vector:
				toMaterial.SetVector(propertyId, fromMaterial.GetVector(propertyId));
				break;
			case ShaderPropertyType.Float:
				toMaterial.SetFloat(propertyId, fromMaterial.GetFloat(propertyId));
				break;
			case ShaderPropertyType.Texture:
				toMaterial.SetTexture(propertyId, fromMaterial.GetTexture(propertyId));
				break;
			}
		}
	}

	public static void TransferMaterialProperty(string propertyName, ShaderPropertyType propertyType, Material fromMaterial, Material toMaterial)
	{
		TransferMaterialProperty(Shader.PropertyToID(propertyName), propertyType, fromMaterial, toMaterial);
	}

	public static bool CheckPropertyValueType(object value, ShaderPropertyType type)
	{
		if (value == null)
		{
			return false;
		}
		return type switch
		{
			ShaderPropertyType.Color => value is Color, 
			ShaderPropertyType.Vector => value is Vector4, 
			ShaderPropertyType.Float => value is float, 
			ShaderPropertyType.Texture => value is Texture, 
			_ => false, 
		};
	}

	public static void SetProperty(this Material material, ShaderPropertyType type, string name, object value)
	{
		if ((bool)material && material.HasProperty(name) && CheckPropertyValueType(value, type))
		{
			switch (type)
			{
			case ShaderPropertyType.Color:
				material.SetColor(name, (Color)value);
				break;
			case ShaderPropertyType.Vector:
				material.SetVector(name, (Vector4)value);
				break;
			case ShaderPropertyType.Float:
				material.SetFloat(name, (float)value);
				break;
			case ShaderPropertyType.Texture:
				material.SetTexture(name, (Texture)value);
				break;
			}
		}
	}

	public static object GetProperty(this Material material, ShaderPropertyType type, string name)
	{
		if (!material || !material.HasProperty(name))
		{
			return null;
		}
		return type switch
		{
			ShaderPropertyType.Color => material.GetColor(name), 
			ShaderPropertyType.Vector => material.GetVector(name), 
			ShaderPropertyType.Float => material.GetFloat(name), 
			ShaderPropertyType.Texture => material.GetTexture(name), 
			_ => null, 
		};
	}
}
