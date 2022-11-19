namespace DataExtractor.Core.Models.Browsers.Childs;

internal class ChromeBrowser : ChromiumBrowser
{
	private string GetUserAgent()
	{
		if (base.BrowserData == null)
		{
			return base.UserAgent;
		}
		return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/" + base.BrowserData.Version + " Safari/537.36";
	}

	public ChromeBrowser(string keyPath)
		: base(keyPath)
	{
	}
}
