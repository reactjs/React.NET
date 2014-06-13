[ReactJS.NET](http://reactjs.net/)
===========

ReactJS.NET is a library that makes it easier to use Facebook's
[React](http://facebook.github.io/react/) and
[JSX](http://facebook.github.io/react/docs/jsx-in-depth.html) from C#.

[![Build status](http://img.shields.io/teamcity/codebetter/bt1242.svg)]((http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1242&guest=1))&nbsp;
![Code coverage](http://img.shields.io/teamcity/coverage/bt1242.svg)&nbsp;
![NuGet downloads](http://img.shields.io/nuget/dt/React.Core.svg)&nbsp;
![NuGet version](http://img.shields.io/nuget/v/React.Core.svg)

Features
========
 * On-the-fly [JSX to JavaScript compilation](http://reactjs.net/getting-started/usage.html)
 * JSX to JavaScript compilation via popular minification/combination
   libraries:
   * [ASP.NET Bundling and Minification](http://reactjs.net/guides/weboptimizer.html)
   * [Cassette](http://reactjs.net/guides/cassette.html)
 * [Server-side component rendering](http://reactjs.net/guides/server-side-rendering.html)
   to make your initial render super-fast (experimental!)
 * [Runs on Linux](http://reactjs.net/guides/mono.html) via Mono and V8

Quick Start
===========
Install the package
```
Install-Package React.Web.Mvc4
```

Create JSX files
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

Reference the JSX files from your HTML
```html
<script src="@Url.Content("~/Scripts/HelloWorld.jsx")"></script>
```

Now you can use the `HelloWorld` component.

For information on more advanced topics (including precompilation and
server-side rendering), check out [the documentation](http://reactjs.net/docs)
