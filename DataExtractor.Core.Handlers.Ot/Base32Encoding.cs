using System;

namespace DataExtractor.Core.Handlers.Otp;

public static class Base32Encoding
{
	public static byte[] ToBytes(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.TrimEnd('=');
		int num = input.Length * 5 / 8;
		byte[] array = new byte[num];
		byte b = 0;
		byte b2 = 8;
		int num2 = 0;
		int num3 = 0;
		string text = input;
		foreach (char c in text)
		{
			int num4 = CharToValue(c);
			if (b2 > 5)
			{
				num2 = num4 << b2 - 5;
				b = (byte)(b | num2);
				b2 = (byte)(b2 - 5);
			}
			else
			{
				num2 = num4 >> 5 - b2;
				b = (byte)(b | num2);
				array[num3++] = b;
				b = (byte)(num4 << 3 + b2);
				b2 = (byte)(b2 + 3);
			}
		}
		if (num3 != num)
		{
			array[num3] = b;
		}
		return array;
	}

	public static string ToString(byte[] input)
	{
		if (input == null || input.Length == 0)
		{
			throw new ArgumentNullException("input");
		}
		int num = (int)Math.Ceiling((double)input.Length / 5.0) * 8;
		char[] array = new char[num];
		byte b = 0;
		byte b2 = 5;
		int num2 = 0;
		foreach (byte b3 in input)
		{
			b = (byte)(b | (b3 >> 8 - b2));
			array[num2++] = ValueToChar(b);
			if (b2 < 4)
			{
				b = (byte)((uint)(b3 >> 3 - b2) & 0x1Fu);
				array[num2++] = ValueToChar(b);
				b2 = (byte)(b2 + 5);
			}
			b2 = (byte)(b2 - 3);
			b = (byte)((uint)(b3 << (int)b2) & 0x1Fu);
		}
		if (num2 != num)
		{
			array[num2++] = ValueToChar(b);
			while (num2 != num)
			{
				array[num2++] = '=';
			}
		}
		return new string(array);
	}

	private static int CharToValue(char c)
	{
		if (c < '[' && c > '@')
		{
			return c - 65;
		}
		if (c < '8' && c > '1')
		{
			return c - 24;
		}
		if (c < '{' && c > '`')
		{
			return c - 97;
		}
		throw new ArgumentException("Character is not a Base32 character.", "c");
	}

	private static char ValueToChar(byte b)
	{
		if (b < 26)
		{
			return (char)(b + 65);
		}
		if (b < 32)
		{
			return (char)(b + 24);
		}
		throw new ArgumentException("Byte is not a value Base32 value.", "b");
	}
}
