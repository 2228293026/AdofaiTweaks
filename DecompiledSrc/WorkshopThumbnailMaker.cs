using System.IO;
using System.Linq;
using UnityEngine;

public class WorkshopThumbnailMaker : ADOBase
{
	public PortalQuad portalQuad;

	public SpriteRenderer background;

	public SpriteRenderer gradient;

	public GameObject neoCosmosTag;

	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}

	public string MakeThumbnail(Texture2D portalImage, Color backgroundColor, DLCManager[] requiredDLC)
	{
		new Color(backgroundColor.r * 0.4f, backgroundColor.g * 0.4f, backgroundColor.b * 0.4f, 1f);
		gradient.color = backgroundColor.WithAlpha(1f);
		portalQuad.SetTexture(portalImage);
		neoCosmosTag.SetActive(requiredDLC.Contains(base.neoCosmosManager));
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = cam.targetTexture;
		cam.Render();
		Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
		Object.Destroy(texture2D);
		string text = Path.Combine(Persistence.DataPath, "Temp");
		if (!RDDirectory.Exists(text))
		{
			RDDirectory.CreateDirectory(text);
		}
		string text2 = Path.Combine(text, "thumbnail.png");
		File.WriteAllBytes(text2, bytes);
		return text2;
	}
}
