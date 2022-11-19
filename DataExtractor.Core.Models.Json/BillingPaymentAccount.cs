using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class BillingPaymentAccount
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("primary")]
	public List<Primary> Primary { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("primary_funding_source")]
	public List<PrimaryFundingSource> PrimaryFundingSource { get; set; }

	[JsonProperty("__isPaymentAccount")]
	public string IsPaymentAccount { get; set; }

	[JsonProperty("billable_account")]
	public BillableAccount BillableAccount { get; set; }

	[JsonProperty("ad_account")]
	public AdAccount AdAccount { get; set; }

	[JsonProperty("payment_legacy_account_id")]
	public string PaymentLegacyAccountId { get; set; }

	[JsonProperty("coupons")]
	public List<Coupon> Coupons { get; set; }

	[JsonProperty("billing_payment_methods")]
	public List<BillingPaymentMethod> BillingPaymentMethods { get; set; }

	[JsonProperty("cards")]
	public List<Card> Cards { get; set; }
}
