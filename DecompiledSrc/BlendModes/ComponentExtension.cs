using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BlendModes;

public abstract class ComponentExtension
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	protected sealed class ExtendedComponentAttribute : Attribute
	{
		public Type ExtendedComponentType { get; private set; }

		public ExtendedComponentAttribute(Type extendedComponentType)
		{
			ExtendedComponentType = extendedComponentType;
		}
	}

	private static Dictionary<Type, List<Type>> extensionsMap;

	private ComponentExtensionState state;

	public string[] SupportedShaderFamilies => GetSupportedShaderFamilies();

	public string DefaultShaderFamily => GetDefaultShaderFamily();

	public ShaderProperty[] DefaultShaderProperties => GetDefaultShaderProperties();

	public bool IsExtendedComponentValid
	{
		get
		{
			if (state != null)
			{
				return state.ExtendedComponent;
			}
			return false;
		}
	}

	protected BlendModeEffect BlendModeEffect
	{
		get
		{
			if (!IsExtendedComponentValid)
			{
				return null;
			}
			return state.ExtendedComponent.GetComponent<BlendModeEffect>();
		}
	}

	protected ShaderProperty[] ShaderProperties
	{
		get
		{
			if (state == null)
			{
				return new ShaderProperty[0];
			}
			return state.ShaderProperties;
		}
	}

	public static ComponentExtension CreateForObject(GameObject extendedObject, ComponentExtensionState state)
	{
		Dictionary<Type, List<Type>> extensionMap = GetExtensionMap();
		Dictionary<Type, List<Type>>.KeyCollection supportedTypes = extensionMap.Keys;
		Component component = extendedObject.GetComponents<Component>().FirstOrDefault((Component c) => supportedTypes.Contains(c.GetType()));
		if (component == null)
		{
			return null;
		}
		Type type = ResolveComponentExtensionType(component);
		if (type == null)
		{
			Debug.LogError($"Failed to resolve component extension type for '{component.GetType().FullName}'.");
			return null;
		}
		ComponentExtension componentExtension = Activator.CreateInstance(type) as ComponentExtension;
		componentExtension.state = state;
		componentExtension.state.ExtendedComponent = component;
		componentExtension.state.ShaderProperties = componentExtension.GetOverridedShaderProperties();
		componentExtension.OnEffectEnabled();
		return componentExtension;
	}

	public virtual bool IsValidFor(GameObject gameObject)
	{
		if (IsExtendedComponentValid)
		{
			return state.ExtendedComponent.gameObject == gameObject;
		}
		return false;
	}

	public virtual string GetDefaultShaderFamily()
	{
		return SupportedShaderFamilies[0];
	}

	public virtual ShaderProperty[] GetDefaultShaderProperties()
	{
		return new ShaderProperty[0];
	}

	public virtual bool MaterialHasProperty(string propertyName)
	{
		Material renderMaterial = GetRenderMaterial();
		if ((bool)renderMaterial)
		{
			return renderMaterial.HasProperty(propertyName);
		}
		return false;
	}

	public virtual ShaderProperty GetShaderProperty(string propertyName)
	{
		return ShaderProperties.FirstOrDefault((ShaderProperty p) => p.Name == propertyName);
	}

	public virtual void ApplyShaderProperties(Material material)
	{
		if (!(material == null))
		{
			ShaderProperty[] shaderProperties = ShaderProperties;
			for (int i = 0; i < shaderProperties.Length; i++)
			{
				shaderProperties[i].ApplyToMaterial(material);
			}
		}
	}

	public virtual bool AllowMaterialSharing()
	{
		return true;
	}

	public virtual MaterialPropertyBlock GetPropertyBlock()
	{
		return null;
	}

	public virtual void SetPropertyBlock(MaterialPropertyBlock propertyBlock)
	{
	}

	public abstract string[] GetSupportedShaderFamilies();

	public abstract Material GetRenderMaterial();

	public abstract void SetRenderMaterial(Material material);

	public virtual void OnEffectMaterialCreated(Material material)
	{
	}

	public virtual void OnEffectEnabled()
	{
	}

	public virtual void OnEffectDisabled()
	{
	}

	public virtual void OnEffectRenderImage(RenderTexture source, RenderTexture destination)
	{
	}

	public TComponent GetExtendedComponent<TComponent>() where TComponent : Component
	{
		return state.ExtendedComponent as TComponent;
	}

	protected void SetBlendMaterialDirty()
	{
		if ((bool)BlendModeEffect)
		{
			BlendModeEffect.SetMaterialDirty();
		}
	}

	protected virtual ShaderProperty[] GetOverridedShaderProperties()
	{
		Material renderMaterial = GetRenderMaterial();
		List<ShaderProperty> list = new List<ShaderProperty>();
		ShaderProperty[] defaultShaderProperties = DefaultShaderProperties;
		foreach (ShaderProperty defaultProperty in defaultShaderProperties)
		{
			ShaderProperty shaderProperty = null;
			if (state != null && state.ShaderProperties.Any((ShaderProperty p) => p.Name == defaultProperty.Name))
			{
				shaderProperty = new ShaderProperty(state.ShaderProperties.First((ShaderProperty p) => p.Name == defaultProperty.Name));
			}
			else
			{
				shaderProperty = new ShaderProperty(defaultProperty);
				if ((bool)renderMaterial && renderMaterial.HasProperty(shaderProperty.Name))
				{
					shaderProperty.SetValue(renderMaterial.GetProperty(shaderProperty.Type, shaderProperty.Name));
				}
			}
			list.Add(shaderProperty);
		}
		return list.ToArray();
	}

	private static Type ResolveComponentExtensionType(Component extendedComponent)
	{
		Dictionary<Type, List<Type>> extensionMap = GetExtensionMap();
		Type type = extendedComponent.GetType();
		List<Type> extensionTypes = extensionMap[type];
		return extensionTypes.Where((Type t1) => !extensionTypes.Exists((Type type2) => type2.BaseType == t1)).FirstOrDefault();
	}

	private static Dictionary<Type, List<Type>> GetExtensionMap()
	{
		if (extensionsMap != null)
		{
			return extensionsMap;
		}
		extensionsMap = new Dictionary<Type, List<Type>>();
		foreach (Type item in (from a in AppDomain.CurrentDomain.GetAssemblies()
			where !IsDynamicAssembly(a)
			select a).SelectMany((Assembly a) => a.GetExportedTypes()))
		{
			object[] customAttributes = item.GetCustomAttributes(typeof(ExtendedComponentAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				ExtendedComponentAttribute extendedComponentAttribute = customAttributes[0] as ExtendedComponentAttribute;
				if (!extensionsMap.ContainsKey(extendedComponentAttribute.ExtendedComponentType))
				{
					extensionsMap.Add(extendedComponentAttribute.ExtendedComponentType, new List<Type>());
				}
				extensionsMap[extendedComponentAttribute.ExtendedComponentType].Add(item);
			}
		}
		return extensionsMap;
	}

	private static bool IsDynamicAssembly(Assembly assembly)
	{
		return assembly.IsDynamic;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void PreloadExtensionMap()
	{
		GetExtensionMap();
	}
}
