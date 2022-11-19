namespace DataExtractor.Core.Models;

public class AdsAccount
{
	public string id { get; set; }

	public string name { get; set; }

	public string account_id => id?.Replace("act_", "").Trim();

	public string Nguong { get; set; }

	public AdsBusiness business { get; set; }

	public string Nguong2 { get; set; }
}
