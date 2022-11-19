using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Card
{
	[JsonProperty("credential")]
	public Credential Credential { get; set; }
}
