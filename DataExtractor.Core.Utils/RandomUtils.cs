using System;
using System.Linq;
using System.Threading;

namespace DataExtractor.Core.Utils;

public class RandomUtils
{
	private static int seed = Environment.TickCount;

	private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

	public static int RandomNumber(int from, int to)
	{
		if (from == to)
		{
			return from;
		}
		return random.Value.Next(from, to + 1);
	}

	public static int RandomNumber(int next)
	{
		return random.Value.Next(next);
	}

	public static string RandomString(int length = 10)
	{
		return new string((from s in Enumerable.Repeat("abcdefghijklmnopqrstuvwxyABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[RandomNumber(s.Length)]).ToArray());
	}
}
