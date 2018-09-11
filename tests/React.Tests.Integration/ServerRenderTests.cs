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
		}

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
