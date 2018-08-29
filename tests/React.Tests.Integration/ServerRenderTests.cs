using System;
using System.IO;
using System.Text;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Xunit;

namespace React.Tests.Integration
{
	public class ServerRenderTests : IDisposable
	{
		public ServerRenderTests()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			JsEngineSwitcher.Current.EngineFactories.Add(new ChakraCoreJsEngineFactory());
			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;
		}

		[Fact]
		public void RendersSuccessfullyWithBundledReact()
		{
			AssemblyRegistration.Container.Register<ICache, NullCache>();
			AssemblyRegistration.Container.Register<IFileSystem, SimpleFileSystem>();

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(false)
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
		}
#if NET461

		[Fact]
		public void RendersSuccessfullyWithBundledReactAndPrecompilation()
		{
			AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
			AssemblyRegistration.Container.Register<IFileSystem, PhysicalFileSystem>();

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(true)
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
		}
#endif

		[Fact]
		public void RendersSuccessfullyWithExternalReact()
		{
			AssemblyRegistration.Container.Register<ICache, NullCache>();
			AssemblyRegistration.Container.Register<IFileSystem, SimpleFileSystem>();

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(false)
				.SetLoadReact(false)
				.AddScriptWithoutTransform("react.generated.js")
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
		}
#if NET461

		[Fact]
		public void RendersSuccessfullyWithExternalReactAndPrecompilation()
		{
			AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
			AssemblyRegistration.Container.Register<IFileSystem, PhysicalFileSystem>();

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.SetAllowJavaScriptPrecompilation(true)
				.SetLoadReact(false)
				.AddScriptWithoutTransform("react.generated.js")
				.AddScript("Sample.jsx");

			var stringWriter = new StringWriter(new StringBuilder(128));
			ReactEnvironment.GetCurrentOrThrow.CreateComponent("HelloWorld", new { name = "Tester" }, serverOnly: true).RenderHtml(stringWriter, renderServerOnly: true);
			Assert.Equal("<div>Hello Tester!</div>", stringWriter.ToString());
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
