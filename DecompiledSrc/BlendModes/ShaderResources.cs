using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlendModes;

public sealed class ShaderResources : ScriptableObject
{
	[Tooltip("Shaders currently included in the build.")]
	[SerializeField]
	private List<Shader> shaders = new List<Shader>();

	[Tooltip("Addtional shader paths to use when resolving available shader extensions. Should be relative to the project root, eg: `Assets/Shaders`.")]
	[SerializeField]
	private List<string> additionalPaths = new List<string>();

	private const string resourcesAssetPath = "BlendModes/ShaderResources";

	private static string[] cachedShaderFamilies;

	public static ShaderResources Load()
	{
		return Resources.Load<ShaderResources>("BlendModes/ShaderResources");
	}

	public static ResourceRequest LoadAsync()
	{
		return Resources.LoadAsync<ShaderResources>("BlendModes/ShaderResources");
	}

	public HashSet<Shader> GetShaders()
	{
		return new HashSet<Shader>(shaders);
	}

	public Shader GetShaderByName(string shaderName)
	{
		return shaders.Find((Shader s) => s.name == shaderName);
	}

	public bool ShaderExists(string shaderName)
	{
		return shaders.Exists((Shader s) => s.name == shaderName);
	}

	public string[] GetShaderFamilies()
	{
		if (cachedShaderFamilies != null)
		{
			return cachedShaderFamilies;
		}
		cachedShaderFamilies = (from s in GetShaders()
			select GetShaderFamily(s.name)).ToArray();
		return cachedShaderFamilies;
	}

	public bool FamilyImplementsGrab(string shaderFamily)
	{
		return shaders.Exists((Shader s) => GetShaderFamily(s.name) == shaderFamily && GetShaderVariant(s.name) == "Grab");
	}

	public bool FamilyImplementsOverlay(string shaderFamily)
	{
		return shaders.Exists((Shader s) => GetShaderFamily(s.name) == shaderFamily && GetShaderVariant(s.name) == "Overlay");
	}

	public bool FamilyImplementsUnifiedGrab(string shaderFamily)
	{
		return shaders.Exists((Shader s) => GetShaderFamily(s.name) == shaderFamily && GetShaderVariant(s.name) == "UnifiedGrab");
	}

	public bool FamilyImplementsFramebuffer(string shaderFamily)
	{
		return shaders.Exists((Shader s) => GetShaderFamily(s.name) == shaderFamily && GetShaderVariant(s.name) == "Framebuffer");
	}

	public bool FamilyImplementsMasking(string shaderFamily)
	{
		return shaders.Exists((Shader s) => GetShaderFamily(s.name) == shaderFamily && GetShaderVariant(s.name).EndsWith("Masked"));
	}

	public void AddShader(Shader shader)
	{
		if (!shaders.Exists((Shader s) => s.name == shader.name))
		{
			shaders.Add(shader);
			cachedShaderFamilies = null;
		}
	}

	public void RemoveShader(Shader shader)
	{
		shaders.RemoveAll((Shader s) => s.name == shader.name);
		cachedShaderFamilies = null;
	}

	public void RemoveAllShaders()
	{
		shaders.Clear();
		cachedShaderFamilies = null;
	}

	public List<string> GetAdditionalPaths()
	{
		return additionalPaths;
	}

	private static string GetShaderFamily(string shaderName)
	{
		return shaderName.Split('/', StringSplitOptions.None)[2];
	}

	private static string GetShaderVariant(string shaderName)
	{
		return shaderName.Split('/', StringSplitOptions.None)[3];
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void PreloadShaderFamilies()
	{
		ShaderResources shaderResources = Load();
		if ((bool)shaderResources)
		{
			shaderResources.GetShaderFamilies();
		}
	}
}
