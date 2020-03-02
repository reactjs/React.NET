---
layout: docs
title: CSS-in-JS
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Template/reactnet-webpack).

CSS-in-JS is a technique for declaring styles within components. ReactJS.NET supports server-rendered stylesheets for several libraries (pull requests welcome to add support for more!). Your project must be using a Javascript bundler such as webpack already.

Make sure ReactJS.NET is up to date. You will need at least ReactJS.NET 4.0 (which is in public beta at the time of writing).

If you're using more than one CSS-in-JS library in your project, we've got you covered! Just pass mutliple server-render helper functions into `ChainedRenderFunctions`, and they will be called in the order they are passed in.

### [Styled Components](https://github.com/styled-components/styled-components)

#### ⚠️  This may break when styled-components publishes a major update, please look at the [webpack sample](https://github.com/reactjs/React.NET/blob/master/src/React.Template/reactnet-webpack/package.json) for the currently known compatible version.

Expose styled-components in your server bundle:

```js
import { ServerStyleSheet } from 'styled-components';
global.Styled = { ServerStyleSheet };
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.RenderFunctions

@{
	var styledComponentsFunctions = new StyledComponentsFunctions();
}

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: new ChainedRenderFunctions(styledComponentsFunctions))

@{
	ViewBag.ServerStyles = styledComponentsFunctions.RenderedStyles;
}
```

In your layout file, render the styles that are in the ViewBag:

```
<!DOCTYPE html>
<html>
<head>
	<title>React Router Sample</title>
	<meta charset="utf-8" />
	@Html.Raw(ViewBag.ServerStyles)
</head>
<body>
	@RenderBody()
</body>
</html>
```

You're now ready to declare styles inside components:

```
import React from 'react';
import styled from 'styled-components';

const BlueTitle = styled.h1`
	color: #222;
	font-family: Helvetica, 'sans-serif';
	text-shadow: 0 0 5px lightgray;
	line-height: 2;

	a {
		transition: color 0.2s ease;
		color: palevioletred;
		text-decoration: none;

		&:hover {
			color: #888;
		}
	}
`;

export function StyledComponentsDemo() {
	return (
		<BlueTitle>
			Hello from{' '}
			<a href="https://github.com/styled-components/styled-components">
				styled-components
			</a>
			!
		</BlueTitle>
	);
}
```

### [React-JSS](https://github.com/cssinjs/react-jss)

#### ⚠️  This may break when react-jss publishes a major update, please look at the [webpack sample](https://github.com/reactjs/React.NET/blob/master/src/React.Template/reactnet-webpack/package.json) for the currently known compatible version.

Expose react-jss in your server bundle:

```js
import { JssProvider, SheetsRegistry } from 'react-jss';
global.ReactJss = { JssProvider, SheetsRegistry };
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.RenderFunctions

@{
	var reactJssFunctions = new ReactJssFunctions();
}

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: new ChainedRenderFunctions(reactJssFunctions))

@{
	ViewBag.ServerStyles = reactJssFunctions.RenderedStyles;
}
```

In your layout file, render the styles that are in the ViewBag:

```
<!DOCTYPE html>
<html>
<head>
	<title>React Router Sample</title>
	<meta charset="utf-8" />
	@Html.Raw(ViewBag.ServerStyles)
</head>
<body>
	@RenderBody()
</body>
</html>
```

You're now ready to declare styles inside components:

```
import React from 'react';
import injectSheet from 'react-jss';

const styles = {
	demoTitle: {
		color: '#222',
		fontFamily: 'Helvetica, sans-serif',
		textShadow: '0 0 5px lightgray',
		lineHeight: '2',
		'& a': {
			transition: 'color 0.2s ease',
			color: 'palevioletred',
			'text-decoration': 'none',

			'&:hover': {
				color: '#888',
			},
		},
	},
};

const DemoTitle = ({ classes, children }) => (
	<h1 className={classes.demoTitle}>
		Hello from <a href="https://github.com/cssinjs/react-jss">React-JSS</a>!
	</h1>
);

const WithInjectedSheet = injectSheet(styles)(DemoTitle);

export class ReactJssDemo extends React.Component {
	componentDidMount() {
		const serverStyles = document.getElementById('server-side-styles');
		if (serverStyles) {
			serverStyles.parentNode.removeChild(serverStyles);
		}
	}

	render() {
		return <WithInjectedSheet />;
	}
}
```

### Emotion

#### ⚠️  This may break when emotion publishes a major update, please look at the [webpack sample](https://github.com/reactjs/React.NET/blob/master/src/React.Template/reactnet-webpack/package.json) for the currently known compatible version.

Emotion's integration with ReactJS.NET only supports rendering inline styles (instead of rendering them in the document head).

Expose emotion in your server bundle:

```js
import { renderStylesToString } from 'emotion-server';
global.EmotionServer = { renderStylesToString };
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.RenderFunctions

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: new ChainedRenderFunctions(new EmotionFunctions()))
```

You're now ready to declare styles inside components:

```
import React from 'react';
import styled from 'react-emotion';

const BlueTitle = styled('h1')`
	color: #222;
	font-family: Helvetica, 'sans-serif';
	text-shadow: 0 0 5px lightgray;
	line-height: 2;

	a {
		transition: color 0.2s ease;
		color: palevioletred;
		text-decoration: none;

		&:hover {
			color: #888;
		}
	}
`;

export function EmotionDemo() {
	return (
		<BlueTitle>
			Hello from{' '}
			<a href="https://github.com/emotion-js/emotion/">emotion</a>!
		</BlueTitle>
	);
}
```
