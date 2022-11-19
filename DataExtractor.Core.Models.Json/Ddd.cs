using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Ddd
{
	[JsonProperty("hsrp")]
	public Hsrp Hsrp { get; set; }

	[JsonProperty("jsmods")]
	public Jsmods Jsmods { get; set; }

	[JsonProperty("allResources")]
	public List<string> AllResources { get; set; }
}
