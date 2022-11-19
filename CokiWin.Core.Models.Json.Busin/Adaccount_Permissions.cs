using System;

namespace CokiWin.Core.Models.Json.BusinessJsonModel;

public class Adaccount_Permissions
{
	public string id { get; set; }

	public string[] permitted_tasks { get; set; }

	public string access_status { get; set; }

	public DateTime access_requested_time { get; set; }

	public DateTime access_updated_time { get; set; }
}
