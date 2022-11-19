namespace DataExtractor.Core.Models.Browsers;

public class CookieData
{
	public string Name { get; internal set; }

	public string Value { get; internal set; }

	public string Host { get; internal set; }

	public string Path { get; internal set; }

	public long Expiry { get; internal set; }

	public bool IsSecure { get; internal set; }

	public bool IsHttpOnly { get; internal set; }
}
