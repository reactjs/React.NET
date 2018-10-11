---
id: download
layout: docs
title: Downloading and Installing
---

## Release Versions

The best way to install ReactJS.NET is via NuGet. There are several NuGet
packages available:

-   [React.Core](https://www.nuget.org/packages/React.Core/) - The core React library. Contains the main functionality of
    React and JSX. You will normally use this through an integration library
    like React.Mvc4.
-   [React.Web.Mvc4](https://www.nuget.org/packages/React.Web.Mvc4/) - Integration with ASP.NET MVC 4 and 5.
-   [React.AspNet](https://www.nuget.org/packages/React.AspNet/) - Integration with ASP.NET Core. [Learn more about ASP.NET 5 support](/getting-started/aspnetcore.html)
-   [Cassette.React](https://www.nuget.org/packages/Cassette.React/) - Integration with [Cassette](http://getcassette.net/). The recommended way to combine and minify your JavaScript.
-   [System.Web.Optimization.React](https://www.nuget.org/packages/System.Web.Optimization.React/) - Integration with
    [ASP.NET Bundling and Minification](http://www.asp.net/mvc/tutorials/mvc-4/bundling-and-minification).

These packages can be installed either via the
[UI in Visual Studio](https://docs.nuget.org/docs/start-here/managing-nuget-packages-using-the-dialog),
or via the Package Manager Console:

```
Install-Package React.Web.Mvc4
```

## Development Builds

Development builds are automatically built after every change. Use these if you
want the very latest bleeding-edge version. These are located on a [custom
package server](http://reactjs.net/packages/) so you need to add this as a
package source in Visual Studio:

1. Click Tools &rarr; NuGet Package Manager &rarr; Package Manager Settings
2. Click Package Sources
3. Click the plus icon, enter name as "ReactJS.NET Dev" and URL as
   http://reactjs.net/packages/
4. When adding the packages to your application, manually select "ReactJS.NET
   Dev" as the package source, and ensure "Include Prerelease" is enabled.

## Building Manually

To build your own copy of ReactJS.NET (for example, if implementing a new
feature or fixing a bug):

1. Compile ReactJS.NET by running `dev-build.bat`
2. Reference React.dll and React.Mvc4.dll (if using MVC 4) in your Web
   Application project

Your first build always needs to be done using the build script (`dev-build.bat`) as
this generates a few files required by the build (such as
`SharedAssemblyVersionInfo.cs`). Once this build is completed, you can open
`React.sln` in Visual Studio and compile directly from Visual Studio.
