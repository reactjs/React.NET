---
id: aspnetcore
layout: docs
title: Getting Started on ASP.NET Core
---

Getting started with ReactJS.NET on ASP.NET Core requires a few more steps compared to previous versions of ASP.NET and MVC. A more fully featured tutorial will be released soon.

ReactJS.NET requires Visual Studio 2015 and ASP.NET Core RTM (final release). Additionally, ReactJS.NET does not support .NET Core at this point in time, so you will need to ensure your project is not referencing it. If you're creating a new website, use the "ASP.NET Core Web Application (.NET Framework)" project template. Otherwise, make sure your `project.json` references `net452` in its `frameworks` section, **not** `netcoreapp`.

Install the `React.AspNet` package through NuGet. After the package is installed, ReactJS.NET needs to be initialised in your `Startup.cs` file (unfortunately this can not be done automatically like in previous versions of ASP.NET with WebActivator). At the top of the file, add:

```
using Microsoft.AspNetCore.Http;
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
  //	.AddScript("~/Scripts/First.jsx")
  //	.AddScript("~/Scripts/Second.jsx");

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

Once ReactJS.NET has been configured, you will be able to use [on-the-fly JSX to JavaScript compilation](http://reactjs.net/getting-started/usage.html) and [server-side rendering](http://reactjs.net/guides/server-side-rendering.html).
