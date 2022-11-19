using System;
using System.Security.Cryptography;

namespace DataExtractor.Core.Handlers.Otp;

public class Totp
{
	private const long unixEpochTicks = 621355968000000000L;

	private const long ticksToSeconds = 10000000L;

	private const int step = 30;

	private const int totpSize = 6;

	private byte[] key;

	public Totp(byte[] secretKey)
	{
		key = secretKey;
	}

	public string ComputeTotp()
	{
		long input = CalculateTimeStepFromTimestamp(DateTime.UtcNow);
		byte[] bigEndianBytes = GetBigEndianBytes(input);
		HMACSHA1 hMACSHA = new HMACSHA1();
		hMACSHA.Key = key;
		byte[] array = hMACSHA.ComputeHash(bigEndianBytes);
		int num = array[^1] & 0xF;
		int num2 = ((array[num] & 0x7F) << 24) | ((array[num + 1] & 0xFF) << 16) | ((array[num + 2] & 0xFF) << 8) | ((array[num + 3] & 0xFF) % 1000000);
		return Digits(num2, 6);
	}

	public int RemainingSeconds()
	{
		return 30 - (int)((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000 % 30);
	}

	private byte[] GetBigEndianBytes(long input)
	{
		byte[] bytes = BitConverter.GetBytes(input);
		Array.Reverse(bytes);
		return bytes;
	}

	private long CalculateTimeStepFromTimestamp(DateTime timestamp)
	{
		long num = (timestamp.Ticks - 621355968000000000L) / 10000000;
		return num / 30;
	}

	private string Digits(long input, int digitCount)
	{
		return ((int)input % (int)Math.Pow(10.0, digitCount)).ToString().PadLeft(digitCount, '0');
	}
}
