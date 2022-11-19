using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class GetInfoPendingJsonModel
{
	[JsonProperty("data")]
	public Data Data { get; set; }

	[JsonProperty("extensions")]
	public Extensions Extensions { get; set; }
}
