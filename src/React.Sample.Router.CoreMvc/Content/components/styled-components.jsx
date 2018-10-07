import React from 'react';
import {
	Link,
	BrowserRouter,
	Route,
	Switch,
	StaticRouter,
	Redirect
} from 'react-router-dom';
import styled from 'styled-components';

const BlueTitle = styled.h1`
	color: lightslategray;
	font-family: Helvetica, 'sans-serif';
	text-shadow: 0 0 5px lightgreen;
`;

export function StyledComponentsDemo() {
	return <BlueTitle>Hello from styled-components!</BlueTitle>;
}
