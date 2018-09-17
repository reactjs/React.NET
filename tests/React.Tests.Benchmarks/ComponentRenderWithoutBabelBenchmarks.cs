using System;
using BenchmarkDotNet.Attributes;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json.Linq;
using React.Tests.Common;
#if NET461
using React.Web.Mvc;
#endif

namespace React.Tests.Benchmarks
{
	[MemoryDiagnoser]
	public class ComponentRenderWithoutBabelBenchmarks
	{
		public static void PopulateTestData()
		{
			for (int i = 0; i < 10000; i++)
			{
				_testData.Add("key" + i, "value" + i);
			}
		}

		public ComponentRenderWithoutBabelBenchmarks()
		{
			PopulateTestData();

			Initializer.Initialize(registration => registration.AsSingleton());
#if NET461
			AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
#else
			AssemblyRegistration.Container.Register<ICache, MemoryFileCacheCore>();
#endif
			AssemblyRegistration.Container.Register<IFileSystem, PhysicalFileSystem>();

			JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(true)
				.SetStartEngines(2)
				.SetMaxEngines(2)
				.SetMaxUsagesPerEngine(2)
				.AddScriptWithoutTransform("Sample.js");
		}

#if NET461

		[Benchmark]
		public void HtmlHelperExtensions_React()
		{
			Utils.AssertContains("Hello Tester!", HtmlHelperExtensions.React(null, "HelloWorld", _testData, serverOnly: true).ToHtmlString());
		}
#endif

		[Benchmark]
		[Arguments(false)]
		[Arguments(true)]
		public void Environment_CreateComponent(bool withPrecompilation)
		{
			ReactSiteConfiguration.Configuration.SetAllowJavaScriptPrecompilation(withPrecompilation);

			var component = ReactEnvironment.Current.CreateComponent("HelloWorld", _testData, serverOnly: true);
			Utils.AssertContains("Hello Tester!", component.RenderHtml(renderServerOnly: true));
			ReactEnvironment.Current.ReturnEngineToPool();
		}

		private static JObject _testData = JObject.FromObject(new System.Collections.Generic.Dictionary<string, string>(){ ["name"] = "Tester" });
	}
}
