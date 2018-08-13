---
layout: docs
title: Webpack
---

This guide is for Webpack 1. To see the latest example for how to use webpack, check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack).

[Webpack](http://webpack.github.io/docs/what-is-webpack.html) is a popular module bundling system built on top of Node.js. It can handle not only combination and minification of JavaScript and CSS files, but also other assets such as image files (spriting) through the use of plugins. Webpack can be used as an alternative to Cassette or ASP.NET Combination and Minification. This guide assumes you have already [installed Webpack](http://webpack.github.io/docs/installation.html).

In order to use Webpack with ReactJS.NET's server-side rendering, it is suggested that you create a separate bundle ("[entry point](http://webpack.github.io/docs/multiple-entry-points.html)") containing *only* the code required to perform server-side rendering. Any components you would like to render server-side must be exposed globally so that ReactJS.NET can access them. This can be done through the [Webpack expose loader](https://github.com/webpack/expose-loader):

```javascript
// Content/components/index.js

module.exports = {
  // All the components you'd like to render server-side
  Avatar: require('./Avatar'),
  Comment: require('./Comment'),
  CommentsBox: require('./CommentsBox')
};
```
```javascript
// Content/server.js

// All JavaScript in here will be loaded server-side
// Expose components globally so ReactJS.NET can use them
var Components = require('expose-loader?Components!./components');
```

The next step is to modify the `webpack.config.js` so that it creates a bundle from `Content/server.js`. A config similar to the following could work as a good starting point:

```javascript
var path = require('path');

module.exports = {
  context: path.join(__dirname, 'Content'),
  entry: {
    server: './server',
    client: './client'
  },
  output: {
    path: path.join(__dirname, 'build'),
    filename: '[name].bundle.js'
  },
  module: {
    loaders: [
      // Transform JSX in .jsx files
      { test: /\.jsx$/, loader: 'jsx-loader?harmony' }
    ],
  },
  resolve: {
    // Allow require('./blah') to require blah.jsx
    extensions: ['', '.js', '.jsx']
  },
  externals: {
    // Use external version of React (from CDN for client-side, or
    // bundled with ReactJS.NET for server-side)
    react: 'React'
  }
};
```

This configuration uses two entry points (`Content/server.js` for the server side and `Content/client.js` for the client side) and will create two bundles (`build/server.bundle.js` and `build/client.bundle.js` respectively). Your configuration may be more complex, but generally you should only have a single bundle with all the code required for server-side rendering.

Our configuration also requires installation of the "expose" and "jsx" loaders:

```
npm install --save-dev expose-loader
npm install --save-dev jsx-loader
```

Once Webpack has been configured, run `webpack` to build the bundles. Once you have verified that the bundle is being created correctly, you can modify your ReactJS.NET configuration (normally `App_Start\ReactConfig.cs`) to load the newly-created bundle:

```csharp
ReactSiteConfiguration.Configuration
  .AddScript("~/build/server.bundle.js");
```

This will load all your components into the `Components` global, which can be used from `Html.React` to render any of the components:

```csharp
@Html.React("Components.CommentsBox", new {
  initialComments = Model.Comments
})
```

A full example is available in [the ReactJS.NET repository](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack).
