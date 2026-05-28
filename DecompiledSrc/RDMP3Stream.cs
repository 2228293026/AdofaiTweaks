using System;
using System.Collections.Generic;
using MP3Sharp;
using UnityEngine;

public class RDMP3Stream
{
	private string name;

	private int position;

	private MP3Stream stream;

	private List<float> samples;

	private bool eof;

	private AudioClip clip;

	private int timesBytesReturned;

	public RDMP3Stream(string clipName)
	{
		name = clipName;
		stream = new MP3Stream(name);
	}

	public AudioClip CreateMP3Stream(float length)
	{
		int frequency = stream.Frequency;
		short channelCount = stream.ChannelCount;
		float num = length + 2f + 70f;
		int num2 = Mathf.RoundToInt((float)stream.Frequency * num);
		samples = new List<float>(num2);
		clip = AudioClip.Create(name, num2, channelCount, frequency, stream: true, OnAudioRead, OnAudioSetPosition);
		return clip;
	}

	private void OnAudioRead(float[] data)
	{
		if (eof || samples == null || stream == null)
		{
			return;
		}
		int num = data.Length * 2;
		byte[] array = new byte[num];
		int num2 = num;
		while (samples.Count < position + data.Length)
		{
			num2 = stream.Read(array, 0, num);
			timesBytesReturned++;
			for (int i = 0; i < num2; i += 2)
			{
				byte b = array[i];
				float item = (float)(short)((array[i + 1] << 8) | b) * 1f / 32767f;
				samples.Add(item);
			}
			if (num2 != num)
			{
				eof = true;
				break;
			}
		}
		int num3 = Math.Min(data.Length, samples.Count - position);
		for (int j = 0; j < num3; j++)
		{
			float num4 = samples[position + j];
			data[j] = num4;
		}
		position += num3;
		if (eof)
		{
			Unload();
		}
	}

	~RDMP3Stream()
	{
		if (stream != null)
		{
			Unload();
		}
	}

	private void Unload()
	{
		stream.Flush();
		stream.Close();
		stream.Dispose();
		stream = null;
		samples.Clear();
		samples = null;
	}

	private void OnAudioSetPosition(int newPosition)
	{
		position = newPosition * stream.ChannelCount;
	}
}
