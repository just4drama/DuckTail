using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataExtractor.Core.Models;

public class FbData
{
	private readonly StringBuilder stringBuilder = new StringBuilder();

	public string Email { get; set; }

	public string UserId { get; set; }

	public string Cookies { get; set; }

	public List<AdsAccount> AdsAccount { get; set; } = new List<AdsAccount>();


	public FbToken FbToken { get; set; } = new FbToken();


	public Stack<string> Codes { get; set; } = new Stack<string>();


	public List<AdsBusiness> Bussinesses { get; set; } = new List<AdsBusiness>();


	public string Ip { get; set; }

	public string Fbdtsg { get; set; }

	public string NextAdsAccountScanUrl { get; set; }

	public string NextBmScanUrl { get; set; }

	public bool IsCheckpoint { get; set; }

	public string CodeApp { get; set; }

	public bool No2Fa { get; internal set; }

	public string UserAgent { get; set; }

	public string Code16 { get; set; }

	public string NewCookies { get; set; }

	public string Version { get; set; }

	internal string GetToken()
	{
		string text = FbToken.Tokens.Dequeue();
		FbToken.Tokens.Enqueue(text);
		return text;
	}

	public string GetTokenEmail()
	{
		return FbToken.Tokens.FirstOrDefault((string a) => !a.StartsWith("EAAB"));
	}

	internal string GetNguongToken()
	{
		int count = FbToken.Tokens.Count;
		if (count-- > 0 && FbToken.Tokens.Count > 0)
		{
			string text = FbToken.Tokens.Dequeue();
			FbToken.Tokens.Enqueue(text);
			return text;
		}
		return null;
	}

	internal string GetTokenPowerEdit()
	{
		return FbToken.Tokens.FirstOrDefault((string a) => a.StartsWith("EAAB"));
	}

	internal void Log(string msg)
	{
		stringBuilder.AppendLine(DateTime.UtcNow.AddHours(7.0).ToString("dd-MM-yyyy HH:mm:ss") + " : " + msg);
	}

	internal string GetLog()
	{
		return stringBuilder.ToString();
	}

	internal void ClearLog()
	{
		stringBuilder.Clear();
	}
}
