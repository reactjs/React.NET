import React from 'react';
import styled from 'styled-components';

const BlueTitle = styled.h1`
	color: lightslategray;
	font-family: Helvetica, 'sans-serif';
	text-shadow: 0 0 5px lightgreen;
`;

export function StyledComponentsDemo() {
	return <BlueTitle>Hello from styled-components!</BlueTitle>;
}
