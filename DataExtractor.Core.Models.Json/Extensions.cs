using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Extensions
{
	[JsonProperty("is_final")]
	public bool IsFinal { get; set; }

	[JsonProperty("sr_payload")]
	public SrPayload SrPayload { get; set; }
}
