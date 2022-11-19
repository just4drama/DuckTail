using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Hsrp
{
	[JsonProperty("hsdp")]
	public Hsdp Hsdp { get; set; }

	[JsonProperty("hblp")]
	public Hblp Hblp { get; set; }
}
