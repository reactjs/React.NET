import React, { Fragment } from 'react';
import styled from 'react-emotion';
import { Helmet } from 'react-helmet';

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
		<Fragment>
			<Helmet>
				<title>ReactJS.NET Demos | Emotion</title>
			</Helmet>
			<BlueTitle>
				Hello from{' '}
				<a href="https://github.com/emotion-js/emotion/">emotion</a>!
			</BlueTitle>
		</Fragment>
	);
}
