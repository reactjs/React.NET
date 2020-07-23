# [ReactJS.NET](http://reactjs.net/)

ReactJS.NET is a library that makes it easier to use [Babel](http://babeljs.io/) along with Facebook's [React](https://reactjs.org/) and [JSX](https://reactjs.org/docs/jsx-in-depth.html) from C#.

![.NET Core Desktop](https://github.com/reactjs/React.NET/workflows/.NET%20Core%20Desktop/badge.svg)
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

```
dotnet new -i React.Template
dotnet new reactnet-vanilla
dotnet run
```

#### Planning on using `require` or `import` module syntax in your application? Use the `reactnet-webpack` template instead for webpack support.

See also:

- [Getting Started](https://reactjs.net/getting-started/aspnetcore.html)
- [Tutorial](https://reactjs.net/tutorials/aspnetcore.html)

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
