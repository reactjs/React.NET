using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json.Linq;
#if NET461
using React.Web.Mvc;
#endif

namespace React.Tests.Benchmarks
{
	public static class Program
    {
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<ComponentRenderBenchmarks>();
		}

		[MemoryDiagnoser]
		public class ComponentRenderBenchmarks
		{
			public static void PopulateTestData()
			{
				for (int i = 0; i < 10000; i++)
				{
					_testData.Add("key" + i, "value" + i);
				}
			}

			public ComponentRenderBenchmarks()
			{
				PopulateTestData();

				Initializer.Initialize(registration => registration.AsSingleton());
#if NET461
				AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
#else
				AssemblyRegistration.Container.Register<ICache, MemoryFileCacheCore>();
#endif
				AssemblyRegistration.Container.Register<IFileSystem, SimpleFileSystem>();
				JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
				JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;

				ReactSiteConfiguration.Configuration
					.SetReuseJavaScriptEngines(false)
					.AddScript("Sample.jsx");
			}

#if NET461

			[Benchmark]
			public void HtmlHelperExtensions_React()
			{
				AssertContains("Hello Tester!", HtmlHelperExtensions.React(null, "HelloWorld", _testData, serverOnly: true).ToHtmlString());
			}
#endif

			[Benchmark]
			[Arguments(false)]
			[Arguments(true)]
			public void Environment_CreateComponent(bool withPrecompilation)
			{
				ReactSiteConfiguration.Configuration.SetAllowJavaScriptPrecompilation(withPrecompilation);

				var component = ReactEnvironment.Current.CreateComponent("HelloWorld", _testData, serverOnly: true);
				AssertContains("Hello Tester!", component.RenderHtml(renderServerOnly: true));
				ReactEnvironment.Current.ReturnEngineToPool();
			}

			private static JObject _testData = JObject.FromObject(new System.Collections.Generic.Dictionary<string, string>(){ ["name"] = "Tester" });
		}

		private static void AssertContains(string expected, string actual)
		{
			if (!actual.Contains(expected))
			{
				throw new InvalidOperationException($"Strings were not equal. {expected} {actual}");
			}
		}
    }
}
