using UnityEngine;
using UnityEngine.UI;

namespace BlendModes;

public abstract class MaskableGraphicExtension<TComponent> : ComponentExtension where TComponent : MaskableGraphic
{
	protected Material DefaultMaterial => GetDefaultMaterial();

	public override void OnEffectEnabled()
	{
		if (base.IsExtendedComponentValid)
		{
			GetExtendedComponent<TComponent>().RegisterDirtyMaterialCallback(base.SetBlendMaterialDirty);
		}
	}

	public override void OnEffectDisabled()
	{
		if (base.IsExtendedComponentValid)
		{
			GetExtendedComponent<TComponent>().UnregisterDirtyMaterialCallback(base.SetBlendMaterialDirty);
			GetExtendedComponent<TComponent>().material = DefaultMaterial;
		}
	}

	public override Material GetRenderMaterial()
	{
		if (!base.IsExtendedComponentValid)
		{
			return null;
		}
		return GetExtendedComponent<TComponent>().materialForRendering;
	}

	public override void SetRenderMaterial(Material renderMaterial)
	{
		if (base.IsExtendedComponentValid)
		{
			TComponent extendedComponent = GetExtendedComponent<TComponent>();
			extendedComponent.material = renderMaterial ?? DefaultMaterial;
			extendedComponent.enabled = false;
			extendedComponent.enabled = true;
		}
	}

	protected virtual Material GetDefaultMaterial()
	{
		if (!base.IsExtendedComponentValid)
		{
			return null;
		}
		return GetExtendedComponent<TComponent>().defaultMaterial;
	}
}
