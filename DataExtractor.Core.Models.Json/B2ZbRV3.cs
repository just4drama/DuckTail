using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class B2ZbRV3
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("src")]
	public string Src { get; set; }
}
