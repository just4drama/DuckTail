using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class BillingPaymentMethod
{
	[JsonProperty("is_primary")]
	public bool IsPrimary { get; set; }

	[JsonProperty("credential")]
	public Credential Credential { get; set; }

	[JsonProperty("usability")]
	public string Usability { get; set; }

	[JsonProperty("is_editable")]
	public bool IsEditable { get; set; }
}
