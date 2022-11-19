using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class OriginalBalance
{
	[JsonProperty("formatted")]
	public string Formatted { get; set; }
}
