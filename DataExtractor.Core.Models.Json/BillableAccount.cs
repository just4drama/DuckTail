using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class BillableAccount
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("aggregated_billing_statement_info")]
	public object AggregatedBillingStatementInfo { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }
}
