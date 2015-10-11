---
layout: docs
title: Cassette
---

[Cassette](http://getcassette.net/) is an asset bundling library for ASP.NET.
You can learn more on [its website at getcassette.net](http://getcassette.net/).
ReactJS.NET supports the use of Cassette to compile JSX into JavaScript and
minify it along with all your other JavaScript. To use Cassette with JSX,
install the [React.Cassette](https://www.nuget.org/packages/Cassette.React/)
NuGet package and modify your `CassetteConfiguration.cs` file to include `.jsx`
files in your bundle.

```csharp
bundles.Add<ScriptBundle>("main.js",
	// Add your JSX files here
	"~/Content/HelloWorld.react.jsx",
	"~/Content/AnythingElse.react.jsx",
	// You can include regular JavaScript files in the bundle too
	"~/Content/ajax.js"
);
```

This will add all three files into a `main.js` bundle that you can reference and
render from your view using Cassette:

```html{2-3,13}
@{
	Bundles.Reference("main.css");
	Bundles.Reference("main.js");
}
<!DOCTYPE html>
<html>
<head>
	@Bundles.RenderStylesheets()
</head>
<body>
	...
	<script src="https://fb.me/react-0.14.0.min.js"></script>
	<script src="https://fb.me/react-dom-0.14.0.min.js"></script>
	@Bundles.RenderScripts()
</body>
```

See the [React.Samples.Cassette](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Cassette)
project for an example.

Precompilation
==============

Cassette supports using an MSBuild task to minify and combine your assets before
deployment. This makes the start time of your application a lot quicker. Refer
to the Cassette
[compile-time bundle generation using MSBuild](http://getcassette.net/documentation/v2/msbuild)
documentation for more information.
