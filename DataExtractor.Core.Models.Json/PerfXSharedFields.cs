using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class PerfXSharedFields
{
	[JsonProperty("r")]
	public List<string> R { get; set; }

	[JsonProperty("be")]
	public int Be { get; set; }
}
