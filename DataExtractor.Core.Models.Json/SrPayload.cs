using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class SrPayload
{
	[JsonProperty("ddd")]
	public Ddd Ddd { get; set; }
}
