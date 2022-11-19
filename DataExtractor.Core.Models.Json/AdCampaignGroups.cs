using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class AdCampaignGroups
{
	[JsonProperty("nodes")]
	public List<Node> Nodes { get; set; }
}
