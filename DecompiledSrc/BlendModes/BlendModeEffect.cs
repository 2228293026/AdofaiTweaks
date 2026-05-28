using System.Linq;
using UnityEngine;

namespace BlendModes;

[AddComponentMenu("Effects/Blend Mode")]
[ExecuteInEditMode]
public sealed class BlendModeEffect : MonoBehaviour
{
	[SerializeField]
	private string shaderFamily;

	[SerializeField]
	private BlendMode blendMode;

	[SerializeField]
	private RenderMode renderMode;

	[SerializeField]
	private MaskMode maskMode;

	[SerializeField]
	private MaskBehaviour maskBehaviour;

	[SerializeField]
	private Color overlayColor = Color.white;

	[SerializeField]
	private Texture overlayTexture;

	[SerializeField]
	private Vector2 overlayTextureOffset = Vector2.zero;

	[SerializeField]
	private Vector2 overlayTextureScale = Vector2.one;

	[SerializeField]
	private bool framebufferEnabled;

	[SerializeField]
	private bool unifiedGrabEnabled;

	[SerializeField]
	private bool shareMaterial;

	[SerializeField]
	private Material blendMaterial;

	[SerializeField]
	private ComponentExtensionState componentExtensionState = new ComponentExtensionState();

	private const int defaultStencilId = 1;

	private static ShaderResources cachedShaderResources;

	private bool isMaterialDirty = true;

	private ComponentExtension сomponentExtension;

	public string ShaderFamily
	{
		get
		{
			return shaderFamily;
		}
		set
		{
			SetShaderFamily(value);
		}
	}

	public BlendMode BlendMode
	{
		get
		{
			return blendMode;
		}
		set
		{
			SetBlendMode(value);
		}
	}

	public RenderMode RenderMode
	{
		get
		{
			return renderMode;
		}
		set
		{
			SetRenderMode(value);
		}
	}

	public MaskMode MaskMode
	{
		get
		{
			return maskMode;
		}
		set
		{
			SetMaskMode(value);
		}
	}

	public MaskBehaviour MaskBehaviour
	{
		get
		{
			return maskBehaviour;
		}
		set
		{
			SetMaskBehaviour(value);
		}
	}

	public Color OverlayColor
	{
		get
		{
			return overlayColor;
		}
		set
		{
			SetOverlayColor(value);
		}
	}

	public Texture OverlayTexture
	{
		get
		{
			return overlayTexture;
		}
		set
		{
			SetOverlayTexture(value);
		}
	}

	public Vector2 OverlayTextureOffset
	{
		get
		{
			return overlayTextureOffset;
		}
		set
		{
			SetOverlayTextureOffset(value);
		}
	}

	public Vector2 OverlayTextureScale
	{
		get
		{
			return overlayTextureScale;
		}
		set
		{
			SetOverlayTextureScale(value);
		}
	}

	public bool FramebufferEnabled
	{
		get
		{
			return framebufferEnabled;
		}
		set
		{
			SetFramebufferEnabled(value);
		}
	}

	public bool UnifiedGrabEnabled
	{
		get
		{
			return unifiedGrabEnabled;
		}
		set
		{
			SetUnifiedGrabEnabled(value);
		}
	}

	public bool ShareMaterial
	{
		get
		{
			return shareMaterial;
		}
		set
		{
			SetShareMaterial(value);
		}
	}

	public bool IsComponentExtensionValid
	{
		get
		{
			if (сomponentExtension != null)
			{
				return сomponentExtension.IsValidFor(base.gameObject);
			}
			return false;
		}
	}

	private static ShaderResources ShaderResources => cachedShaderResources ?? (cachedShaderResources = ShaderResources.Load());

	public bool IsShaderFamilySupported(string shaderFamily)
	{
		if (string.IsNullOrEmpty(shaderFamily) || сomponentExtension == null || !ShaderResources)
		{
			return false;
		}
		if (сomponentExtension.SupportedShaderFamilies.Contains(shaderFamily))
		{
			return ShaderResources.GetShaderFamilies().Contains(shaderFamily);
		}
		return false;
	}

	public TComponent GetComponentExtension<TComponent>() where TComponent : ComponentExtension
	{
		return сomponentExtension as TComponent;
	}

