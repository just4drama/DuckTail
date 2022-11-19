using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class IntlAddress
{
	[JsonProperty("full_address")]
	public string FullAddress { get; set; }
}
