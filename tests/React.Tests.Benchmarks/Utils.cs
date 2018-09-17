using System;

namespace React.Tests.Benchmarks
{
	public static class Utils
	{
		public static void AssertContains(string expected, string actual)
		{
			if (!actual.Contains(expected))
			{
				throw new InvalidOperationException($"Strings were not equal. {expected} {actual}");
			}
		}
	}
}