	public void SetShaderFamily(string shaderFamily)
	{
		if (!(this.shaderFamily == shaderFamily))
		{
			this.shaderFamily = shaderFamily;
			SetMaterialDirty();
		}
	}

	public void SetBlendMode(BlendMode blendMode)
	{
		if (this.blendMode != blendMode)
		{
			this.blendMode = blendMode;
			SetMaterialDirty();
		}
	}

	public void SetRenderMode(RenderMode renderMode)
	{
		if (this.renderMode != renderMode)
		{
			this.renderMode = renderMode;
			SetMaterialDirty();
		}
	}

	public void SetMaskMode(MaskMode maskMode)
	{
		if (this.maskMode != maskMode)
		{
			this.maskMode = maskMode;
			SetMaterialDirty();
		}
	}

	public void SetMaskBehaviour(MaskBehaviour maskBehaviour)
	{
		if (this.maskBehaviour != maskBehaviour)
		{
			this.maskBehaviour = maskBehaviour;
			SetMaterialDirty();
		}
	}

	public void SetOverlayColor(Color color)
	{
		if (!(overlayColor == color))
		{
			overlayColor = color;
			SetMaterialDirty();
		}
	}

	public void SetOverlayTexture(Texture texture)
	{
		if (!(overlayTexture == texture))
		{
			overlayTexture = texture;
			SetMaterialDirty();
		}
	}

	public void SetOverlayTextureOffset(Vector2 offset)
	{
		if (!(overlayTextureOffset == offset))
		{
			overlayTextureOffset = offset;
			SetMaterialDirty();
		}
	}

	public void SetOverlayTextureScale(Vector2 scale)
	{
		if (!(overlayTextureScale == scale))
		{
			overlayTextureScale = scale;
			SetMaterialDirty();
		}
	}

	public void SetFramebufferEnabled(bool enabled)
	{
		if (framebufferEnabled != enabled)
		{
			framebufferEnabled = enabled;
			SetMaterialDirty();
		}
	}

	public void SetUnifiedGrabEnabled(bool enabled)
	{
		if (unifiedGrabEnabled != enabled)
		{
			unifiedGrabEnabled = enabled;
			SetMaterialDirty();
		}
	}

	public void SetShareMaterial(bool value)
	{
		if (shareMaterial != value)
		{
			shareMaterial = value;
			SetMaterialProperties(forceCreateMaterial: true);
		}
	}

	public void SetMaterialDirty()
	{
		isMaterialDirty = true;
	}

	public void InitializeComponentExtension()
	{
		bool materialProperties = false;
		if (!IsComponentExtensionValid)
		{
			сomponentExtension = ComponentExtension.CreateForObject(base.gameObject, componentExtensionState);
			materialProperties = true;
		}
		if (сomponentExtension != null)
		{
			сomponentExtension.OnEffectEnabled();
			if (string.IsNullOrEmpty(ShaderFamily))
			{
				ShaderFamily = сomponentExtension.DefaultShaderFamily;
			}
			SetMaterialProperties(materialProperties);
		}
	}

