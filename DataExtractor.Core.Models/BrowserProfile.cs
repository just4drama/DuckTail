using System.Collections.Generic;
using DataExtractor.Core.Models.Browsers;

namespace DataExtractor.Core.Models;

public class BrowserProfile
{
	public MyBrowser MyBrowser { get; private set; }

	public string CookiePath { get; set; }

	public List<CookieData> Cookies { get; } = new List<CookieData>();


	public string FbCookies { get; set; }

	public FbData FbData { get; set; }

	public string ProfilePath { get; set; }

	public string ExtensionData { get; internal set; }

	public BrowserProfile(MyBrowser myBrowser)
	{
		MyBrowser = myBrowser;
	}
}
