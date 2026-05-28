using System;
using System.Collections;
using System.IO;
using OggVorbisEncoder;
using UnityEngine;

public static class AudioclipToOggEncoder
{
	private const int RescaleFactor = 32767;

	private const float Padding = 0.3f;

	private const int SixteenBit = 2;

	private const int WriteBufferSize = 512;

	public static IEnumerator EncodeToOgg(AudioClip clip, float startTime, float duration, string outputPath, Action<float> progressCallback)
	{
		int num = Mathf.RoundToInt(startTime * (float)clip.frequency * (float)clip.channels);
		int a = Mathf.RoundToInt((startTime + duration) * (float)clip.frequency * (float)clip.channels);
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		a = Mathf.Min(a, array.Length);
		int num2 = Mathf.FloorToInt(0.3f * (float)clip.frequency * (float)clip.channels);
		float[] array2 = new float[a - num + num2];
		Array.Copy(array, num, array2, 0, a - num);
		byte[] array3 = new byte[array2.Length * 2];
		for (int i = 0; i < array2.Length; i++)
		{
			short value = (short)((!(array2[i] > 1f)) ? ((!(array2[i] < -1f)) ? ((short)(array2[i] * 32767f)) : (-32767)) : short.MaxValue);
			BitConverter.GetBytes(value).CopyTo(array3, i * 2);
		}
		if (RDFile.Exists(outputPath))
		{
			File.Delete(outputPath);
		}
		int num3 = (int)((float)(array3.Length / 2 / clip.channels) / (float)clip.frequency * (float)clip.frequency);
		num3 = num3 / 512 * 512;
		float[][] OutSamples = new float[clip.channels][];
		for (int j = 0; j < clip.channels; j++)
		{
			OutSamples[j] = new float[num3];
		}
		for (int k = 0; k < num3; k++)
		{
			for (int l = 0; l < clip.channels; l++)
			{
				int num4 = k * clip.channels * 2;
				if (l < clip.channels)
				{
					num4 += l * 2;
				}
				float num5 = (float)(short)((array3[num4 + 1] << 8) | array3[num4]) / 32768f;
				OutSamples[l][k] = num5;
			}
		}
		byte[] bytes;
		using (MemoryStream outputData = new MemoryStream())
		{
			VorbisInfo info = VorbisInfo.InitVariableBitRate(clip.channels, clip.frequency, 0.5f);
			int serialNumber = new System.Random().Next();
			OggStream oggStream = new OggStream(serialNumber);
			HeaderPacketBuilder headerPacketBuilder = new HeaderPacketBuilder();
			Comments comments = new Comments();
			comments.AddTag("ARTIST", "TEST");
			OggPacket packet = headerPacketBuilder.BuildInfoPacket(info);
			OggPacket packet2 = headerPacketBuilder.BuildCommentsPacket(comments);
			OggPacket packet3 = headerPacketBuilder.BuildBooksPacket(info);
			oggStream.PacketIn(packet);
			oggStream.PacketIn(packet2);
			oggStream.PacketIn(packet3);
			FlushPages(oggStream, outputData, Force: true);
			ProcessingState processingState = ProcessingState.Create(info);
			int lastPercentage = -1;
			for (int readIndex = 0; readIndex <= OutSamples[0].Length; readIndex += 512)
			{
				float num6 = (float)readIndex * 1f / (float)OutSamples[0].Length * 100f;
				int num7 = Mathf.FloorToInt(num6);
				if (lastPercentage != num7)
				{
					lastPercentage = num7;
					progressCallback(num6);
					yield return null;
				}
				if (readIndex == OutSamples[0].Length)
				{
					processingState.WriteEndOfStream();
				}
				else
				{
					processingState.WriteData(OutSamples, 512, readIndex);
				}
				OggPacket packet4;
				while (!oggStream.Finished && processingState.PacketOut(out packet4))
				{
					oggStream.PacketIn(packet4);
					FlushPages(oggStream, outputData, Force: false);
				}
			}
			FlushPages(oggStream, outputData, Force: true);
			bytes = outputData.ToArray();
		}
		File.WriteAllBytes(outputPath, bytes);
	}

	private static void FlushPages(OggStream oggStream, Stream Output, bool Force)
	{
		OggPage page;
		while (oggStream.PageOut(out page, Force))
		{
			Output.Write(page.Header, 0, page.Header.Length);
			Output.Write(page.Body, 0, page.Body.Length);
		}
	}
}
