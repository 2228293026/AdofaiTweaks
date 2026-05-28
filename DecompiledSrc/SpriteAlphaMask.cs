using UnityEngine;

public class SpriteAlphaMask : MonoBehaviour
{
	public static int ShaderProperty_Texture1 = Shader.PropertyToID("_Texture1");

	public static int ShaderProperty_DisableTexture1 = Shader.PropertyToID("_DisableTexture1");

	public static int ShaderProperty_Texture2 = Shader.PropertyToID("_Texture2");

	public static int ShaderProperty_Texture2Scale = Shader.PropertyToID("_Texture2Scale");

	public static int ShaderProperty_Texture2Pos = Shader.PropertyToID("_Texture2Pos");

	public static int ShaderProperty_Texture2Rot = Shader.PropertyToID("_Texture2Rot");

	public static int ShaderProperty_Texture3 = Shader.PropertyToID("_Texture3");

	public static int ShaderProperty_Texture3Scale = Shader.PropertyToID("_Texture3Scale");

	public static int ShaderProperty_Texture3Pos = Shader.PropertyToID("_Texture3Pos");

	public static int ShaderProperty_Texture3Rot = Shader.PropertyToID("_Texture3Rot");

	public static int ShaderProperty_DisableTexture3 = Shader.PropertyToID("_DisableTexture3");

	public static int ShaderProperty_Texture4 = Shader.PropertyToID("_Texture4");

	public static int ShaderProperty_Texture4Scale = Shader.PropertyToID("_Texture4Scale");

	public static int ShaderProperty_Texture4Pos = Shader.PropertyToID("_Texture4Pos");

	public static int ShaderProperty_Texture4Rot = Shader.PropertyToID("_Texture4Rot");

	public static int ShaderProperty_DisableTexture4 = Shader.PropertyToID("_DisableTexture4");

	public static int ShaderProperty_Texture5 = Shader.PropertyToID("_Texture5");

	public static int ShaderProperty_Texture5Scale = Shader.PropertyToID("_Texture5Scale");

	public static int ShaderProperty_Texture5Pos = Shader.PropertyToID("_Texture5Pos");

	public static int ShaderProperty_Texture5Rot = Shader.PropertyToID("_Texture5Rot");

	public static int ShaderProperty_DisableTexture5 = Shader.PropertyToID("_DisableTexture5");

	public static int ShaderProperty_RGB = Shader.PropertyToID("_MainTex");

	public static int ShaderProperty_A = Shader.PropertyToID("_AlphaTex");

	public static int ShaderProperty_ARot = Shader.PropertyToID("_ARot");

	public static int ShaderProperty_VisibleOutsideMask = Shader.PropertyToID("_VisibleOutsideMask");

	public Sprite sprite;

	public string targetTag;

	public Transform pivotTrans;

	public Transform childTrans;

	private int _lastDecoCacheUpdate;

	public Vector3 maskSize
	{
		get
		{
			Vector3 vector = sprite.bounds.size;
			if (pivotTrans != null)
			{
				vector = (Vector2)vector * (Vector2)pivotTrans.localScale;
			}
			return vector;
		}
	}
}
