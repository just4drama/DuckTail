using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Node
{
	[JsonProperty("__typename")]
	public string Typename { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }
}
