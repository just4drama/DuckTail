using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class QnTDwGI
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("src")]
	public string Src { get; set; }
}
