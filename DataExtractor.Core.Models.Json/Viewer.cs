using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models.Json.GetInfoPendingJsonModel;

public class Viewer
{
	[JsonProperty("work_admin_capabilities")]
	public List<object> WorkAdminCapabilities { get; set; }
}
