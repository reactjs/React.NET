---
layout: docs
title: Getting Started
---

Installation
------------
The best way to install ReactJS.NET is via NuGet. There are several NuGet
packages available:

 * [React](#) - The core React library. Contains the main functionality of React
   and JSX.  You will normally use this through an integration library like
   React.Mvc4.
 * [React.Mvc4](#) - Integration with ASP.NET MVC 4 and 5
 * [React.Mvc3](#) - Integration with ASP.NET MVC 3
 * [System.Web.Optimization.React](#) - Integration with
   [ASP.NET Bundling and Minification](http://www.asp.net/mvc/tutorials/mvc-4/bundling-and-minification).
   Use this to combine and minify your JavaScript.
 * [Cassette.React](#) - Integration with [Cassette](http://getcassette.net/)

These packages can be installed either via the
[UI in Visual Studio](https://docs.nuget.org/docs/start-here/managing-nuget-packages-using-the-dialog),
or via the Package Manager Console:

```
Install-Package React.Mvc4
```

Usage
-----
Once installed, create your React components as usual, ensuring you add the
`/** @jsx React.DOM */` docblock.

```javascript
// /Scripts/HelloWorld.jsx
/** @jsx React.DOM */
var HelloWorld = React.createClass({
	render: function () {
		return (
			<div>Hello {this.props.name}</div>
		);
	}
});
```

On-the-Fly JSX to JavaScript Compilation
----------------------------------------
Hit a JSX file in your browser (eg. `/Scripts/HelloWorld.jsx`) and observe
the magnificence of JSX being compiled into JavaScript with no precompilation
necessary.

Next Steps
-----------
On-the-fly JSX compilation is good for fast iteration during development, but
for production you will want to precompile for best performance. This can be
done via [ASP.NET Bundling and Minification](/guides/weboptimizer.html) or
[Cassette](/guides/cassette.html).
