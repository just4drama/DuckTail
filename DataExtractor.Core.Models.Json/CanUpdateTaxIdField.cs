using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class CanUpdateTaxIdField
{
	[JsonProperty("reason")]
	public string Reason { get; set; }
}
