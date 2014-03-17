React.NET
=========
React.NET is an experimental library that uses the power of Facebook's
[React](http://facebook.github.io/react/) library to render UI components on 
the server-side with C# as well as on the client. It utilises a JavaScript 
engine to run your component's code server-side. This allows you to reuse
the same logic on the client-side and server-side, and lets you create dynamic
JavaScript applications while keeping search engine optimisation in mind.

It is designed to be cross-platform and work on Linux via Mono as well as 
Microsoft .NET.

Bug reports and feature requests are welcome!

Requirements
============
 * ASP.NET 4.0 or higher
 * ASP.NET MVC 4 (support for other versions will come eventually)
 * [JSON.NET](http://james.newtonking.com/json)
 * A JavaScript engine:
   * [MsieJavaScriptEngine](https://github.com/Taritsyn/MsieJavaScriptEngine) -
     Windows only and requires IE9 or above to be installed on the server
   * [Jint](https://github.com/sebastienros/jint) - Slower but cross-platform

Installation
============
Via released [NuGet package](#)
----------------------------
To be written

Via latest development build
----------------------------
To be written

Manual Installation
-------------------
1. Compile React.NET by running `build.bat`
2. Reference React.dll and React.Mvc4.dll (if using MVC 4) in your Web Application project
3. See usage example below

Usage
=====
Create a React component

```javascript
// HelloWorld.react.js
var HelloWorld = React.createClass({
	render: function () {
		return React.DOM.div(null, 'Hello ', this.props.name);
	}
});
```

Modify `App_Start\ReactConfig.cs` to reference your component

```csharp
public class ReactConfig : IReactSiteInitializer
{
	public void Configure(IReactSiteConfiguration config)
	{
		config.AddScript("~/Scripts/HelloWorld.react.js");
	}
}
```

Call `Html.React` to render a component server-side

```csharp
@Html.React("HelloWorld", new
{
	name = "Daniel"
})
```

Render your scripts normally (through your preferred minifier/combiner or just directly) and call `Html.ReactInitJavaScript` to render initialisation scripts

```csharp
<script src="http://fb.me/react-0.9.0.min.js"></script>
<script src="~/Scripts/HelloWorld.react.js"></script>
@Html.ReactInitJavaScript()
```

Hit the page and admire the server-rendered beauty:

```html
<div id="react1">
	<div data-reactid=".2aubxk2hwsu" data-react-checksum="-1025167618">
		<span data-reactid=".2aubxk2hwsu.0">Hello </span>
		<span data-reactid=".2aubxk2hwsu.1">Daniel</span>
	</div>
</div>

<script src="http://fb.me/react-0.9.0.min.js"></script>
<script src="/Scripts/HelloWorld.react.js"></script>
<script>React.renderComponent(HelloWorld({"name":"Daniel"}), document.getElementById("react1"));</script>
```

The server-rendered HTML will automatically be reused by React client-side, meaning your initial render will be super fast.

Changelog
=========
1.0 - ??? 2014
-------------------
 - Initial release

Licence
=======
BSD License for React.NET

Copyright (c) 2014, Facebook, Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

 * Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
 * Neither the name Facebook nor the names of its contributors may be used to
   endorse or promote products derived from this software without specific
   prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
