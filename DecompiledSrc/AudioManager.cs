using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : ADOBase
{
	public struct MusicSource
	{
		public AudioSource audioSource;

		public bool isTrackActive;
	}

	public Dictionary<string, AudioClip> audioLib;

	public Dictionary<string, AsyncOperationHandle<AudioClip>> audioLibHandles;

	public List<RDMP3Stream> rdMP3Streams;

	public PriorityQueue<AudioSource, double> liveSources;

	public Queue<AudioSource> reusableSources;

	public List<MusicSource> musicTracks;

	public AudioSource audioSourcePrefab;

	public float maxMusicVol = 0.4f;

	public static bool pauseGameSounds;

	[Header("Note: This variable is for display only")]
	public int audioClipsLoaded;

	private GameObject audioSourceContainer;

	public AudioMixer Mixer;

	public AudioMixerGroup fallbackMixerGroup;

	private static AudioManager _instance;

	public AudioClipData audioClipData;

	public bool loadingExternalSong { get; private set; }

	public static AudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = GameObject.Find("AudioManager");
				if (gameObject != null)
				{
					_instance = gameObject.GetComponent<AudioManager>();
				}
				else
				{
					_instance = new GameObject("AudioManager").AddComponent<AudioManager>();
				}
			}
			return _instance;
		}
	}

	public void Awake()
	{
		_instance = this;
		if (audioLib == null)
		{
			audioLib = new Dictionary<string, AudioClip>();
		}
		if (audioLibHandles == null)
		{
			audioLibHandles = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
		}
		audioSourceContainer = RDUtils.SpawnIfNotFound("AudioSource Container");
		rdMP3Streams = new List<RDMP3Stream>();
		liveSources = new PriorityQueue<AudioSource, double>();
		reusableSources = new Queue<AudioSource>();
		musicTracks = new List<MusicSource>();
		if (audioSourcePrefab == null)
		{
			audioSourcePrefab = Resources.Load<AudioSource>("Audio Source");
		}
		Mixer = Resources.Load<AudioMixer>("MasterMixer");
		fallbackMixerGroup = Mixer.FindMatchingGroups("Fallback")[0];
	}

	public void Update()
	{
		if (AudioListener.pause)
		{
			return;
		}
		while (liveSources.Count != 0)
		{
			AudioSource audioSource = liveSources.Peek();
			if (audioSource == null)
			{
				liveSources.Dequeue();
				continue;
			}
			if (audioSource.isPlaying)
			{
				break;
			}
			liveSources.Dequeue();
			audioSource.gameObject.SetActive(value: false);
			audioSource.Stop();
			audioSource.clip = null;
			audioSource.loop = false;
			reusableSources.Enqueue(audioSource);
		}
		int num = 0;
		while (num < musicTracks.Count)
		{
			AudioSource audioSource2 = musicTracks[num].audioSource;
			if (audioSource2 == null)
			{
				musicTracks.RemoveAt(num);
			}
			else if (audioSource2.isPlaying)
			{
				UnityEngine.Object.Destroy(audioSource2.gameObject, 3.5f);
				musicTracks.RemoveAt(num);
			}
			else
			{
				CrossFadeAudioSource(audioSource2, musicTracks[num].isTrackActive);
				num++;
			}
		}
	}

	public IEnumerator FindOrLoadAudioClipExternal(string path, bool mp3Streaming, float length = 0f, bool stream = true)
	{
		string fileName = Path.GetFileName(path);
		string text = Path.GetExtension(path).ToLower().Replace(".", "");
		string conductorName = fileName + "*external";
		AudioClip clip = null;
		bool flag = text == "ogg";
		bool flag2 = text == "wav";
		bool flag3 = text == "aif";
		bool flag4 = text == "aiff";
		bool flag5 = text == "mp3";
		if (audioLib.ContainsKey(conductorName))
		{
			yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, audioLib[conductorName]);
			yield break;
		}
		if (!RDFile.Exists(path))
		{
			yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
			yield break;
		}
		if (flag || flag2 || flag3 || flag4)
		{
			AudioType audioType = (flag ? AudioType.OGGVORBIS : (flag2 ? AudioType.WAV : (flag3 ? AudioType.AIFF : (flag4 ? AudioType.AIFF : AudioType.UNKNOWN))));
			string text2 = path.ToFileUri();
			UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(text2, audioType);
			try
			{
				DownloadHandler downloadHandler = webRequest.downloadHandler;
				((DownloadHandlerAudioClip)((downloadHandler is DownloadHandlerAudioClip) ? downloadHandler : null)).streamAudio = stream;
				yield return webRequest.SendWebRequest();
				if ((int)webRequest.result != 1)
				{
					Debug.LogWarning($"webRequest error: {webRequest.error} ({webRequest.result})");
				}
				else
				{
					clip = DownloadHandlerAudioClip.GetContent(webRequest);
				}
				if (clip == null)
				{
					Debug.LogWarning("AudioClip couldn't be found on url: " + path);
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
					yield break;
				}
				if (clip.length == 0f)
				{
					Debug.LogWarning("Audio is invalid for url: " + path + ". Make sure is a valid ogg format.");
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingOggVorbis, null);
					yield break;
				}
			}
			finally
			{
				((IDisposable)webRequest)?.Dispose();
			}
		}
		else
		{
			if (!flag5)
			{
				yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFormatNotSupported, null);
				yield break;
			}
			if (mp3Streaming)
			{
				RDMP3Stream rDMP3Stream = new RDMP3Stream(path);
				clip = rDMP3Stream.CreateMP3Stream(length);
				rdMP3Streams.Add(rDMP3Stream);
			}
			else
			{
				clip = new AudioClipData(path).CreateAudioClip();
			}
		}
		clip.name = conductorName;
		audioLib.Add(conductorName, clip);
		yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, clip);
	}

	public void StopLoadingMP3File()
	{
		if (audioClipData != null)
		{
			audioClipData.StopLoading();
			audioClipData = null;
		}
	}

	public void StopAllSounds()
	{
		AudioSource element;
		double priority;
		while (liveSources.TryDequeue(out element, out priority))
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}
		liveSources = new PriorityQueue<AudioSource, double>();
	}

	public void CrossFadeAudioSource(AudioSource audioSource, bool fadingIn = false)
	{
		if (!audioSource)
		{
			return;
		}
		if (fadingIn)
		{
			if (audioSource.volume < maxMusicVol - 0.1f)
			{
				audioSource.volume = Mathf.Lerp(audioSource.volume, maxMusicVol, 0.75f * Time.deltaTime);
			}
			else
			{
				audioSource.volume = maxMusicVol;
			}
		}
		else if ((double)audioSource.volume > 0.05)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, 1f * Time.deltaTime);
		}
		else
		{
			audioSource.volume = 0f;
		}
	}

	public AudioSource MakeSource(string clipName = "Error", double scheduleTimeInfo = 0.0)
	{
		AudioSource audioSource = default(AudioSource);
		if (reusableSources.TryDequeue(ref audioSource))
		{
			audioSource.gameObject.SetActive(value: true);
		}
		else
		{
			audioSource = UnityEngine.Object.Instantiate(audioSourcePrefab, audioSourceContainer.transform);
		}
		AudioClip clip = FindOrLoadAudioClip(clipName);
		audioSource.clip = clip;
		return audioSource;
	}

	public IEnumerator LoadAddressableAudio(string clipName)
	{
		string path = Path.ChangeExtension(clipName, null).Replace("\\", "/");
		clipName = Path.GetFileName(clipName);
		if (!audioLib.ContainsKey(clipName))
		{
			AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>((object)path);
			yield return handle;
			AudioClip audioClip = null;
			if ((int)handle.Status == 1)
			{
				audioClip = handle.Result;
			}
			else
			{
				Debug.LogError("Failed to load AudioClip in " + path);
			}
			if (!(audioClip == null))
			{
				audioClipsLoaded++;
				audioLib.Add(clipName, audioClip);
				audioLibHandles.Add(clipName, handle);
			}
		}
	}

	public AudioClip FindOrLoadAudioClip(string clipName, string internalLevelName = null, bool fromBundle = false)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		string text;
		if (fromBundle)
		{
			text = Path.ChangeExtension(clipName, null).Replace("\\", "/");
			clipName = Path.GetFileName(clipName);
			flag = true;
		}
		else if (internalLevelName != null)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(clipName);
			string text2 = internalLevelName.Split("-", StringSplitOptions.None)[0];
			text = "InternalLevels/" + text2 + "/" + fileNameWithoutExtension;
			flag = GCNS.worldData[text2].isDLC;
		}
		else
		{
			text = Path.Join(string.op_Implicit("Audio"), string.op_Implicit(clipName));
		}
		if (audioLib.ContainsKey(clipName))
		{
			return audioLib[clipName];
		}
		AudioClip audioClip;
		if (flag)
		{
			AsyncOperationHandle<AudioClip> value = Addressables.LoadAssetAsync<AudioClip>((object)text);
			audioLibHandles.Add(clipName, value);
			audioClip = value.WaitForCompletion();
		}
		else
		{
			audioClip = Resources.Load<AudioClip>(text);
		}
		if (audioClip == null)
		{
			return null;
		}
		audioClipsLoaded++;
		audioLib.Add(clipName, audioClip);
		return audioLib[clipName];
	}

	private AudioSource _Play(string snd, double time, AudioMixerGroup group, float volume = 1f, int priority = 128)
	{
		AudioSource audioSource = MakeSource(snd, time);
		audioSource.pitch = 1f;
		if (group != null)
		{
			audioSource.outputAudioMixerGroup = group;
		}
		else
		{
			audioSource.outputAudioMixerGroup = fallbackMixerGroup;
		}
		audioSource.volume = volume;
		audioSource.priority = priority;
		audioSource.PlayScheduled(time);
		float num = (audioSource.clip ? audioSource.clip.length : float.PositiveInfinity);
		liveSources.Enqueue(audioSource, time + (double)num);
		return audioSource;
	}

	public static AudioSource Play(string snd, double time, AudioMixerGroup group, float volume = 1f, int priority = 128)
	{
		return Instance._Play(snd, time, group, volume, priority);
	}

	public void UnloadAllAddressableAudio()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		foreach (AsyncOperationHandle<AudioClip> value in audioLibHandles.Values)
		{
			Addressables.Release<AudioClip>(value);
		}
		audioLibHandles.Clear();
	}

	public void FlushData()
	{
		UnloadAllAddressableAudio();
		audioLib.Clear();
		rdMP3Streams.Clear();
		AudioSource element;
		double priority;
		while (liveSources.TryDequeue(out element, out priority))
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}
		reusableSources = new Queue<AudioSource>();
		for (int i = 0; i < musicTracks.Count; i++)
		{
			UnityEngine.Object.Destroy(musicTracks[i].audioSource.gameObject);
		}
	}
}
