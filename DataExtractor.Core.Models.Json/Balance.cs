using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Balance
{
	[JsonProperty("offset_amount")]
	public string OffsetAmount { get; set; }
}
