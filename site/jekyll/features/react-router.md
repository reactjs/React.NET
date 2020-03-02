---
layout: docs
title: React Router
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Template/reactnet-webpack).

[React Router](https://github.com/ReactTraining/react-router) is a Javascript routing library. By using the `React.Router` package, you can add server-side route resolution by deferring to React Router.

Add the `React.Router` package to your solution:

```
dotnet add package React.Router
```

Use a wildcard route in ASP.NET's route declarations:

```csharp
app.UseMvc(routes =>
{
	routes.MapRoute("default", "{path?}", new { controller = "Home", action = "Index" });
});
```

Change `@Html.React` to `Html.ReactRouter` in the razor view:

```csharp
using React.Router;

@Html.ReactRouter("RootComponent", props);
```

Expose the routes in the root component in your app:

```
import {
	Link,
	BrowserRouter,
	Route,
	Switch,
	StaticRouter,
	Redirect,
} from 'react-router-dom';

render() {
	const app = (
		<div>
			<Navbar />
			<Switch>
				<Route
					exact
					path="/"
					render={() => <Redirect to="/home" />}
				/>
				<Route path="/home" component={HomePage} />
				<Route
					path="*"
					component={({ staticContext }) => {
						if (staticContext) staticContext.status = 404;

						return <h1>Not Found :(</h1>;
					}}
				/>
			</Switch>
		</div>
	);

	if (typeof window === 'undefined') {
		return (
			<StaticRouter
				context={this.props.context}
				location={this.props.location}
			>
				{app}
			</StaticRouter>
		);
	}
	return <BrowserRouter>{app}</BrowserRouter>;
}
```
