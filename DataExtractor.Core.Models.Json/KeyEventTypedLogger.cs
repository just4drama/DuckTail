using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class KeyEventTypedLogger
{
	[JsonProperty("r")]
	public List<string> R { get; set; }

	[JsonProperty("rds")]
	public Rds Rds { get; set; }

	[JsonProperty("be")]
	public int Be { get; set; }
}