	private void SetMaterialProperties(bool forceCreateMaterial = false)
	{
		if (!base.enabled || сomponentExtension == null || !ShaderResources || !IsShaderFamilySupported(ShaderFamily))
		{
			return;
		}
		bool flag = MaskMode != MaskMode.Disabled;
		string text = ShaderUtilities.BuildShaderName(ShaderFamily, RenderMode, flag, FramebufferEnabled, UnifiedGrabEnabled);
		if (!ShaderResources.ShaderExists(text))
		{
			return;
		}
		int num;
		if (ShareMaterial)
		{
			num = (сomponentExtension.AllowMaterialSharing() ? 1 : 0);
			if (num != 0)
			{
				Shader shaderByName = ShaderResources.GetShaderByName(text);
				bool materialCreated = false;
				blendMaterial = SharedMaterials.GetSharedFor(shaderByName, this, out materialCreated);
				if (materialCreated)
				{
					сomponentExtension.OnEffectMaterialCreated(blendMaterial);
				}
				goto IL_0106;
			}
		}
		else
		{
			num = 0;
		}
		if (!blendMaterial || blendMaterial.shader.name != text || forceCreateMaterial)
		{
			Shader shaderByName2 = ShaderResources.GetShaderByName(text);
			blendMaterial = CreateBlendMaterial(shaderByName2);
			сomponentExtension.OnEffectMaterialCreated(blendMaterial);
		}
		goto IL_0106;
		IL_0106:
		ShaderUtilities.SelectBlendModeKeyword(blendMaterial, BlendMode);
		MaterialPropertyBlock materialPropertyBlock = ((num != 0) ? сomponentExtension.GetPropertyBlock() : null);
		if (RenderMode == RenderMode.TextureWithSelf)
		{
			SetOverlayMaterialProperties(blendMaterial, materialPropertyBlock);
		}
		if (flag)
		{
			SetMaskMaterialProperties(blendMaterial, materialPropertyBlock);
		}
		if (materialPropertyBlock != null)
		{
			сomponentExtension.SetPropertyBlock(materialPropertyBlock);
		}
		сomponentExtension.ApplyShaderProperties(blendMaterial);
		сomponentExtension.SetRenderMaterial(blendMaterial);
		isMaterialDirty = false;
	}

	private void SetOverlayMaterialProperties(Material material, MaterialPropertyBlock block)
	{
		if (block != null)
		{
			block.SetColor(ShaderUtilities.OverlayColorPropertyId, OverlayColor);
			block.SetTexture(ShaderUtilities.OverlayTexturePropertyId, (!OverlayTexture) ? Texture2D.whiteTexture : OverlayTexture);
			block.SetVector(ShaderUtilities.OverlayTextureSTPropertyId, new Vector4(OverlayTextureScale.x, OverlayTextureScale.y, OverlayTextureOffset.x, OverlayTextureOffset.y));
		}
		else
		{
			material.SetColor(ShaderUtilities.OverlayColorPropertyId, OverlayColor);
			material.SetTexture(ShaderUtilities.OverlayTexturePropertyId, OverlayTexture);
			material.SetTextureOffset(ShaderUtilities.OverlayTexturePropertyId, OverlayTextureOffset);
			material.SetTextureScale(ShaderUtilities.OverlayTexturePropertyId, OverlayTextureScale);
		}
	}

	private void SetMaskMaterialProperties(Material material, MaterialPropertyBlock block)
	{
		int num = ((MaskMode == MaskMode.NothingButMask) ? 3 : 6);
		int num2 = ((MaskBehaviour == MaskBehaviour.Cutout) ? 1 : ((MaskMode == MaskMode.NothingButMask) ? 6 : 3));
		if (block != null)
		{
			block.SetFloat(ShaderUtilities.BlendStencilCompPropertyId, num);
			block.SetFloat(ShaderUtilities.NormalStencilCompPropertyId, num2);
			block.SetFloat(ShaderUtilities.StencilIdPropertyId, 1f);
		}
		else
		{
			material.SetFloat(ShaderUtilities.BlendStencilCompPropertyId, num);
			material.SetFloat(ShaderUtilities.NormalStencilCompPropertyId, num2);
			material.SetFloat(ShaderUtilities.StencilIdPropertyId, 1f);
		}
	}

	private static Material CreateBlendMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = (HideFlags.HideInHierarchy | HideFlags.NotEditable)
		};
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void PreloadShaderResources()
	{
		cachedShaderResources = ShaderResources.Load();
	}

	private void Reset()
	{
		InitializeComponentExtension();
	}

	private void Awake()
	{
		InitializeComponentExtension();
	}

	private void OnEnable()
	{
		InitializeComponentExtension();
	}

	private void OnDisable()
	{
		if (сomponentExtension != null)
		{
			сomponentExtension.OnEffectDisabled();
		}
	}

	private void OnValidate()
	{
		SetMaterialDirty();
	}

	private void OnDidApplyAnimationProperties()
	{
		SetMaterialDirty();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (сomponentExtension != null)
		{
			сomponentExtension.OnEffectRenderImage(source, destination);
		}
	}

	private void Update()
	{
		if (isMaterialDirty)
		{
			SetMaterialProperties();
		}
	}
}
