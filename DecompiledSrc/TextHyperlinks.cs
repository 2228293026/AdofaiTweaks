using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextHyperlinks : ADOBase, IPointerClickHandler, IEventSystemHandler
{
	public static int FindIntersectingCharacterIndex(Text textComp, Vector3 position, Camera camera)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(textComp.rectTransform, position, camera, out var localPoint);
		return GetCharacterIndexFromPosition(textComp, localPoint);
	}

	private static int GetCharacterIndexFromPosition(Text textComp, Vector2 pos)
	{
		TextGenerator cachedTextGenerator = textComp.cachedTextGenerator;
		if (cachedTextGenerator.lineCount == 0)
		{
			return -1;
		}
		int unclampedCharacterLineFromPosition = GetUnclampedCharacterLineFromPosition(textComp, pos, cachedTextGenerator);
		if (unclampedCharacterLineFromPosition < 0)
		{
			return -1;
		}
		if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
		{
			return cachedTextGenerator.characterCountVisible;
		}
		int startCharIdx = cachedTextGenerator.lines[unclampedCharacterLineFromPosition].startCharIdx;
		int lineEndPosition = GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
		for (int i = startCharIdx; i < lineEndPosition && i < cachedTextGenerator.characterCountVisible; i++)
		{
			UICharInfo uICharInfo = cachedTextGenerator.characters[i];
			Vector2 vector = uICharInfo.cursorPos / textComp.pixelsPerUnit;
			float num = uICharInfo.charWidth / textComp.pixelsPerUnit;
			if (pos.x > vector.x && pos.x < vector.x + num)
			{
				return i;
			}
		}
		return lineEndPosition;
	}

	private static int GetUnclampedCharacterLineFromPosition(Text textComp, Vector2 pos, TextGenerator generator)
	{
		float num = pos.y * textComp.pixelsPerUnit;
		float num2 = 0f;
		for (int i = 0; i < generator.lineCount; i++)
		{
			float topY = generator.lines[i].topY;
			float num3 = topY - (float)generator.lines[i].height;
			if (num > topY)
			{
				float num4 = topY - num2;
				if (num > topY - 0.5f * num4)
				{
					return i - 1;
				}
				return i;
			}
			if (num > num3)
			{
				return i;
			}
			num2 = num3;
		}
		return generator.lineCount;
	}

	private static int GetLineEndPosition(TextGenerator gen, int line)
	{
		line = Mathf.Max(line, 0);
		if (line + 1 < gen.lines.Count)
		{
			return gen.lines[line + 1].startCharIdx - 1;
		}
		return gen.characterCountVisible;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Text component = GetComponent<Text>();
		int num = FindIntersectingCharacterIndex(component, eventData.position, null);
		if (num < 0 || num >= component.text.Length)
		{
			return;
		}
		string text = component.text;
		int num2 = text.LastIndexOf("<a href", num, num);
		if (num2 == -1 || text.IndexOf("</a>", num2, num - num2) != -1)
		{
			return;
		}
		_ = component.text[num];
		int num3 = text.IndexOf("\"", num2, num - num2);
		if (num3 != -1)
		{
			int num4 = num3 + 1;
			int num5 = text.IndexOf("\"", num4, num - num4);
			if (num5 != -1)
			{
				num4 = num3 + 1;
				string url = text.Substring(num4, num5 - num4);
				ADOBase.platformHelper.OpenURL(url);
			}
		}
	}
}
