using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Coupon
{
	[JsonProperty("credential")]
	public Credential Credential { get; set; }
}
