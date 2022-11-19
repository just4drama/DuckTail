using System.Collections.Generic;

namespace DataExtractor.Core.Models.Browsers;

public class BrowserCookie
{
	public List<CookieData> AllCookies { get; set; }

	public string FbCookies { get; set; }
}
