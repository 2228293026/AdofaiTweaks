using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI;

[AddComponentMenu("UI/Effects/Letter Movement", 15)]
public class LetterMovement : BaseMeshEffect
{
	public float letterShakeRadius;

	public float letterShakeInterval;

	private float letterShakeTime;

	private bool startedShaking;

	public float letterWaveFrequency;

	public float letterWaveHeight;

	public float letterWavePhasePerChar;

	private List<int> charsToShake = new List<int>();

	private List<int> charsToWave = new List<int>();

	private Dictionary<int, Vector3> charOffsets = new Dictionary<int, Vector3>();

	private bool hasShakingText;

	private bool hasWavingText;

	protected LetterMovement()
	{
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		bool flag = false;
		if (hasShakingText)
		{
			if (!startedShaking)
			{
				ShakeAllChars();
				startedShaking = true;
			}
			letterShakeTime += Time.unscaledDeltaTime;
			if (letterShakeTime >= letterShakeInterval)
			{
				ShakeAllChars();
				letterShakeTime = 0f;
			}
			flag = true;
		}
		if (hasWavingText)
		{
			WaveAllChars();
			flag = true;
		}
		if (flag && base.graphic != null)
		{
			base.graphic.SetVerticesDirty();
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}
	}

	public void ModifyVertices(List<UIVertex> verts)
	{
		if (!IsActive())
		{
			return;
		}
		Text component = GetComponent<Text>();
		string text = component.text;
		_ = text.Length;
		if (component == null)
		{
			Debug.LogWarning("LetterMovement: Missing Text component");
			return;
		}
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			int index = num * 6;
			int index2 = num * 6 + 1;
			int index3 = num * 6 + 2;
			int index4 = num * 6 + 3;
			int index5 = num * 6 + 4;
			int num2 = num * 6 + 5;
			if (num2 > verts.Count - 1)
			{
				return;
			}
			UIVertex value = verts[index];
			UIVertex value2 = verts[index2];
			UIVertex value3 = verts[index3];
			UIVertex value4 = verts[index4];
			UIVertex value5 = verts[index5];
			UIVertex value6 = verts[num2];
			Vector3 vector = ((!charOffsets.ContainsKey(i)) ? Vector3.zero : charOffsets[i]);
			value.position += vector;
			value2.position += vector;
			value3.position += vector;
			value4.position += vector;
			value5.position += vector;
			value6.position += vector;
			verts[index] = value;
			verts[index2] = value2;
			verts[index3] = value3;
			verts[index4] = value4;
			verts[index5] = value5;
			verts[num2] = value6;
			num++;
		}
		num++;
	}

	public void AddCharIndexToShake(int charIndex)
	{
		if (!charsToShake.Contains(charIndex))
		{
			charsToShake.Add(charIndex);
			hasShakingText = true;
			ShakeChar(charIndex);
		}
	}

	public void ClearAllShakeChars()
	{
		foreach (int item in charsToShake)
		{
			charOffsets[item] = Vector3.zero;
		}
		charsToShake.Clear();
		hasShakingText = false;
		startedShaking = false;
		letterShakeTime = 0f;
		if (base.graphic != null)
		{
			base.graphic.SetVerticesDirty();
		}
	}

	private void ShakeChar(int charIndex)
	{
		charOffsets[charIndex] = new Vector3(Random.Range(0f - letterShakeRadius, letterShakeRadius), Random.Range(0f - letterShakeRadius, letterShakeRadius), 0f);
	}

	private void ShakeAllChars()
	{
		if (!charsToShake.Any())
		{
			return;
		}
		foreach (int item in charsToShake)
		{
			charOffsets[item] = new Vector3(Random.Range(0f - letterShakeRadius, letterShakeRadius), Random.Range(0f - letterShakeRadius, letterShakeRadius), 0f);
		}
	}

	public void AddCharIndexToWave(int charIndex)
	{
		if (!charsToWave.Contains(charIndex))
		{
			charsToWave.Add(charIndex);
			hasWavingText = true;
			WaveChar(charIndex);
		}
	}

	public void ClearAllWaveChars()
	{
		foreach (int item in charsToWave)
		{
			charOffsets[item] = Vector3.zero;
		}
		charsToWave.Clear();
		hasWavingText = false;
		if (base.graphic != null)
		{
			base.graphic.SetVerticesDirty();
		}
	}

	private void WaveChar(int charIndex)
	{
		float num = Mathf.Sin((Time.unscaledTime + (float)charIndex * letterWavePhasePerChar) * letterWaveFrequency) * letterWaveHeight;
		charOffsets[charIndex] = Vector3.up * num;
	}

	private void WaveAllChars()
	{
		if (!charsToWave.Any())
		{
			return;
		}
		foreach (int item in charsToWave)
		{
			WaveChar(item);
		}
	}
}
