using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GraphData.Core.Utils;

namespace DataExtractor.Core.Handlers;

public class EncryptHandler
{
	private static string _key = "<RSAKeyValue><Modulus>24WR+D2sq7MyJmlclhJhcfwtFce8h9z2lhRT7arLKI7KbAmjYskWIk15cp46TYqwxrNkSo38uS9IF+NtnajztutKJy7bh8XdJk9DGKtrPn2Dj3hq1F0vejcWTq3GLyIljwT3MXPOrvJpwOesB1cVqifjWUBuyCE2xNKr/8ZHp80=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

	public static byte[] Encrypt(byte[] data, string caption, out string key)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(_key);
		byte[] array = GenerateRandom(16);
		byte[] array2 = GenerateRandom(16);
		byte[] result = Encrypt(data, array, array2);
		string data2 = Base64Utils.Encode(array) + "|" + Base64Utils.Encode(array2) + "|" + Base64Utils.Encode(caption);
		key = EncryptKey(data2);
		return result;
	}

	private static string EncryptKey(string data)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(_key);
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		byte[] inArray = rSACryptoServiceProvider.Encrypt(bytes, fOAEP: false).ToArray();
		return Convert.ToBase64String(inArray);
	}

	private static byte[] GenerateRandom(int length)
	{
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		byte[] array = new byte[length];
		randomNumberGenerator.GetBytes(array);
		return array;
	}

	private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
	{
		using Aes aes = Aes.Create();
		aes.KeySize = 128;
		aes.BlockSize = 128;
		aes.Padding = PaddingMode.Zeros;
		aes.Key = key;
		aes.IV = iv;
		using ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
		return PerformCryptography(data, cryptoTransform);
	}

	private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();
		return memoryStream.ToArray();
	}
}
