using BenchmarkDotNet.Attributes;

namespace React.Tests.Benchmarks
{
	[MemoryDiagnoser]
	public class ComponentRenderWithBabelBenchmarks : ComponentRenderBenchmarksBase
	{
		[GlobalSetup]
		public void Setup()
		{
			PopulateTestData();
			RegisterCommonServices();

			ReactSiteConfiguration.Configuration
				.AddScript("Sample.jsx");
		}
	}
}
