using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataExtractor.Core.Models;

public class ConfigData
{
	public List<FbData> CacheFbData { get; set; } = new List<FbData>();


	[JsonIgnore]
	public MyIp Ip { get; set; }

	public string GuidId { get; set; }

	[JsonIgnore]
	public List<BrowserProfile> BrowserProfiles { get; set; } = new List<BrowserProfile>();

}
