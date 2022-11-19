using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Hblp
{
	[JsonProperty("consistency")]
	public Consistency Consistency { get; set; }

	[JsonProperty("rsrcMap")]
	public RsrcMap RsrcMap { get; set; }

	[JsonProperty("compMap")]
	public CompMap CompMap { get; set; }
}
