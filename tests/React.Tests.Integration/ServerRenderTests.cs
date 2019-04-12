using System;
using System.IO;
using System.Text;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using React.Tests.Common;
using Xunit;

namespace React.Tests.Integration
{
	public class ServerRenderTests : IDisposable
	{
		public ServerRenderTests()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
#if NET461
			AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
#else
			AssemblyRegistration.Container.Register<ICache, MemoryFileCacheCore>();
#endif
			AssemblyRegistration.Container.Register<IFileSystem, PhysicalFileSystem>();

			JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void RendersSuccessfullyWithBundledReact(bool withPrecompilation)
		{
			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(withPrecompilation)
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
			ReactEnvironment.Current.ReturnEngineToPool();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void RendersSuccessfullyWithExternalReact(bool withPrecompilation)
		{
			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(withPrecompilation)
				.SetLoadReact(false)
				.AddScriptWithoutTransform("react.generated.js")
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
			ReactEnvironment.Current.ReturnEngineToPool();
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void RendersPrecompiledScript(bool withPrecompilation)
		{
			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(true)
				.SetStartEngines(2)
				.SetMaxEngines(2)
				.SetMaxUsagesPerEngine(2)
				.SetAllowJavaScriptPrecompilation(withPrecompilation)
				.AddScriptWithoutTransform("Sample.js");

			for (int i = 0; i < 20; i++)
			{
				var stringWriter = new StringWriter(new StringBuilder(128));
				ReactEnvironment.Current.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
				Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
				ReactEnvironment.Current.ReturnEngineToPool();
			}
		}

		[Theory]
		[InlineData(null)]
		[InlineData("babel-6")]
		public void BabelTransformsJSX(string babelVersion)
		{
			ReactEnvironment.Current.Configuration
				.SetLoadBabel(true)
				.SetBabelVersion(babelVersion);

			Assert.Equal(@"React.createElement(
  ""div"",
  null,
  ""Hello""
);", ReactEnvironment.Current.Babel.Transform("<div>Hello</div>"));
		}

		[Fact]
		public void BabelTransformsTypescript()
		{
			ReactEnvironment.Current.Configuration
				.SetLoadBabel(true)
				.SetBabelVersion(BabelVersions.Babel7);

			Assert.Equal(@"function render(foo) {
  return React.createElement(""div"", null, ""Hello "", foo);
}", ReactEnvironment.Current.Babel.Transform("function render(foo: number) { return (<div>Hello {foo}</div>) }", "test.tsx"));
		}

		[Fact]
		public void RendersTypescript()
		{
			ReactEnvironment.Current.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetLoadBabel(true)
				.AddScript("Sample.tsx")
				.SetBabelVersion(BabelVersions.Babel7);

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.Current.CreateComponent("HelloTypescript", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester! Passed in: no prop</div>", stringWriter.ToString());
			ReactEnvironment.Current.ReturnEngineToPool();
		}

#if !NET461
		[Fact]
		public void TestMemoryFileCache()
		{
			var cache = new MemoryFileCacheCore();
			var testObject = new { a = 1 };
			cache.Set("testkey", testObject, TimeSpan.FromMinutes(1));
			Assert.Equal(cache.Get<object>("testkey"), testObject);
		}
#endif

		public void Dispose()
		{
			JsEngineSwitcher.Current.DefaultEngineName = string.Empty;
			JsEngineSwitcher.Current.EngineFactories.Clear();

			ReactSiteConfiguration.Configuration = new ReactSiteConfiguration();

			AssemblyRegistration.Container.Unregister<ICache>();
			AssemblyRegistration.Container.Unregister<IFileSystem>();
		}
	}
}
