using System.Collections.Generic;

namespace DataExtractor.Core.Models.Json.DataShareLinkJsonModel;

public class DataShareLinkJsonModel
{
	public List<Datum> data { get; set; }

	public Paging paging { get; set; }
}
