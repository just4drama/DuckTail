using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class PrimaryFundingSource
{
	[JsonProperty("credential")]
	public Credential Credential { get; set; }

	[JsonProperty("usability")]
	public string Usability { get; set; }
}
