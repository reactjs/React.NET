using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json.Linq;
using React.Web.Mvc;

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
				AssemblyRegistration.Container.Register<ICache, NullCache>();
				AssemblyRegistration.Container.Register<IFileSystem, SimpleFileSystem>();
				JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
				JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;

				ReactSiteConfiguration.Configuration
					.SetReuseJavaScriptEngines(false)
					.AddScript("Sample.jsx");
			}

			[Benchmark]
			public void HtmlHelperExtensions_React()
			{
				AssertContains("Hello Tester!", HtmlHelperExtensions.React(null, "HelloWorld", _testData, serverOnly: true).ToHtmlString());
			}

			[Benchmark]
			public void Environment_CreateComponent()
			{
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
