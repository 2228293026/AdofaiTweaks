using UnityEngine;

[ExecuteInEditMode]
public class TNOFilter : MonoBehaviour
{
	public Shader SCShader;

	public Texture2D palette;

	public Texture2D palette2;

	[Range(0f, 1f)]
	public float interp;

	private Material SCMaterial;

	private Material material
	{
		get
		{
			if (SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return SCMaterial;
		}
	}

	private void Awake()
	{
		SCShader = Shader.Find("ADOFAI/TNOFilter");
		_ = material;
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			material.SetTexture("_Palette", palette);
			material.SetTexture("_Palette2", palette2);
			material.SetFloat("_Interp", interp);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		if ((bool)SCMaterial)
		{
			Object.DestroyImmediate(SCMaterial);
		}
	}
}
