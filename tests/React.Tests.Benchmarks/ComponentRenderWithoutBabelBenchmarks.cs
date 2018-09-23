using BenchmarkDotNet.Attributes;

namespace React.Tests.Benchmarks
{
	[MemoryDiagnoser]
	public class ComponentRenderWithoutBabelBenchmarks : ComponentRenderBenchmarksBase
	{
		[GlobalSetup]
		public void Setup()
		{
			PopulateTestData();
			RegisterCommonServices();

			ReactSiteConfiguration.Configuration
				.SetLoadBabel(false)
				.AddScriptWithoutTransform("Sample.js");
		}
	}
}
