using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class AdAccount
{
	[JsonProperty("ad_market_account")]
	public AdMarketAccount AdMarketAccount { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("is_eligible_for_flex_billing_onboarding")]
	public bool IsEligibleForFlexBillingOnboarding { get; set; }
}
