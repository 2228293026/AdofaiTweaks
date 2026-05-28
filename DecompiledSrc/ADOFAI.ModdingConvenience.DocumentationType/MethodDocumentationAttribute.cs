using System;

namespace ADOFAI.ModdingConvenience.DocumentationType;

public class MethodDocumentationAttribute : Attribute
{
	internal MethodDocumentationAttribute(string summary, string[] parameters = null, string returns = null, string[] exceptions = null)
	{
	}
}
