using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class WYrn6D4
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("src")]
	public string Src { get; set; }

	[JsonProperty("d")]
	public int D { get; set; }

	[JsonProperty("nc")]
	public int Nc { get; set; }
}
