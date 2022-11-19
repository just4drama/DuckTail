using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class CometRainbowNativeSurveyDialogReact
{
	[JsonProperty("r")]
	public List<string> R { get; set; }

	[JsonProperty("rdfds")]
	public Rdfds Rdfds { get; set; }

	[JsonProperty("rds")]
	public Rds Rds { get; set; }

	[JsonProperty("be")]
	public int Be { get; set; }
}
