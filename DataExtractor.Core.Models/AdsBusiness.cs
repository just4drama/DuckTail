using System.Collections.Generic;

namespace DataExtractor.Core.Models;

public class AdsBusiness
{
	public string id { get; set; }

	public string name { get; set; }

	public HashSet<string> BmLinks { get; set; } = new HashSet<string>();


	public string verifyStatus { get; set; }

	public List<AdsAccount> clients { get; set; }

	public string Email { get; set; }

	public List<AdsAccount> AdsAccount { get; set; } = new List<AdsAccount>();


	public bool IsVerify => !string.IsNullOrEmpty(verifyStatus) && verifyStatus == "verified";

	public string RequestId { get; set; }

	public bool IsCantShareLink { get; internal set; }

	public int CountBmLink { get; set; } = 2;


	public bool IsAdmin { get; set; }
}
