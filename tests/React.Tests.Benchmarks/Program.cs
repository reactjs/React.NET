using System;
using BenchmarkDotNet.Running;

namespace React.Tests.Benchmarks
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<ComponentRenderWithBabelBenchmarks>();
			BenchmarkRunner.Run<ComponentRenderWithoutBabelBenchmarks>();
		}
	}
}
