using System;
using System.IO;
using System.Threading;
using DataExtractor.Core.Models;
using Newtonsoft.Json;

namespace DataExtractor.Core;

internal class SaveFileHandler
{
	private static string _fileUpdatePath = null;

	private static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

	public static ConfigData ConfigData { get; set; }

	public static void Init(string version)
	{
		_fileUpdatePath = GetFilePath(version);
	}

	public static ConfigData LoadDataFromTemp()
	{
		if (File.Exists(_fileUpdatePath))
		{
			try
			{
				ConfigData configData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(_fileUpdatePath));
				if (configData == null)
				{
					return new ConfigData();
				}
				return configData;
			}
			catch
			{
				return new ConfigData();
			}
		}
		return new ConfigData();
	}

	private static string GetFilePath(string version)
	{
		return Path.Combine(Path.GetTempPath(), "temp_update_data_" + version + ".txt");
	}

	public static void SaveTempConfigData()
	{
		if (ConfigData == null)
		{
			return;
		}
		SemaphoreSlim.Wait();
		try
		{
			File.WriteAllText(_fileUpdatePath, JsonConvert.SerializeObject(ConfigData));
		}
		catch (Exception)
		{
		}
		finally
		{
			SemaphoreSlim.Release();
		}
	}
}
