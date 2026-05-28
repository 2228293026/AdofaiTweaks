using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro.Examples;

public class TMP_LetterMovement : MonoBehaviour
{
	private struct VertexAnim
	{
		public float angleRange;

		public float angle;

		public float speed;
	}

	public float ShakeAngleVariance = 3f;

	public float ShakeMagnitude = 0.1f;

	public float ShakeUpdateFPS = 10f;

	public float WaveAmplitude = 1f;

	public float WavePeriod = 6f;

	public float WaveFrequency = 1f;

	private TMP_Text m_TextComponent;

	private bool hasTextChanged;

	private List<int> charsToShake = new List<int>();

	private List<int> charsToWave = new List<int>();

	private Dictionary<int, Vector3> charOffsets = new Dictionary<int, Vector3>();

	private bool hasWavingText;

	private bool hasShakingText;

	private float shakeTimer;

	private Vector3 waveOffset = Vector3.zero;

	private Vector3 jitterOffset = Vector3.zero;

	private Quaternion shakeAngleQuat;

	private void Awake()
	{
		m_TextComponent = GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
	}

	private void OnDisable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
	}

	private void Start()
	{
	}

	private void ON_TEXT_CHANGED(UnityEngine.Object obj)
	{
		if (obj == m_TextComponent)
		{
			hasTextChanged = true;
		}
	}

	public void AddCharIndexToShake(int charIndex)
	{
		if (!charsToShake.Contains(charIndex))
		{
			ShakeChar(charIndex);
			charsToShake.Add(charIndex);
			hasShakingText = true;
		}
	}

	public void ClearAllShakeChars()
	{
		charsToShake.Clear();
		hasShakingText = false;
	}

	public void AddCharIndexToWave(int charIndex)
	{
		if (!charsToWave.Contains(charIndex))
		{
			charsToWave.Add(charIndex);
			hasWavingText = true;
		}
	}

	public void ClearAllWaveChars()
	{
		charsToWave.Clear();
		hasWavingText = false;
	}

	public void ClearTextEffects()
	{
		charOffsets.Clear();
		ClearAllShakeChars();
		ClearAllWaveChars();
	}

	public void ShakeChar(int i)
	{
		charOffsets[i] = ShakeMagnitude * (Vector3.right * UnityEngine.Random.Range(-1f, 1f) + Vector3.up * UnityEngine.Random.Range(-1f, 1f)) + Vector3.forward * UnityEngine.Random.Range(-1f, 1f) * ShakeAngleVariance;
	}

	public void WaveChar(int i)
	{
		charOffsets[i] = Vector3.up * WaveAmplitude * Mathf.Sin((Time.time + (float)i / WavePeriod) * 2f * (float)Math.PI / WaveFrequency);
	}

	private void Update()
	{
		if (hasShakingText && ShakeUpdateFPS > 0f)
		{
			shakeTimer += Time.deltaTime;
			if (shakeTimer > 1f / ShakeUpdateFPS)
			{
				foreach (int item in charsToShake)
				{
					ShakeChar(item);
				}
				shakeTimer -= 1f / ShakeUpdateFPS;
			}
		}
		if (hasWavingText)
		{
			foreach (int item2 in charsToWave)
			{
				WaveChar(item2);
			}
		}
		if (hasWavingText || hasShakingText)
		{
			UpdateChars();
		}
	}

	private void UpdateChars()
	{
		m_TextComponent.ForceMeshUpdate();
		TMP_TextInfo textInfo = m_TextComponent.textInfo;
		hasTextChanged = true;
		VertexAnim[] array = new VertexAnim[1024];
		for (int i = 0; i < 1024; i++)
		{
			array[i].angleRange = UnityEngine.Random.Range(10f, 25f);
			array[i].speed = UnityEngine.Random.Range(1f, 3f);
		}
		TMP_MeshInfo[] array2 = textInfo.CopyMeshInfoVertexData();
		if (hasTextChanged)
		{
			array2 = textInfo.CopyMeshInfoVertexData();
			hasTextChanged = false;
		}
		int characterCount = textInfo.characterCount;
		if (characterCount <= 0)
		{
			return;
		}
		for (int j = 0; j < characterCount; j++)
		{
			if (charOffsets.ContainsKey(j) && textInfo.characterInfo[j].isVisible)
			{
				int materialReferenceIndex = textInfo.characterInfo[j].materialReferenceIndex;
				int vertexIndex = textInfo.characterInfo[j].vertexIndex;
				Vector3[] vertices = array2[materialReferenceIndex].vertices;
				Vector3 vector = (Vector2)((vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f);
				Vector3[] vertices2 = textInfo.meshInfo[materialReferenceIndex].vertices;
				vertices2[vertexIndex] = vertices[vertexIndex] - vector;
				vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - vector;
				vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - vector;
				vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - vector;
				jitterOffset = Vector3.right * charOffsets[j].x + Vector3.up * charOffsets[j].y;
				shakeAngleQuat = Quaternion.Euler(0f, 0f, charOffsets[j].z);
				Matrix4x4 matrix4x = Matrix4x4.TRS(jitterOffset, shakeAngleQuat, Vector3.one);
				vertices2[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex]);
				vertices2[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
				vertices2[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
				vertices2[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
				vertices2[vertexIndex] += vector;
				vertices2[vertexIndex + 1] += vector;
				vertices2[vertexIndex + 2] += vector;
				vertices2[vertexIndex + 3] += vector;
			}
		}
		m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
	}
}
