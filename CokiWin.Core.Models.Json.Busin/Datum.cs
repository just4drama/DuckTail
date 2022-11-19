namespace CokiWin.Core.Models.Json.BusinessJsonModel;

public class Datum
{
	public string verification_status { get; set; }

	public string name { get; set; }

	public string id { get; set; }

	public Clients clients { get; set; }

	public string[] permitted_roles { get; set; }
}
