using UnityEngine;

public class GlitchText : MonoBehaviour
{
	public TextMesh[] textMeshes;

	public string characters = "";

	public float shortInterval;

	public float longInterval;

	private float nextVariation;

	private bool longVariation = true;

	private void Update()
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		if (timeSinceLevelLoad > nextVariation)
		{
			float average = (longVariation ? shortInterval : longInterval);
			nextVariation = timeSinceLevelLoad + RandomWithVariation(average, 0.2f);
			ChangeGlitch();
			longVariation = !longVariation;
		}
		static float RandomWithVariation(float num, float variation)
		{
			return Random.Range(num - num * variation, num + num * variation);
		}
	}

	private void ChangeGlitch()
	{
		string text = " " + RandomChars(3) + " \n" + RandomChars(5) + "\n" + " " + RandomChars(3) + " ";
		TextMesh[] array = textMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = text;
		}
		char RandomCharacter()
		{
			int index = Random.Range(0, characters.Length);
			return characters[index];
		}
		string RandomChars(int charCount)
		{
			string text2 = "";
			for (int j = 0; j < charCount; j++)
			{
				text2 += RandomCharacter();
			}
			return text2;
		}
	}
}
