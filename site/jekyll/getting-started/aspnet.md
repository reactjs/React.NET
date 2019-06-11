---
id: aspnet
layout: docs
title: Getting Started (ASP.NET 4.x)
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4).

This guide covers enabling server-side rendering and Babel compilation. If you want a step-by-step guide on configuring a brand new site, see [the ReactJS.NET tutorial for ASP.NET](/tutorials/aspnet4.html).

ReactJS.NET requires Visual Studio 2015 and MVC 4 or 5.

Install the `React.Web.Mvc4` package through NuGet. You will also need to install a JS engine to use (either V8 or ChakraCore are recommended). See the [JSEngineSwitcher docs](https://github.com/Taritsyn/JavaScriptEngineSwitcher/wiki/Registration-of-JS-engines) for more information.

To use V8, add the following packages:

```
JavaScriptEngineSwitcher.V8
JavaScriptEngineSwitcher.V8.Native.win-x64
```

`ReactConfig.cs` will be automatically generated for you. Update it to register a JS engine and your JSX files:

```csharp
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(React.Sample.Mvc4.ReactConfig), "Configure")]

namespace React.Sample.Mvc4
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration
				.AddScript("~/Content/Sample.jsx");

			JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddV8();
		}
	}
}
```

Reference JSX files directly in script tags at the end of the page:

```html
<script src="~/Content/Sample.jsx"></script>
@Html.ReactInitJavaScript();
```

You're done! You can now call `Html.React` from within Razor files:

```
@Html.React("Sample", new { initialComments = Model.Comments, page = Model.Page })
```

You can also use [webpack](/guides/webpack.html) or [System.Web.Optimization](/guides/weboptimizer.html) to bundle your scripts together.

Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4) for a working demo.
