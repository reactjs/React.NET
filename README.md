# [ReactJS.NET](http://reactjs.net/)

ReactJS.NET is a library that makes it easier to use [Babel](http://babeljs.io/) along with Facebook's [React](http://facebook.github.io/react/) and [JSX](http://facebook.github.io/react/docs/jsx-in-depth.html) from C#.

[![Build status](https://img.shields.io/appveyor/ci/Daniel15/react-net/master.svg)](https://ci.appveyor.com/project/Daniel15/react-net/branch/master)&nbsp;
[![NuGet version](http://img.shields.io/nuget/v/React.Core.svg)](https://www.nuget.org/packages/React.Core/)
[![Download count](https://img.shields.io/nuget/dt/React.Core.svg)](https://www.nuget.org/packages/React.Core/)

# Features

- On-the-fly [JSX to JavaScript compilation](http://reactjs.net/getting-started/usage.html) via [Babel](http://babeljs.io/)

* JSX to JavaScript compilation via popular minification/combination
  libraries:
  - [ASP.NET Bundling and Minification](http://reactjs.net/bundling/weboptimizer.html)
  - [Cassette](http://reactjs.net/bundling/cassette.html)
  - [Webpack](http://reactjs.net/bundling/webpack.html)
  - [MSBuild](http://reactjs.net/bundling/msbuild.html)
* [Server-side component rendering](http://reactjs.net/features/server-side-rendering.html)
  to make your initial render super-fast, including support for:
  - [CSS-in-JS libraries](https://reactjs.net/features/css-in-js.html)
  - [React Router](https://reactjs.net/features/react-router.html)
  - [React Helmet](https://reactjs.net/features/react-helmet.html)
  - Custom JS logic via implementing [IRenderFunctions](https://github.com/reactjs/React.NET/blob/c93921f059bfe9419ad7094c184979da422a4477/src/React.Core/IRenderFunctions.cs) and passing to [Html.React](https://github.com/reactjs/React.NET/blob/c93921f059bfe9419ad7094c184979da422a4477/src/React.AspNet/HtmlHelperExtensions.cs#L71)
* [Runs on Windows, OS X and Linux](http://reactjs.net/getting-started/chakracore.html) via .NET Core and ChakraCore
* Supports both ASP.NET 4.0/4.5 and ASP.NET Core

# Quick Start

Install the package

```powershell
Install-Package React.Web.Mvc4 # For ASP.NET MVC 4 or 5
Install-Package React.AspNet   # For ASP.NET Core MVC
```

Install a Javascript engine and configure as the default (more info [here](https://reactjs.net/getting-started/aspnet.html) on how this works)

```powershell
Install-Package JavaScriptEngineSwitcher.V8
Install-Package JavaScriptEngineSwitcher.V8.Native.win-x64
```

```csharp
public static class ReactConfig
{
    public static void Configure()
    {
        ReactSiteConfiguration.Configuration
            .AddScript("~/Content/HelloWorld.jsx");

        JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
        JsEngineSwitcher.Current.EngineFactories.AddV8();
    }
}
```

Create JSX files

```javascript
// /Scripts/HelloWorld.jsx
const HelloWorld = props => {
	return <div>Hello {props.greeting}</div>;
};
```

Reference the JSX files from your HTML

```html
<!-- Place this where you want the component div to render -->
@Html.React("HelloWorld", new { Greeting = "friends!" });

<!-- Place these at the end of the page -->
<script src="@Url.Content("~/Scripts/HelloWorld.jsx")"></script>
@Html.ReactInitJavaScript();
```

Now you can use the `HelloWorld` component.

For information on more advanced topics (including precompilation and
server-side rendering), check out [the documentation](http://reactjs.net/docs)

## Building Manually and Contributing

When building your own copy of ReactJS.NET (for example, if implementing a new
feature or fixing a bug), your first build always needs to be done using the
build script (`dev-build.bat`) as this generates a few files required by the
build (such as `SharedAssemblyVersionInfo.cs`). Once this build is completed,
you can open `React.sln` in Visual Studio and compile directly from Visual
Studio. Please refer to the [documentation page on
contributing](http://reactjs.net/dev/contributing.html) for more information on
contributing to ReactJS.NET.

Note that the build requires you to have Git installed. If you do not want to
install Git, you may remove the `GitVersion` task from `build.proj`.
