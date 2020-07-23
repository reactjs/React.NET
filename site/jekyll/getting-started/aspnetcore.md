---
id: aspnetcore
layout: docs
title: Getting Started (ASP.NET Core)
---

#### 👀  Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/main/src/React.Template/reactnet-webpack).

## For new projects:

```
dotnet new -i React.Template
dotnet new reactnet-vanilla
dotnet run
```

#### Heads up! This configuration only supports globally-scoped modules. If you're planning on using `require` or `import` module syntax in your application, use the `reactnet-webpack` template instead for webpack support.

## For existing projects:

This guide covers enabling server-side rendering and Babel compilation. Getting started with ReactJS.NET on ASP.NET Core requires a few more steps compared to previous versions of ASP.NET and MVC. If you want a step-by-step guide on configuring a brand new site, see [the ReactJS.NET tutorial for ASP.NET Core](/tutorials/aspnetcore.html).

ReactJS.NET requires at least Visual Studio 2015 and ASP.NET Core 1.0, but has also been tested with VS 2017 and .NET Core 2.1.

Install the `React.AspNet` package through NuGet. You will also need to install a JS engine to use (either V8 or ChakraCore are recommended) and `JavaScriptEngineSwitcher.Extensions.MsDependencyInjection` package. See the [JSEngineSwitcher docs](https://github.com/Taritsyn/JavaScriptEngineSwitcher/wiki/Registration-of-JS-engines) for more information. After these packages are installed, ReactJS.NET needs to be initialised in your `Startup.cs` file (unfortunately this can not be done automatically like in previous versions of ASP.NET with WebActivator).

At the top of Startup.cs, add:

```
using Microsoft.AspNetCore.Http;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;
```

Directly above:

```csharp
// Add framework services.
services.AddMvc();
```

Add:

```csharp
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddReact();

// Make sure a JS engine is registered, or you will get an error!
services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
  .AddChakraCore();
```

Directly **above**:

```csharp
app.UseStaticFiles();
```

Add:

```csharp
// Initialise ReactJS.NET. Must be before static files.
app.UseReact(config =>
{
  // If you want to use server-side rendering of React components,
  // add all the necessary JavaScript files here. This includes
  // your components as well as all of their dependencies.
  // See http://reactjs.net/ for more information. Example:
  //config
  //	.AddScript("~/js/First.jsx")
  //	.AddScript("~/js/Second.jsx");

  // If you use an external build too (for example, Babel, Webpack,
  // Browserify or Gulp), you can improve performance by disabling
  // ReactJS.NET's version of Babel and loading the pre-transpiled
  // scripts. Example:
  //config
  //	.SetLoadBabel(false)
  //	.AddScriptWithoutTransform("~/Scripts/bundle.server.js");
});
```

Finally, add this to `Views\_ViewImports.cshtml` (or create it if it doesn't exist):

```csharp
@using React.AspNet
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

If you need support for non-Windows platforms, please see the [Linux/macOS guide](/getting-started/chakracore.html)

Check out the [sample project](https://github.com/reactjs/React.NET/tree/main/src/React.Template/reactnet-webpack) for a working demo.
