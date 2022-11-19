using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Jsmods
{
	[JsonProperty("define")]
	public List<List<object>> Define { get; set; }

	[JsonProperty("require")]
	public List<List<object>> Require { get; set; }
}
