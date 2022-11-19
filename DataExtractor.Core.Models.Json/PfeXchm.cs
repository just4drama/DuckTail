using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class PfeXchm
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("src")]
	public string Src { get; set; }
}
