namespace DataExtractor.Core.Models;

public class MyIp
{
	public string Ip { get; set; }

	public string Country { get; set; }

	public string Cc { get; set; }

	public override string ToString()
	{
		return Ip + " - " + Country + "(" + Cc + ")";
	}
}
