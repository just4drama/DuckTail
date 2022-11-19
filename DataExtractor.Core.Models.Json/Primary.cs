using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Primary
{
	[JsonProperty("credential")]
	public Credential Credential { get; set; }

	[JsonProperty("usability")]
	public string Usability { get; set; }

	[JsonProperty("is_editable")]
	public bool IsEditable { get; set; }

	[JsonProperty("is_primary")]
	public bool IsPrimary { get; set; }
}
