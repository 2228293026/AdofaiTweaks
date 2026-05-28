using System;
using UnityEngine;

namespace BlendModes;

[Serializable]
public class ComponentExtensionState
{
	[SerializeField]
	private Component extendedComponent;

	[SerializeField]
	private ShaderProperty[] shaderProperties = new ShaderProperty[0];

	public Component ExtendedComponent
	{
		get
		{
			return extendedComponent;
		}
		set
		{
			extendedComponent = value;
		}
	}

	public ShaderProperty[] ShaderProperties
	{
		get
		{
			return shaderProperties;
		}
		set
		{
			shaderProperties = value;
		}
	}
}
