---
layout: docs
title: OS X/Linux
---

ReactJS.NET supports running on non-Windows platforms via both Mono and .NET Core. This guide focuses on OS X / Linux support via the ChakraCore engine and .NET Core, which uses precompiled binaries. To use the full .NET framework with Mono, please see the [Mono guide](/guides/mono.html).

Add `React.AspNet` as a dependency to your .NET Core project. Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc) or the [documentation](https://reactjs.net/getting-started/aspnetcore.html) if you need more details on that.

Next, install the `JavascriptEngineSwitcher.ChakraCore` and `JavaScriptEngineSwitcher.Extensions.MsDependencyInjection` NuGet packages. Depending on the platform(s) you want to support, also install one or more of these NuGet packages:

-   Windows: `JavaScriptEngineSwitcher.ChakraCore.Native.win-x64`. The VC++ 2017 runtime is also required.
-   OS X: ``JavaScriptEngineSwitcher.ChakraCore.Native.osx-x64`
-   Linux x64: ``JavaScriptEngineSwitcher.ChakraCore.Native.linux-x64`

In `Startup.cs`, set ChakraCore as the default Javascript engine.

```csharp
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
	services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
		.AddChakraCore();

	// existing services below:
	services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
	services.AddReact();
	services.AddMvc();
}
```

You're done! Server-side rendering and JSX compilation should now be working properly.

For more information about registering Javascript engines, check out the [JavascriptEngineSwitcher documentation](https://github.com/Taritsyn/JavaScriptEngineSwitcher/wiki/Registration-of-JS-engines).

### Mono

While Mono is supported, we strongly recommend using .NET Core instead.

ReactJS.NET includes full support for Mono via Google's [V8 JavaScript engine](https://code.google.com/p/v8/), the same engine used by Google Chrome and Node.js. To use ReactJS.NET with Mono, follow the documentation on the [JavaScriptEngineSwitcher repo](https://github.com/Taritsyn/JavaScriptEngineSwitcher/wiki/JS-Engine-Switcher:-Vroom) to build Vroom, and then register the JS Engine as the default in `Startup.cs`.

If VroomJs fails to load, you will see an exception when your application is started. If this happens, run Mono with the `MONO_LOG_LEVEL=debug` environment variable to get more useful debugging information. Often, this occurs when Mono is unable to locate V8 (ie. it's not in /usr/lib/ or /usr/local/lib/)
