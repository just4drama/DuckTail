using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Data
{
	[JsonProperty("viewer")]
	public Viewer Viewer { get; set; }

	[JsonProperty("billable_account_by_payment_account")]
	public BillableAccountByPaymentAccount BillableAccountByPaymentAccount { get; set; }
}
