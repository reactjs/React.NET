[ReactJS.NET](http://reactjs.net/)
===========

ReactJS.NET is a library that makes it easier to use [Babel](http://babeljs.io/) along with Facebook's [React](http://facebook.github.io/react/) and [JSX](http://facebook.github.io/react/docs/jsx-in-depth.html) from C#.

[![Build status](https://img.shields.io/appveyor/ci/Daniel15/react-net/master.svg)](https://ci.appveyor.com/project/Daniel15/react-net/branch/master)&nbsp;
[![NuGet version](http://img.shields.io/nuget/v/React.Core.svg)](https://www.nuget.org/packages/React.Core/)

Features
========
 * On-the-fly [JSX to JavaScript compilation](http://reactjs.net/getting-started/usage.html) via [Babel](http://babeljs.io/)
 * JSX to JavaScript compilation via popular minification/combination
   libraries:
   * [ASP.NET Bundling and Minification](http://reactjs.net/guides/weboptimizer.html)
   * [Cassette](http://reactjs.net/guides/cassette.html)
   * [Webpack](http://reactjs.net/guides/webpack.html)
   * [MSBuild](http://reactjs.net/guides/msbuild.html)
 * [Server-side component rendering](http://reactjs.net/guides/server-side-rendering.html)
   to make your initial render super-fast (experimental!)
 * [Runs on Linux](http://reactjs.net/guides/mono.html) via Mono and V8
 * Supports both ASP.NET 4.0/4.5 and ASP.NET 5

Quick Start
===========
Install the package
```
Install-Package React.Web.Mvc4 # For ASP.NET MVC 4 or 5
Install-Package React.AspNet   # For ASP.NET MVC 6 (ASP.NET 5)
```

Create JSX files
```javascript
// /Scripts/HelloWorld.jsx
var HelloWorld = React.createClass({
    render: function () {
        return (
            <div>Hello {this.props.name}</div>
        );
    }
});
```

Reference the JSX files from your HTML
```html
<script src="@Url.Content("~/Scripts/HelloWorld.jsx")"></script>
```

Now you can use the `HelloWorld` component.

For information on more advanced topics (including precompilation and
server-side rendering), check out [the documentation](http://reactjs.net/docs)

Building Manually and Contributing
----------------------------------

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
