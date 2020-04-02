---
layout: docs
title: React Helmet
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).

React Helmet is a library that allows setting elements inside the `<head>` tag from anywhere in the render tree.

Make sure ReactJS.NET is up to date. You will need at least ReactJS.NET 4.0 (which is in public beta at the time of writing).

Expose React Helmet as `global.Helmet`:

```js
require('expose-loader?Helmet!react-helmet');
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.RenderFunctions

@{
	var helmetFunctions = new ReactHelmetFunctions();
}

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: helmetFunctions)

@{
	ViewBag.HelmetTitle = helmetFunctions.RenderedHelmet.GetValueOrDefault("title");
}
```

In your layout file, render the helmet title that is now in the ViewBag:

```
<!DOCTYPE html>
<html>
<head>
	@Html.Raw(ViewBag.HelmetTitle)
	<meta charset="utf-8" />
	@Html.Raw(ViewBag.ServerStyles)
</head>
<body>
	@RenderBody()
</body>
</html>
```
