using System.Linq;
using UnityEngine;

namespace BlendModes;

[ExtendedComponent(typeof(MeshRenderer))]
public class MeshRendererVisualDecoExtension : MeshRendererExtension
{
	private static ShaderProperty[] cachedDefaultProperties;

	public override string[] GetSupportedShaderFamilies()
	{
		return base.GetSupportedShaderFamilies().Concat(new string[1] { "VisualDeco" }).ToArray();
	}
}
