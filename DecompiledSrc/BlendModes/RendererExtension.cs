using UnityEngine;

namespace BlendModes;

public abstract class RendererExtension<TComponent> : ComponentExtension where TComponent : Renderer
{
	private static Material defaultMaterial;

	protected string DefaultShaderName => GetDefaultShaderName();

	public override void OnEffectEnabled()
	{
		if (!defaultMaterial)
		{
			Shader shader = Shader.Find(DefaultShaderName);
			if (!shader)
			{
				Debug.LogWarning($"Failed to find `{DefaultShaderName}` default shader.");
			}
			else
			{
				defaultMaterial = new Material(shader);
			}
		}
	}

	public override void OnEffectDisabled()
	{
		SetRendererMaterials(defaultMaterial);
	}

	public override Material GetRenderMaterial()
	{
		if (!base.IsExtendedComponentValid)
		{
			return null;
		}
		Material[] sharedMaterials = GetExtendedComponent<TComponent>().sharedMaterials;
		if (sharedMaterials == null || sharedMaterials.Length == 0)
		{
			return null;
		}
		return sharedMaterials[0];
	}

	public override void SetRenderMaterial(Material renderMaterial)
	{
		SetRendererMaterials(renderMaterial);
	}

	protected abstract string GetDefaultShaderName();

	protected virtual void SetRendererMaterials(Material material)
	{
		if (!base.IsExtendedComponentValid)
		{
			return;
		}
		TComponent extendedComponent = GetExtendedComponent<TComponent>();
		if (extendedComponent.sharedMaterials == null)
		{
			extendedComponent.sharedMaterials = new Material[1] { material };
			return;
		}
		Material[] array = new Material[extendedComponent.sharedMaterials.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = material;
		}
		extendedComponent.sharedMaterials = array;
	}
}
