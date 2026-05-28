using System.Collections.Generic;
using UnityEngine;

namespace BlendModes;

public static class SharedMaterials
{
	private static List<Material> sharedMaterials = new List<Material>();

	public static Material GetSharedFor(Shader shader, BlendModeEffect blendModeEffect, out bool materialCreated)
	{
		materialCreated = false;
		string keyword = blendModeEffect.BlendMode.ToShaderKeyword();
		for (int i = 0; i < sharedMaterials.Count; i++)
		{
			Material material = sharedMaterials[i];
			if ((bool)material && material.shader == shader && material.IsKeywordEnabled(keyword))
			{
				return material;
			}
		}
		materialCreated = true;
		return CreateSharedMaterial(shader);
	}

	public static void DestroySharedMaterials()
	{
		for (int i = 0; i < sharedMaterials.Count; i++)
		{
			Material material = sharedMaterials[i];
			if ((bool)material)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(material);
				}
				else
				{
					Object.DestroyImmediate(material);
				}
			}
		}
		sharedMaterials.Clear();
	}

	private static Material CreateSharedMaterial(Shader shader)
	{
		Material material = new Material(shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		sharedMaterials.Add(material);
		return material;
	}
}
