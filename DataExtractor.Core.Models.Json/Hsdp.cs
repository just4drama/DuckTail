using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Hsdp
{
	[JsonProperty("clpData")]
	public ClpData ClpData { get; set; }

	[JsonProperty("gkxData")]
	public GkxData GkxData { get; set; }

	[JsonProperty("ixData")]
	public IxData IxData { get; set; }

	[JsonProperty("qexData")]
	public QexData QexData { get; set; }

	[JsonProperty("justknobxData")]
	public JustknobxData JustknobxData { get; set; }
}
