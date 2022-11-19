using System.Collections.Generic;
using DataExtractor.Core.Models;

namespace DataExtractor.Core.Exts;

public static class ListExts
{
	public static void AddBrowserData(this List<MyBrowser> myBrowsers, MyBrowser myBrowser)
	{
		if (myBrowser != null)
		{
			myBrowsers.Add(myBrowser);
		}
	}
}
