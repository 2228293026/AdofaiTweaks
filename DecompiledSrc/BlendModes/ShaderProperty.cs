using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlendModes;

[Serializable]
public class ShaderProperty : IEquatable<ShaderProperty>
{
	[SerializeField]
	private string name;

	[SerializeField]
	private ShaderPropertyType type;

	[SerializeField]
	private Color colorValue = Color.white;

	[SerializeField]
	private Vector4 vectorValue = Vector4.zero;

	[SerializeField]
	private float floatValue;

	[SerializeField]
	private Texture textureValue;

	public string Name => name;

	public ShaderPropertyType Type => type;

	public Color ColorValue => colorValue;

	public Vector4 VectorValue => vectorValue;

	public float FloatValue => floatValue;

	public Texture TextureValue => textureValue;

	public ShaderProperty(string name, ShaderPropertyType type, object value)
	{
		this.name = name;
		this.type = type;
		SetValue(value);
	}

	public ShaderProperty(ShaderProperty shaderProperty)
	{
		name = shaderProperty.name;
		type = shaderProperty.type;
		colorValue = shaderProperty.colorValue;
		vectorValue = shaderProperty.vectorValue;
		floatValue = shaderProperty.floatValue;
		textureValue = shaderProperty.textureValue;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as ShaderProperty);
	}

	public bool Equals(ShaderProperty other)
	{
		if (other != null)
		{
			return name == other.name;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
	}

	public static bool operator ==(ShaderProperty property1, ShaderProperty property2)
	{
		return EqualityComparer<ShaderProperty>.Default.Equals(property1, property2);
	}

	public static bool operator !=(ShaderProperty property1, ShaderProperty property2)
	{
		return !(property1 == property2);
	}

	public void SetValue(object value)
	{
		if (ShaderUtilities.CheckPropertyValueType(value, Type))
		{
			switch (Type)
			{
			case ShaderPropertyType.Color:
				colorValue = (Color)value;
				break;
			case ShaderPropertyType.Vector:
				vectorValue = (Vector4)value;
				break;
			case ShaderPropertyType.Float:
				floatValue = (float)value;
				break;
			case ShaderPropertyType.Texture:
				textureValue = (Texture)value;
				break;
			}
		}
	}

	public void ApplyToMaterial(Material material)
	{
		if ((bool)material && material.HasProperty(Name))
		{
			switch (Type)
			{
			case ShaderPropertyType.Color:
				material.SetColor(Name, ColorValue);
				break;
			case ShaderPropertyType.Vector:
				material.SetVector(Name, VectorValue);
				break;
			case ShaderPropertyType.Float:
				material.SetFloat(Name, FloatValue);
				break;
			case ShaderPropertyType.Texture:
				material.SetTexture(Name, TextureValue);
				break;
			}
		}
	}
}
