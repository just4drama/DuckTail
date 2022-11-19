using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class AdMarketAccount
{
	[JsonProperty("security_card_verification_status")]
	public string SecurityCardVerificationStatus { get; set; }

	[JsonProperty("security_card_is_eligible")]
	public bool SecurityCardIsEligible { get; set; }

	[JsonProperty("security_card_eligibility_reasons")]
	public List<object> SecurityCardEligibilityReasons { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }
}
