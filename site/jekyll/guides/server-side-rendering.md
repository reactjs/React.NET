---
layout: docs
title: Server-Side Rendering
---

Server-side rendering allows you to pre-render the initial state of your React
components server-side. This speeds up initial page loads as users do not need
to wait for all the JavaScript to load before seeing the web page.

To use server-side rendering in your application, follow the following steps:

1 - Modify `App_Start\ReactConfig.cs` to reference your components:

```csharp
namespace MyApp
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			// For basic server-side rendering of React components
			// add all necessary JavaScript files here. This includes
			// all dependencies.
			ReactSiteConfiguration.Configuration
				.AddScript("~/Scripts/First.jsx")
				.AddScript("~/Scripts/DependencyOfSecondComponent.jsx")
				.AddScript("~/Scripts/SecondComponent.jsx");
			
			// This setup assumes the use of a build tool (Babel,
			// Webpack, Gulp, Browserify, etc). Performance is improved
			// when you disable ReactJS.NET's version of Babel and load
			// pre-transpiled scripts. Example:
			ReactSiteConfiguration.Configuration = new ReactSiteConfiguration()
				.SetLoadBabel(false)
				.AddScriptWithoutTransform("~/Scripts/HelloWorld.jsx");
		}
	}
}
```

This tells ReactJS.NET to load all the relevant JavaScript files server-side.
The JavaScript files of all the components you want to load and all their
dependencies should be included here.

2 - In your ASP.NET MVC view, call `Html.React` to render a component server-side,
passing it the name of the component, and any required props.

```csharp
@Html.React("HelloWorld", new
{
	name = "Daniel"
})
```

3 - Call `Html.ReactInitJavaScript` at the bottom of the page (just above the
`</body>`) to render initialisation scripts. Note that this does **not** load
the JavaScript files for your components, it only renders the initialisation
code.

```html
<!-- Load all your scripts normally before calling ReactInitJavaScript -->
<!-- Assumes minification/combination is configured as per previous section -->
<script src="https://fb.me/react-0.14.0.min.js"></script>
<script src="https://fb.me/react-dom-0.14.0.min.js"></script>
@Scripts.Render("~/bundles/main")
@Html.ReactInitJavaScript()
```

4 - Hit the page and admire the server-rendered beauty:

```html
<div id="react1">
	<div data-reactid=".2aubxk2hwsu" data-react-checksum="-1025167618">
		<span data-reactid=".2aubxk2hwsu.0">Hello </span>
		<span data-reactid=".2aubxk2hwsu.1">Daniel</span>
	</div>
</div>

<script src="https://fb.me/react-0.14.0.min.js"></script>
<script src="https://fb.me/react-dom-0.14.0.min.js"></script>
<script src="/Scripts/HelloWorld.js"></script>
<script>ReactDOM.render(HelloWorld({"name":"Daniel"}), document.getElementById("react1"));</script>
```

The server-rendered HTML will automatically be reused by React client-side,
meaning your initial render will be super fast.

For a more in-depth example, take a look at the included sample application
(**React.Samples.Mvc4**).

5 - Server-side only rendering

If there is no need to have a React application client side and you just want to use the server side rendering but without the React specific data attributes call `Html.React` and pass serverOnly parameter as true.

```csharp
@Html.React("HelloWorld", new
{
	name = "Daniel"
}, serverOnly: true)
```

And the Html mark up will look like the one following which is a lot cleaner. In this case there is no need to load the React script or call the `Html.ReactInitJavaScript()` method.

```html
<div id="react1">
	<div>
		<span>Hello </span>
		<span>Daniel</span>
	</div>
</div>
```
