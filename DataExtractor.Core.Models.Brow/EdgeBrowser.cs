using System;

namespace DataExtractor.Core.Models.Browsers.Childs;

internal class EdgeBrowser : ChromiumBrowser
{
	private string GetUserAgent()
	{
		throw new NotImplementedException();
	}

	public EdgeBrowser(string keyPath)
		: base(keyPath)
	{
	}
}
