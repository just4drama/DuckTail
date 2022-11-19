using DataExtractor.Core.Handlers.Otp;

namespace DataExtractor.Core.Utils;

internal class TotpUtils
{
	public static string GetCodeTotp(string key)
	{
		try
		{
			byte[] secretKey = Base32Encoding.ToBytes(key);
			Totp totp = new Totp(secretKey);
			return totp.ComputeTotp();
		}
		catch
		{
			return null;
		}
	}
}
