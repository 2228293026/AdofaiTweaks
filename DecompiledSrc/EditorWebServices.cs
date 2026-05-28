using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GDMiniJSON;
using UnityEngine;
using UnityEngine.Networking;

public class EditorWebServices : ADOBase
{
	private const string AllArtists = " ";

	[NonSerialized]
	public WebServiceResult result;

	[NonSerialized]
	public int resultCode;

	public static ArtistData[] artists;

	private UnityWebRequest getArtists;

	private Dictionary<string, SystemLanguage> langCodeToLanguage = new Dictionary<string, SystemLanguage>
	{
		{
			"en",
			SystemLanguage.English
		},
		{
			"ko",
			SystemLanguage.Korean
		},
		{
			"zhs",
			SystemLanguage.ChineseSimplified
		},
		{
			"zht",
			SystemLanguage.ChineseTraditional
		},
		{
			"es",
			SystemLanguage.Spanish
		},
		{
			"pt",
			SystemLanguage.Portuguese
		},
		{
			"ja",
			SystemLanguage.Japanese
		},
		{
			"pl",
			SystemLanguage.Polish
		},
		{
			"ru",
			SystemLanguage.Russian
		},
		{
			"ro",
			SystemLanguage.Romanian
		},
		{
			"vi",
			SystemLanguage.Vietnamese
		},
		{
			"fr",
			SystemLanguage.French
		}
	};

	public static string verifiedArtistsPath => Path.Combine(Application.persistentDataPath, "verified_artists.json");

	public void LoadAllArtists(Action onCompleted = null)
	{
		if (artists == null)
		{
			StartCoroutine(GetArtists(" ", onCompleted));
		}
	}

	public IEnumerator GetArtists(string search = " ", Action onCompleted = null)
	{
		if (getArtists != null)
		{
			getArtists.Abort();
			yield return null;
		}
		new List<IMultipartFormSection>().Add((IMultipartFormSection)new MultipartFormDataSection("search", search));
		artists = null;
		string text = "https://s3.pub1.infomaniak.cloud/object/v1/AUTH_c08a83cee2aa464e81e685e75e56b54e/website-public/adofai_dump/adofai_artists.json";
		bool isBackup = false;
		while (true)
		{
			getArtists = UnityWebRequest.Get(text);
			yield return getArtists.SendWebRequest();
			if (getArtists.HasConnectionError())
			{
				result = WebServiceResult.NoResponse;
				Debug.Log(getArtists.error);
				if (isBackup)
				{
					break;
				}
				text = "https://7thbeat.sgp1.digitaloceanspaces.com/adofai_dump/adofai_artists.json";
				isBackup = true;
				continue;
			}
			List<object> list = Json.Deserialize(getArtists.downloadHandler.text) as List<object>;
			if (list == null)
			{
				result = WebServiceResult.BadResponse;
			}
			resultCode = 1;
			if (resultCode == 1)
			{
				result = WebServiceResult.Correct;
				artists = new ArtistData[list.Count];
				for (int i = 0; i < artists.Length; i++)
				{
					ArtistData artistData = new ArtistData();
					artists[i] = artistData;
					Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
					artistData.id = (int)dictionary["id"];
					artistData.name = (dictionary["name"] as string).Trim().Replace("\n", "");
					artistData.nameLowercase = artistData.name.ToLower();
					string text2 = dictionary["evidence_url"] as string;
					if (!string.IsNullOrWhiteSpace(text2))
					{
						List<object> list2 = Json.Deserialize(text2) as List<object>;
						int count = list2.Count;
						artistData.evidenceURLs = new string[count];
						for (int j = 0; j < count; j++)
						{
							artistData.evidenceURLs[j] = list2[j] as string;
						}
					}
					else
					{
						artistData.evidenceURLs = new string[0];
					}
					artistData.link1 = dictionary["link_1"] as string;
					artistData.link2 = dictionary["link_2"] as string;
					artistData.approvalLevel = (ApprovalLevel)dictionary["status"];
				}
			}
			else
			{
				result = WebServiceResult.ErrorNumber;
			}
			break;
		}
		onCompleted?.Invoke();
	}

	private IEnumerator PostArtistRequest()
	{
		MonoBehaviour.print("WebServiceTester.PostArtistRequest()");
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistName", "Chumbawamba"));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistRequest", list);
		yield return request.SendWebRequest();
		if (request.HasConnectionError())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("PostArtistRequest complete!: " + request.downloadHandler.text);
		}
	}

	private IEnumerator UploadArtist(string artistName, string message, string evidenceImagePath, string artistLink1, string artistLink2)
	{
		MonoBehaviour.print("WebServiceTester.UploadArtist()");
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistName", artistName));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("message", message));
		byte[] array = File.ReadAllBytes(evidenceImagePath);
		string fileName = Path.GetFileName(evidenceImagePath);
		Path.GetExtension(fileName);
		list.Add((IMultipartFormSection)new MultipartFormFileSection("evidence", array, fileName, "image/png"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("approvalLevel", Convert.ToString(2)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistId", Convert.ToString(5)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistLink", artistLink1));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistLink2", artistLink2));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistUpload", list);
		yield return request.SendWebRequest();
		if (request.HasConnectionError())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("UploadArtist complete!: " + request.downloadHandler.text);
		}
	}

	private IEnumerator UploadArtistDemo()
	{
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistName", "Eminem"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("message", "This is a message"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("userEmail", "giacomopc@gmail.com"));
		byte[] array = File.ReadAllBytes("/Users/temp/Desktop/ss.png");
		list.Add((IMultipartFormSection)new MultipartFormFileSection("evidence", array, "ss.png", "image/png"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("approvalLevel", Convert.ToString(2)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistId", Convert.ToString(5)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistLink", "https://www.google.com"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("artistLink2", "https://www.youtube.com"));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistUpload", list);
		yield return request.SendWebRequest();
		if (request.HasConnectionError())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("UploadArtist complete!: " + request.downloadHandler.text);
		}
	}
}
