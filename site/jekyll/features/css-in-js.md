---
layout: docs
title: CSS-in-JS
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack.CoreMvc).

CSS-in-JS is a technique for declaring styles within components. ReactJS.NET supports server-rendered stylesheets for several libraries (pull requests welcome to add support for more!). Your project must be using a Javascript bundler such as webpack already.

Make sure ReactJS.NET is up to date. You will need at least ReactJS.NET 4.0 (which is in public beta at the time of writing).

### [Styled Components](https://github.com/styled-components/styled-components)

Expose styled-components as `global.Styled`:

```js
require('expose-loader?Styled!styled-components');
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.StylesheetHelpers

@{
	var styledComponentsFunctions = new StyledComponentsFunctions();
}

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: styledComponentsFunctions)

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

Expose react-jss as `global.ReactJss`:

```js
require('expose-loader?ReactJss!react-jss');
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.StylesheetHelpers

@{
	var reactJssFunctions = new ReactJssFunctions();
}

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: reactJssFunctions)

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

Emotion's integration with ReactJS.NET only supports rendering inline styles (instead of rendering them in the document head).

Expose emotion as `global.EmotionServer`:

```js
require('expose-loader?EmotionServer!emotion-server');
```

Add the render helper to the call to `Html.React`:

```
@using React.AspNet
@using React.StylesheetHelpers

@Html.React("RootComponent", new { exampleProp = "a" }, renderFunctions: new EmotionFunctions())
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
