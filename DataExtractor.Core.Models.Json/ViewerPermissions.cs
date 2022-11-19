using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class ViewerPermissions
{
	[JsonProperty("billing_write")]
	public bool BillingWrite { get; set; }
}
