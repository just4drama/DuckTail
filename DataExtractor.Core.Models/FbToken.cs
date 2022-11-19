using System.Collections.Generic;

namespace DataExtractor.Core.Models;

public class FbToken
{
	public Queue<string> Tokens { get; set; } = new Queue<string>();

}
