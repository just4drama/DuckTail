using System;
using System.Text;

namespace GraphData.Core.Utils;

public class Base64Utils
{
	public static string Decode(string base64EncodedData)
	{
		byte[] bytes = Convert.FromBase64String(base64EncodedData);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string Encode(byte[] data)
	{
		return Convert.ToBase64String(data);
	}

	public static string Encode(string data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		return Convert.ToBase64String(bytes);
	}
}
