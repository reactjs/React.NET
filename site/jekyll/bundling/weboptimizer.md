---
layout: docs
title: Bundling and Minification (ASP.NET 4.x)
---

> **Note:**
>
> This guide applies only to ASP.NET 4.x. Please consider using [webpack](/bundling/webpack.html) if possible.

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4).

ReactJS.NET supports the use of Microsoft's
[ASP.NET Bundling and Minification](http://www.asp.net/mvc/tutorials/mvc-4/bundling-and-minification)
library to transform JavaScript via Babel, and minify it along with all your other
JavaScript. Simply create a `BabelBundle` containing any number of JSX or regular
JavaScript files:

```csharp
// In BundleConfig.cs
bundles.Add(new BabelBundle("~/bundles/main").Include(
	// Add your JSX files here
	"~/Content/HelloWorld.react.jsx",
	"~/Content/AnythingElse.react.jsx",
	// You can include regular JavaScript files in the bundle too
	"~/Content/ajax.js",
));
```

`BabelBundle` will compile your JSX to JavaScript and then minify it. For more
control (eg. if you want to run other transforms as well), you can use
`BabelTransform` directly:

```csharp
// In BundleConfig.cs
bundles.Add(new Bundle("~/bundles/main", new IBundleTransform[]
{
	// This works the same as BabelBundle (transform then minify) but you could
	//add your own transforms as well.
	new BabelTransform(),
	new JsMinify(),
}).Include(
	"~/Content/HelloWorld.react.jsx"
));
```

Note that debug mode should be set to `false` in your `Web.config` file for this to work.

```csharp
// Web.config
  <system.web>
    <compilation debug="false" targetFramework="4.x" />
  </system.web>
```
