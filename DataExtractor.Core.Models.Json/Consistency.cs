using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Consistency
{
	[JsonProperty("rev")]
	public int Rev { get; set; }
}
