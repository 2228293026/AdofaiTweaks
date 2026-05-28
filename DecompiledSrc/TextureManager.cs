using System;
using System.Collections.Generic;
using System.IO;
using ADOFAI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TextureManager
{
	[Flags]
	public enum ImageOptions
	{
		None = 0,
		Smooth = 1
	}

	public class CustomTexture : IDisposable
	{
		private readonly Texture2D baseTexture;

		private readonly Dictionary<ImageOptions, Texture2D> textures;

		public DateTime fileLastModified;

		public bool inUse;

		private readonly bool isInternal;

		private readonly bool isFromBundle;

		private bool baseTextureUsed;

		public CustomTexture(Texture2D baseTexture, DateTime fileLastModified, bool isInternal, bool isFromBundle)
		{
			this.isInternal = isInternal;
			this.baseTexture = baseTexture;
			this.fileLastModified = fileLastModified;
			this.isFromBundle = isFromBundle;
			textures = new Dictionary<ImageOptions, Texture2D>();
			baseTextureUsed = false;
		}

		public Texture2D GetTexture(ImageOptions options)
		{
			if (textures.TryGetValue(options, out var value))
			{
				return value;
			}
			if (!baseTexture)
			{
				return null;
			}
			value = baseTexture;
			if (baseTextureUsed)
			{
				Texture2D texture2D = new Texture2D(value.width, value.height, value.format, mipChain: false);
				texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
				Graphics.CopyTexture(value, texture2D);
				value = texture2D;
			}
			baseTextureUsed = true;
			ApplyOptionsToTexture(value, options);
			textures[options] = value;
			return value;
		}

		public void Dispose()
		{
			if (isFromBundle)
			{
				Addressables.Release<Texture2D>(baseTexture);
			}
			else if (!isInternal)
			{
				UnityEngine.Object.Destroy(baseTexture);
			}
			foreach (Texture2D value in textures.Values)
			{
				if (!isFromBundle || !(value == baseTexture))
				{
					UnityEngine.Object.Destroy(value);
				}
			}
			textures.Clear();
		}
	}

	public class CustomSprite : IDisposable
	{
		private readonly Texture2D baseTexture;

		private readonly Dictionary<ImageOptions, Sprite> sprites;

		private readonly float pixelsPerUnit;

		private readonly bool isInternal;

		private readonly SpriteMeshType spriteType;

		public DateTime fileLastModified;

		public bool inUse;

		private bool baseTextureUsed;

		private readonly bool isFromBundle;

		public CustomSprite(Texture2D texture, DateTime fileLastModified, bool isInternal, bool isFromBundle, float pixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.FullRect)
		{
			baseTexture = texture;
			this.fileLastModified = fileLastModified;
			this.pixelsPerUnit = pixelsPerUnit;
			this.spriteType = spriteType;
			this.isInternal = isInternal;
			this.isFromBundle = isFromBundle;
			baseTextureUsed = false;
			sprites = new Dictionary<ImageOptions, Sprite>();
		}

		public Sprite GetSprite(ImageOptions options)
		{
			if (sprites.TryGetValue(options, out var value))
			{
				return value;
			}
			if (!baseTexture)
			{
				return null;
			}
			Texture2D texture2D = baseTexture;
			if (baseTextureUsed)
			{
				Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, texture2D.format, mipChain: false);
				texture2D2.Apply(updateMipmaps: false, makeNoLongerReadable: true);
				Graphics.CopyTexture(texture2D, texture2D2);
				texture2D = texture2D2;
			}
			ApplyOptionsToTexture(texture2D, options);
			baseTextureUsed = true;
			value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.one / 2f, pixelsPerUnit, 0u, spriteType);
			sprites[options] = value;
			return value;
		}

		public void Dispose()
		{
			if (isFromBundle)
			{
				Addressables.Release<Texture2D>(baseTexture);
			}
			else if (!isInternal)
			{
				UnityEngine.Object.Destroy(baseTexture);
			}
			foreach (Sprite value in sprites.Values)
			{
				UnityEngine.Object.Destroy(value);
			}
			sprites.Clear();
		}
	}

	public Dictionary<string, CustomTexture> customTextures = new Dictionary<string, CustomTexture>();

	public Dictionary<string, CustomSprite> customSprites = new Dictionary<string, CustomSprite>();

	public static Sprite LoadNewSprite(string filePath, out LoadResult status, float pixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.FullRect)
	{
		Texture2D texture2D = LoadTexture(filePath, out status);
		if (texture2D == null)
		{
			return null;
		}
		Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
		return Sprite.Create(texture2D, rect, Vector2.one / 2f, pixelsPerUnit, 0u, spriteType);
	}

	public static Texture2D LoadTexture(string filePath, out LoadResult status, int maxSideSize = -1)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		status = LoadResult.MissingFile;
		Texture2D texture2D;
		if (GCS.internalLevelName != null)
		{
			string text = GCS.internalLevelName.Split('-', StringSplitOptions.None)[0];
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			string text2 = "InternalLevels/" + text + "/" + fileNameWithoutExtension;
			texture2D = ((!ADOBase.isDLCLevel) ? Resources.Load<Texture2D>(text2) : Addressables.LoadAssetAsync<Texture2D>((object)text2).WaitForCompletion());
		}
		else if (ADOBase.isBundleLevel)
		{
			texture2D = Addressables.LoadAssetAsync<Texture2D>((object)filePath.ToFeaturedDLCPath()).WaitForCompletion();
		}
		else
		{
			if (!RDFile.Exists(filePath))
			{
				return null;
			}
			byte[] array = RDFile.ReadAllBytes(filePath, out status);
			TextureFormat textureFormat = TextureFormat.RGBA32;
			texture2D = new Texture2D(2, 2, textureFormat, mipChain: false);
			if (!ImageConversion.LoadImage(texture2D, array))
			{
				return null;
			}
			if (maxSideSize != -1)
			{
				ShrinkImage(texture2D, maxSideSize);
			}
			texture2D.name = filePath;
		}
		if (texture2D.isReadable)
		{
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		}
		texture2D.wrapMode = TextureWrapMode.Repeat;
		return texture2D;
	}

	public static void ShrinkImage(Texture2D tex2D, int maxSideSize)
	{
		if (tex2D == null)
		{
			Debug.LogWarning("texture is null!");
		}
		else if (tex2D.width > maxSideSize || tex2D.height > maxSideSize)
		{
			int num = Mathf.Max(tex2D.width, tex2D.height);
			float num2 = (float)maxSideSize * 1f / (float)num;
			new TextureScale().Bilinear(tex2D, Mathf.RoundToInt((float)tex2D.width * num2), Mathf.RoundToInt((float)tex2D.height * num2));
		}
	}

	public CustomTexture AddTexture(string key, out LoadResult status, string filePath = null, int maxSideSize = -1, bool isInternal = false)
	{
		status = LoadResult.Error;
		if (key.IsNullOrEmpty())
		{
			return null;
		}
		bool flag = customTextures.ContainsKey(key);
		DateTime lastWriteTimeUtc = new FileInfo(filePath).LastWriteTimeUtc;
		if (!flag || DateTime.Compare(customTextures[key].fileLastModified, lastWriteTimeUtc) != 0)
		{
			if (flag)
			{
				UnloadTexture(key);
			}
			if (filePath.IsNullOrEmpty())
			{
				return null;
			}
			Texture2D texture2D = null;
			if (isInternal)
			{
				texture2D = Resources.Load<Texture2D>(key);
			}
			if (!texture2D)
			{
				texture2D = LoadTexture(filePath, out status, maxSideSize);
			}
			customTextures.Add(key, new CustomTexture(texture2D, lastWriteTimeUtc, isInternal, ADOBase.isBundleLevel && !isInternal));
		}
		status = LoadResult.Successful;
		customTextures[key].inUse = true;
		return customTextures[key];
	}

	public CustomSprite AddSprite(string key, string filePath, out LoadResult status)
	{
		status = LoadResult.Error;
		if (key.IsNullOrEmpty())
		{
			return null;
		}
		bool flag = customSprites.ContainsKey(key);
		DateTime lastWriteTimeUtc = new FileInfo(filePath).LastWriteTimeUtc;
		if (!flag || DateTime.Compare(customSprites[key].fileLastModified, lastWriteTimeUtc) != 0)
		{
			if (flag)
			{
				UnloadSprite(key);
			}
			if (filePath.IsNullOrEmpty())
			{
				return null;
			}
			Texture2D texture = LoadTexture(filePath, out status);
			customSprites.Add(key, new CustomSprite(texture, lastWriteTimeUtc, GCS.internalLevelName != null, ADOBase.isBundleLevel));
		}
		status = LoadResult.Successful;
		customSprites[key].inUse = true;
		return customSprites[key];
	}

	private static void ApplyOptionsToTexture(Texture2D texture, ImageOptions options)
	{
		texture.filterMode = (options.HasFlag(ImageOptions.Smooth) ? FilterMode.Bilinear : FilterMode.Point);
	}

	public override string ToString()
	{
		string text = "Textures:";
		foreach (KeyValuePair<string, CustomTexture> customTexture in customTextures)
		{
			text = text + "\n" + customTexture.Key;
		}
		return text;
	}

	public void MarkAllUnused()
	{
		foreach (CustomSprite value in customSprites.Values)
		{
			value.inUse = false;
		}
		foreach (CustomTexture value2 in customTextures.Values)
		{
			value2.inUse = false;
		}
	}

	public void Unload(bool onlyIfUnused)
	{
		UnloadAllTextures(onlyIfUnused);
		UnloadAllSprites(onlyIfUnused);
	}

	public void UnloadAllTextures(bool onlyIfUnused = false)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CustomTexture> customTexture in customTextures)
		{
			if (!onlyIfUnused || !customTexture.Value.inUse)
			{
				list.Add(customTexture.Key);
			}
		}
		foreach (string item in list)
		{
			UnloadTexture(item);
		}
	}

	public void UnloadTexture(string key)
	{
		customTextures[key].Dispose();
		customTextures.Remove(key);
	}

	public void UnloadAllSprites(bool onlyIfUnused = false)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CustomSprite> customSprite in customSprites)
		{
			if (!onlyIfUnused || !customSprite.Value.inUse)
			{
				list.Add(customSprite.Key);
			}
		}
		foreach (string item in list)
		{
			UnloadSprite(item);
		}
	}

	public void UnloadSprite(string key)
	{
		customSprites[key].Dispose();
		customSprites.Remove(key);
	}
}
