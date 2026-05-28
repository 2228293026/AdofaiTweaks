using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SpriteAlphaMaskUtils
{
	private static int lastCache;

	private static List<SpriteAlphaMask> alphaMaskCache = new List<SpriteAlphaMask>();

	private static Dictionary<string, List<SpriteAlphaMask>> taggedAlphaMaskCache = new Dictionary<string, List<SpriteAlphaMask>>();

	public static bool doRefreshMaskCache = false;

	public static void RefreshMaskCache()
	{
		if (lastCache >= Time.frameCount)
		{
			return;
		}
		alphaMaskCache.Clear();
		taggedAlphaMaskCache.Clear();
		alphaMaskCache.AddRange(Object.FindObjectsByType<SpriteAlphaMask>(FindObjectsInactive.Include, FindObjectsSortMode.None));
		foreach (SpriteAlphaMask item in alphaMaskCache)
		{
			if (!(item.targetTag == "NO TAG"))
			{
				(CollectionExtensions.GetValueOrDefault<string, List<SpriteAlphaMask>>((IReadOnlyDictionary<string, List<SpriteAlphaMask>>)taggedAlphaMaskCache, item.targetTag) ?? (taggedAlphaMaskCache[item.targetTag] = new List<SpriteAlphaMask>())).Add(item);
			}
		}
		lastCache = Time.frameCount;
		doRefreshMaskCache = false;
	}

	public static void UpdateAlphaMaskCache(this scrVisualDecoration obj, bool force = false)
	{
		if (!force && obj.alphaMaskCache != null)
		{
			return;
		}
		HashSet<SpriteAlphaMask> hashSet = null;
		foreach (string tag in obj.tags)
		{
			if (hashSet != null)
			{
				hashSet.UnionWith(CollectionExtensions.GetValueOrDefault<string, List<SpriteAlphaMask>>((IReadOnlyDictionary<string, List<SpriteAlphaMask>>)taggedAlphaMaskCache, tag) ?? new List<SpriteAlphaMask>());
			}
			else
			{
				hashSet = CollectionExtensions.GetValueOrDefault<string, List<SpriteAlphaMask>>((IReadOnlyDictionary<string, List<SpriteAlphaMask>>)taggedAlphaMaskCache, tag)?.ToHashSet();
			}
		}
		obj.alphaMaskCache = (hashSet ?? new HashSet<SpriteAlphaMask>()).Where((SpriteAlphaMask x) => x.gameObject.activeSelf).ToList();
	}

	public static void ApplyAlphaMaskToRT(this scrVisualDecoration obj, Vector3 objSize, RenderTexture pre, RenderTexture rt, bool visibleOutsideMask = false)
	{
		if (scnGame.instance == null)
		{
			return;
		}
		Transform transform = obj.transform;
		Vector3 objPos = transform.position;
		float objRot = transform.eulerAngles.z;
		obj.UpdateAlphaMaskCache();
		List<SpriteAlphaMask> list = obj.alphaMaskCache.FindAll((SpriteAlphaMask mask) => OBBCollision.CheckTouched(objPos, objSize, objRot, mask.transform.position, mask.maskSize, mask.pivotTrans.eulerAngles.z));
		int count = list.Count;
		Material mergeSprite = RDC.data.mergeSprite;
		if (obj.spareRT1?.width != pre.width || obj.spareRT1?.height != pre.height)
		{
			if (obj.spareRT1 != null)
			{
				obj.spareRT1.Release();
			}
			if (obj.spareRT2 != null)
			{
				obj.spareRT2.Release();
			}
			obj.spareRT1 = new RenderTexture(pre.width, pre.height, pre.depth, pre.format);
			obj.spareRT2 = new RenderTexture(pre.width, pre.height, pre.depth, pre.format);
		}
		RenderTexture renderTexture = obj.spareRT1;
		RenderTexture renderTexture2 = obj.spareRT2;
		bool flag = true;
		for (int num = 0; num < count; num++)
		{
			SpriteAlphaMask spriteAlphaMask = list[num];
			Transform pivotTrans = spriteAlphaMask.pivotTrans;
			Transform childTrans = spriteAlphaMask.childTrans;
			Vector3 obj2 = ((childTrans != null) ? childTrans.position : pivotTrans.position);
			float z = pivotTrans.eulerAngles.z;
			Vector3 maskSize = spriteAlphaMask.maskSize;
			Vector2 vector = (Vector2)objSize / (Vector2)maskSize;
			Vector3 vector2 = obj2 - maskSize / 2f;
			Vector3 vector3 = objPos - objSize / 2f;
			Vector2 vector4 = (Vector2)(vector3 - vector2) / (Vector2)maskSize;
			mergeSprite.SetTexture(SpriteAlphaMask.ShaderProperty_Texture1, flag ? null : renderTexture);
			mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture1, flag ? 1 : 0);
			mergeSprite.SetTexture(SpriteAlphaMask.ShaderProperty_Texture2, spriteAlphaMask.sprite.texture);
			mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture2Scale, vector);
			mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture2Pos, vector4);
			mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_Texture2Rot, z);
			if (num < count - 1)
			{
				num++;
				SpriteAlphaMask spriteAlphaMask2 = list[num];
				Transform pivotTrans2 = spriteAlphaMask2.pivotTrans;
				Transform childTrans2 = spriteAlphaMask2.childTrans;
				Vector3 obj3 = ((childTrans2 != null) ? childTrans2.position : pivotTrans2.position);
				float z2 = pivotTrans2.eulerAngles.z;
				Vector3 maskSize2 = spriteAlphaMask2.maskSize;
				Vector2 vector5 = (Vector2)objSize / (Vector2)maskSize2;
				Vector3 vector6 = obj3 - maskSize2 / 2f;
				Vector2 vector7 = (Vector2)(vector3 - vector6) / (Vector2)maskSize2;
				mergeSprite.SetTexture(SpriteAlphaMask.ShaderProperty_Texture3, spriteAlphaMask2.sprite.texture);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture3Scale, vector5);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture3Pos, vector7);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_Texture3Rot, z2);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture3, 0f);
			}
			else
			{
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture3, 1f);
			}
			if (num < count - 1)
			{
				num++;
				SpriteAlphaMask spriteAlphaMask3 = list[num];
				Transform pivotTrans3 = spriteAlphaMask3.pivotTrans;
				Transform childTrans3 = spriteAlphaMask3.childTrans;
				Vector3 obj4 = ((childTrans3 != null) ? childTrans3.position : pivotTrans3.position);
				float z3 = pivotTrans3.eulerAngles.z;
				Vector3 maskSize3 = spriteAlphaMask3.maskSize;
				Vector2 vector8 = (Vector2)objSize / (Vector2)maskSize3;
				Vector3 vector9 = obj4 - maskSize3 / 2f;
				Vector2 vector10 = (Vector2)(vector3 - vector9) / (Vector2)maskSize3;
				mergeSprite.SetTexture(SpriteAlphaMask.ShaderProperty_Texture4, spriteAlphaMask3.sprite.texture);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture4Scale, vector8);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture4Pos, vector10);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_Texture4Rot, z3);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture4, 0f);
			}
			else
			{
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture4, 1f);
			}
			if (num < count - 1)
			{
				num++;
				SpriteAlphaMask spriteAlphaMask4 = list[num];
				Transform pivotTrans4 = spriteAlphaMask4.pivotTrans;
				Transform childTrans4 = spriteAlphaMask4.childTrans;
				Vector3 obj5 = ((childTrans4 != null) ? childTrans4.position : pivotTrans4.position);
				float z4 = pivotTrans4.eulerAngles.z;
				Vector3 maskSize4 = spriteAlphaMask4.maskSize;
				Vector2 vector11 = (Vector2)objSize / (Vector2)maskSize4;
				Vector3 vector12 = obj5 - maskSize4 / 2f;
				Vector2 vector13 = (Vector2)(vector3 - vector12) / (Vector2)maskSize4;
				mergeSprite.SetTexture(SpriteAlphaMask.ShaderProperty_Texture5, spriteAlphaMask4.sprite.texture);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture5Scale, vector11);
				mergeSprite.SetVector(SpriteAlphaMask.ShaderProperty_Texture5Pos, vector13);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_Texture5Rot, z4);
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture5, 0f);
			}
			else
			{
				mergeSprite.SetFloat(SpriteAlphaMask.ShaderProperty_DisableTexture5, 1f);
			}
			Graphics.Blit(renderTexture, renderTexture2, mergeSprite, 0);
			RenderTexture renderTexture3 = renderTexture2;
			RenderTexture renderTexture4 = renderTexture;
			renderTexture = renderTexture3;
			renderTexture2 = renderTexture4;
			flag = false;
		}
		if (!flag)
		{
			Material spriteAlphaMask5 = RDC.data.spriteAlphaMask;
			spriteAlphaMask5.SetTexture(SpriteAlphaMask.ShaderProperty_RGB, pre);
			spriteAlphaMask5.SetTexture(SpriteAlphaMask.ShaderProperty_A, renderTexture);
			spriteAlphaMask5.SetFloat(SpriteAlphaMask.ShaderProperty_ARot, objRot);
			spriteAlphaMask5.SetFloat(SpriteAlphaMask.ShaderProperty_VisibleOutsideMask, visibleOutsideMask ? 1 : 0);
			Graphics.Blit(pre, rt, spriteAlphaMask5, 0);
		}
		else if (visibleOutsideMask)
		{
			Graphics.Blit(pre, rt);
		}
		else
		{
			rt.Clear();
		}
	}
}
