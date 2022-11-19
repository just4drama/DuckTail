using System.Text.RegularExpressions;

namespace DataExtractor.Core.Exts;

public static class StringExts
{
	public static string CutString(this string html, string strCompare, string endStr, int startIndex = 0)
	{
		int num = html.IndexOf(strCompare, startIndex);
		if (num < 0)
		{
			return null;
		}
		num += strCompare.Length;
		int num2 = html.IndexOf(endStr, num);
		return html.Substring(num, num2 - num);
	}

	internal static string StripQuotes(this string s)
	{
		if (s.EndsWith("\"") && s.StartsWith("\""))
		{
			return s.Substring(1, s.Length - 2);
		}
		return s;
	}

	public static string RegGetString(this string input, string regx, RegexOptions op = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace)
	{
		string result = "";
		Match match = Regex.Match(input, regx, op);
		if (match.Success)
		{
			result = match.Groups[1].Value;
		}
		return result;
	}
}
