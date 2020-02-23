---
layout: docs
title: Webpack
---

#### 👀  Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).

[Webpack](https://webpack.js.org/) is a popular module bundling system built on top of Node.js. It can handle not only combination and minification of JavaScript and CSS files, but also other assets such as image files (spriting) through the use of plugins. Webpack is the recommended bundling solution and should be preferred over Cassette or ASP.NET Bundling.

Your project will bundle its own copy of react and react-dom with webpack, and ReactJS.NET will be used only for server-side rendering.

For new projects, copy from the sample project to the root of your project:

- [package.json](https://github.com/reactjs/React.NET/blob/master/src/React.Sample.Webpack.CoreMvc/package.json), which includes everything you need to bundle with webpack
- [webpack.config.js](https://github.com/reactjs/React.NET/blob/master/src/React.Sample.Webpack.CoreMvc/webpack.config.js), which contains the configuration needed for webpack to create the bundles
- [.babelrc](https://github.com/reactjs/React.NET/blob/master/src/React.Sample.Webpack.CoreMvc/.babelrc), which contains the Babel settings needed to compile JSX files

Run `npm install` to start the package restore process.

Then, create the `Content/components/expose-components.js` file which will be the entrypoint for both your client and server-side Javascript.

```javascript
// Content/components/expose-components.js

import React from 'react';
import ReactDOM from 'react-dom';
import ReactDOMServer from 'react-dom/server';

import RootComponent from './home.jsx';

// any css-in-js or other libraries you want to use server-side
import { ServerStyleSheet } from 'styled-components';
import { renderStylesToString } from 'emotion-server';
import Helmet from 'react-helmet';

global.React = React;
global.ReactDOM = ReactDOM;
global.ReactDOMServer = ReactDOMServer;

global.Styled = { ServerStyleSheet };
global.Helmet = Helmet;

global.Components = { RootComponent };
```

Once Webpack has been configured, run `npm run build` to build the bundles. Once you have verified that the bundle is being created correctly, you can modify your ReactJS.NET configuration (normally `App_Start\ReactConfig.cs`) to load the newly-created bundle.

```csharp
ReactSiteConfiguration.Configuration
  .SetLoadBabel(false)
  .SetLoadReact(false)
  .AddScriptWithoutTransform("~/dist/runtime.js")
  .AddScriptWithoutTransform("~/dist/vendor.js")
  .AddScriptWithoutTransform("~/dist/components.js");
```

This will load all your components into the `Components` global, which can be used from `Html.React` to render any of the components:

```csharp
// at the top of your layout
@using React.AspNet

@Html.React("Components.RootComponent", new {
  someProp = "some value from .NET"
})
```

Reference the built bundle directly in a script tag at the end of the page in `_Layout.cshtml`:

```html
// at the top of your layout
@using React.AspNet

// before the closing </body> tag
<script src="/dist/runtime.js"></script>
<script src="/dist/vendor.js"></script>
<script src="/dist/components.js"></script>
@Html.ReactInitJavaScript()
```

A full example is available in [the ReactJS.NET repository](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).
