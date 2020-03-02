import { Component, Fragment } from 'react';
import {
	Link,
	BrowserRouter,
	Route,
	Switch,
	StaticRouter,
	Redirect,
} from 'react-router-dom';
import { Helmet } from 'react-helmet';

import { CommentsBox } from './comments/Sample.jsx';
import { StyledComponentsDemo } from './styled-components.jsx';
import { EmotionDemo } from './emotion.jsx';
import { ReactJssDemo } from './react-jss.jsx';
import { LazyLoadDemo } from './lazy-load.jsx';

class Navbar extends Component {
	render() {
		return (
			<ul>
				<li>
					<Link to="/">Home</Link>
				</li>
				<li>
					<Link to="/comments">Comments Demo</Link>
				</li>
				<li>
					<Link to="/styled-components">Styled Components Demo</Link>
				</li>
				<li>
					<Link to="/react-jss">React-JSS Demo</Link>
				</li>
				<li>
					<Link to="/emotion">Emotion Demo</Link>
				</li>
				<li>
					<Link to="/lazy-load">Lazy loading demo</Link>
				</li>
			</ul>
		);
	}
}

class HomePage extends Component {
	render() {
		return (
			<Fragment>
				<Helmet>
					<title>ReactJS.NET Demos</title>
				</Helmet>
				ReactJS.NET is 🔥🔥
			</Fragment>
		);
	}
}

export default class HomeComponent extends Component {
	render() {
		const app = (
			<div className="container">
				<div className="jumbotron">
					<h1 className="display-4">.NET Core Sample</h1>
					<Navbar />
					<hr className="my-4" />
					<Switch>
						<Route
							exact
							path="/"
							render={() => <Redirect to="/home" />}
						/>
						<Route path="/home" component={HomePage} />
						<Route
							path="/comments"
							component={() => (
								<CommentsBox
									initialComments={this.props.initialComments}
									page={this.props.page}
								/>
							)}
						/>
						<Route
							path="/styled-components"
							component={StyledComponentsDemo}
						/>
						<Route path="/react-jss" component={ReactJssDemo} />
						<Route path="/emotion" component={EmotionDemo} />
						<Route path="/lazy-load" component={LazyLoadDemo} />
						<Route
							path="*"
							component={({ staticContext }) => {
								if (staticContext) staticContext.status = 404;

								return <h1>Not Found :(</h1>;
							}}
						/>
					</Switch>
				</div>
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
}
