using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

public class EntitlementsService : MonoBehaviour
{
	public string playerIdentifier;

	public List<string> entitlements;

	private void Start()
	{
		GetPlayerIdentifier();
		GetEntitlements();
	}

	private void DeserializeEntitlements(string json)
	{
		string[] source = null;
		try
		{
			source = JsonConvert.DeserializeObject<string[]>(json);
		}
		catch (Exception arg)
		{
			Debug.Log($"There was an error deserializing {json}.\nException: {arg}");
		}
		entitlements = source.ToList();
	}

	public void GetPlayerIdentifier()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (SteamIntegration.initialized)
		{
			CSteamID steamID = SteamUser.GetSteamID();
			playerIdentifier = $"steam:{steamID.m_SteamID}";
		}
	}

	public void GetEntitlements()
	{
		StartCoroutine(GetEntitlementsRequest());
	}

	private IEnumerator GetEntitlementsRequest()
	{
		UnityWebRequest request = new UnityWebRequest("https://7thbe.at/api/entitlements/check?player_identifier=" + playerIdentifier, "GET")
		{
			downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(),
			redirectLimit = 0
		};
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();
		if ((int)request.result != 2 && request.responseCode != 204 && request.responseCode != 200)
		{
			string.Format("{0} {1}", "error.entitlement.response.unknown", request.responseCode);
		}
		if (request.responseCode == 200)
		{
			DeserializeEntitlements(request.downloadHandler.text);
		}
	}

	public void RedeemCode(string code, Action<bool, string> onCompleteAction)
	{
		code = code.Trim().ToUpper();
		if (!new Regex("^[A-Z0-9]{8}$").IsMatch(code))
		{
			onCompleteAction(arg1: false, "error.entitlement.code");
		}
		else
		{
			StartCoroutine(RedeemCodeRequest(code, onCompleteAction));
		}
	}

	private IEnumerator RedeemCodeRequest(string code, Action<bool, string> onCompleteAction)
	{
		string s = "{\"code\":\"" + code + "\", \"player_identifier\": \"" + playerIdentifier + "\"}";
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		UnityWebRequest request = new UnityWebRequest("https://7thbe.at/api/entitlements/redeem", "POST", (DownloadHandler)new DownloadHandlerBuffer(), (UploadHandler)new UploadHandlerRaw(bytes)
		{
			contentType = "application/json"
		})
		{
			redirectLimit = 0
		};
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();
		string arg = "levelSelect.entitlement.complete";
		if ((int)request.result == 2)
		{
			arg = "error.connection";
		}
		else if (request.responseCode == 403)
		{
			arg = "error.entitlement.response.403";
		}
		else if (request.responseCode == 404)
		{
			arg = "error.entitlement.response.404";
		}
		else if (request.responseCode == 409)
		{
			arg = "error.entitlement.response.409";
		}
		else if (request.responseCode == 410)
		{
			arg = "error.entitlement.response.410";
		}
		else if (request.responseCode != 200)
		{
			arg = "error.entitlement.response.unknown";
		}
		if (request.responseCode == 200)
		{
			DeserializeEntitlements(request.downloadHandler.text);
		}
		onCompleteAction(request.responseCode == 200, arg);
	}
}
