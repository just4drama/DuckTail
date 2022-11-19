using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class SpendingInfo
{
	[JsonProperty("spend_limit_currency_amount")]
	public object SpendLimitCurrencyAmount { get; set; }

	[JsonProperty("amount_spent_currency_amount")]
	public AmountSpentCurrencyAmount AmountSpentCurrencyAmount { get; set; }
}
