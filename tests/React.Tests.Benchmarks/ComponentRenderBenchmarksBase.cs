using System.Collections.Generic;
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
	public abstract class ComponentRenderBenchmarksBase
	{
		protected JObject _testData = JObject.FromObject(new Dictionary<string, string>() { ["name"] = "Tester" });

		protected void PopulateTestData()
		{
			for (int i = 0; i < 10000; i++)
			{
				_testData.Add("key" + i, "value" + i);
			}
		}

		protected void RegisterCommonServices()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
#if NET461
			AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
#else
			AssemblyRegistration.Container.Register<ICache, MemoryFileCacheCore>();
#endif
			AssemblyRegistration.Container.Register<IFileSystem, PhysicalFileSystem>();
			AssemblyRegistration.Container.Register<IReactEnvironment, ReactEnvironment>().AsMultiInstance();

			JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;
		}
#if NET461

		[Benchmark]
		public void HtmlHelperExtensions_React()
		{
			Utils.AssertContains("Hello Tester!", HtmlHelperExtensions.React(null, "HelloWorld", _testData, serverOnly: true).ToHtmlString());
		}
#endif

		[Benchmark]
		[Arguments(false, false)]
		[Arguments(false, true)]
		[Arguments(true, false)]
		[Arguments(true, true)]
		public void Environment_CreateComponent(bool reuseEngines, bool withPrecompilation)
		{
			var configuration = ReactSiteConfiguration.Configuration;
			configuration
				.SetReuseJavaScriptEngines(reuseEngines)
				.SetAllowJavaScriptPrecompilation(withPrecompilation);
			if (reuseEngines)
			{
				configuration
					.SetStartEngines(2)
					.SetMaxEngines(2)
					.SetMaxUsagesPerEngine(2);
			}

			// Simulate web requests
			const int maxRequestCount = 3;

			for (int requestNumber = 1; requestNumber <= maxRequestCount; requestNumber++)
			{
				var environment = AssemblyRegistration.Container.Resolve<IReactEnvironment>();
				var component = environment.CreateComponent("HelloWorld", _testData, serverOnly: true);
				Utils.AssertContains("Hello Tester!", component.RenderHtml(renderServerOnly: true));
				environment.ReturnEngineToPool();
			}
		}
	}
}
