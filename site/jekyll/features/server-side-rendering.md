---
layout: docs
title: Server-Side Rendering
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).

Server-side rendering allows you to pre-render the initial state of your React
components server-side. This speeds up initial page loads as users do not need
to wait for all the JavaScript to load before seeing the web page.

To use server-side rendering in your application, follow the following steps:

1 - Modify `App_Start\ReactConfig.cs` (for ASP.NET MVC 4 or 5) or `Startup.cs` (for ASP.NET Core) to reference your components:

```csharp
namespace MyApp
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration = new ReactSiteConfiguration()
				.AddScript("~/Scripts/HelloWorld.jsx");
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
<script crossorigin src="https://cdnjs.cloudflare.com/ajax/libs/react/16.8.0/umd/react.development.js"></script>
<script crossorigin src="https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.0/umd/react-dom.development.js"></script>
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

<script crossorigin src="https://cdnjs.cloudflare.com/ajax/libs/react/16.8.0/umd/react.development.js"></script>
<script crossorigin src="https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.0/umd/react-dom.development.js"></script>
<script src="/Scripts/HelloWorld.js"></script>
<script>ReactDOM.render(HelloWorld({"name":"Daniel"}), document.getElementById("react1"));</script>
```

The server-rendered HTML will automatically be reused by React client-side,
meaning your initial render will be super fast.

If you encounter any errors with the JavaScript, you may want to temporarily disable server-side rendering in order to debug your components in your browser. You can do this by calling `DisableServerSideRendering()` in your ReactJS.NET config.

For a more in-depth example, take a look at the included sample application
(**React.Samples.Mvc4**).

5 - Server-side only rendering

If there is no need to have a React application client side and you just want to use the server side rendering but without the React specific data attributes, call `Html.React` and pass serverOnly parameter as true.

```csharp
@Html.React("HelloWorld", new
{
	name = "Daniel"
}, serverOnly: true)
```

And the HTML will look like the one following which is a lot cleaner. In this case there is no need to load the React script or call the `Html.ReactInitJavaScript()` method.

```html
<div id="react1">
	<div>
		<span>Hello </span>
		<span>Daniel</span>
	</div>
</div>
```
