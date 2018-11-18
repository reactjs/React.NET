import React, { Fragment } from 'react';
import injectSheet from 'react-jss';
import { Helmet } from 'react-helmet';

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
		return (
			<Fragment>
				<Helmet>
					<title>ReactJS.NET Demos | React-JSS</title>
				</Helmet>
				<WithInjectedSheet />
			</Fragment>
		);
	}
}
