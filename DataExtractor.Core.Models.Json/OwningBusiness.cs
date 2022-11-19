using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class OwningBusiness
{
	[JsonProperty("viewer_is_admin")]
	public bool ViewerIsAdmin { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }
}
