---
layout: docs
title: Webpack
---

[Webpack](http://webpack.github.io/docs/what-is-webpack.html) is a popular module bundling system built on top of Node.js. It can handle not only combination and minification of JavaScript and CSS files, but also other assets such as image files (spriting) through the use of plugins. Webpack can be used as an alternative to Cassette or ASP.NET Combination and Minification. This guide assumes you have already [installed Webpack](http://webpack.github.io/docs/installation.html).

In order to use Webpack with ReactJS.NET's server-side rendering, it is suggested that you create a separate bundle ("[entry point](http://webpack.github.io/docs/multiple-entry-points.html)") containing _only_ the code required to perform server-side rendering. Any components you would like to render server-side must be exposed globally so that ReactJS.NET can access them. This can be done by assigning properties to `global`. Even though `global` is not available in the browser, Webpack will add a shim that makes any assigned properties available in the global Javascript scope.

```javascript
// Content/components/index.js

// All the components you'd like to render server-side
export Avatar from './Avatar';
export Comment from './Comment';
export CommentsBox from './CommentsBox';
```

```javascript
// Content/server.js

// All JavaScript in here will be loaded server-side
// Expose components globally so ReactJS.NET can use them
import Components from './components';

global.Components = Components;
```

The next step is to modify the `webpack.config.js` so that it creates a bundle from `Content/server.js`. A config similar to the following could work as a good starting point:

```javascript
// This example still uses CommonJS syntax because Node hasn't yet shipped support for ES6 module syntax at the time of writing
var path = require('path');

module.exports = {
	entry: {
		server: './Content/server.js',
		client: './Content/client.js',
	},
	output: {
		filename: './wwwroot/[name].bundle.js',
	},
	module: {
		rules: [
			{
				test: /\.jsx?$/,
				exclude: /node_modules/,
				loader: 'babel-loader',
			},
		],
	},
	resolve: {
		// Allow require('./blah') to require blah.jsx
		extensions: ['', '.js', '.jsx'],
	},
};
```

This configuration uses two entry points (`Content/server.js` for the server side and `Content/client.js` for the client side) and will create two bundles (`build/server.bundle.js` and `build/client.bundle.js` respectively). Your configuration may be more complex, but generally you should only have a single bundle with all the code required for server-side rendering.

Our configuration also requires installation of the "babel" loader:

```sh
npm install --save-dev babel-loader
```

You will also need a `.babelrc` in the root of your project, with at least preset-react and preset-env enabled. Note that the plugins and presets need to be separately installed via `npm install --save-dev`.

```json
{
	"presets": ["@babel/preset-react", "@babel/preset-env"],
	"plugins": [
		"@babel/proposal-object-rest-spread",
		"@babel/plugin-syntax-dynamic-import",
		"@babel/proposal-class-properties"
	]
}
```

Once Webpack has been configured, run `webpack` to build the bundles. Once you have verified that the bundle is being created correctly, you can modify your ReactJS.NET configuration (normally `App_Start\ReactConfig.cs`) to load the newly-created bundle:

```csharp
ReactSiteConfiguration.Configuration
  .AddScriptWithoutTransform("~/wwwroot/server.bundle.js");
```

This will load all your components into the `Components` global, which can be used from `Html.React` to render any of the components:

```csharp
@Html.React("Components.CommentsBox", new {
  initialComments = Model.Comments
})
```

Reference the built bundle directly in a script tag at the end of the page:

```html
<script src="~/client.bundle.js"></script>
@Html.ReactInitJavaScript();
```

A full example is available in [the ReactJS.NET repository](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).

### Migrating from expose-loader

Mixing ES6 class syntax with CommonJS `require` has been the source of a lot of confusion, so we no longer recommend using `expose-loader`, `module.exports`, or `require`. Instead, use `import` and `export` statements throughout your whole React codebase when using Webpack. It is still fine to use `require` in webpack's config directly.
