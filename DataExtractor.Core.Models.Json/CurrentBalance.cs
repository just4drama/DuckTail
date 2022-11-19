using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class CurrentBalance
{
	[JsonProperty("formatted")]
	public string Formatted { get; set; }
}
