import { Component } from 'react';
import {
	Link,
	BrowserRouter,
	Route,
	Switch,
	StaticRouter,
	Redirect
} from 'react-router-dom';

class Navbar extends Component {
	render() {
		return (
			<ul>
				<li>
					<Link to="/">Home</Link>
				</li>
				<li>
					<Link to="/about">About</Link>
				</li>
				<li>
					<Link to="/contact">Contact</Link>
				</li>
			</ul>
		);
	}
}

class HomePage extends Component {
	render() {
		return <h1>Home</h1>;
	}
}

class AboutPage extends Component {
	render() {
		return <h1>About</h1>;
	}
}

class ContactPage extends Component {
	render() {
		return <h1>Contact</h1>;
	}
}

export default class HomeComponent extends Component {
	render() {
		const app = (
			<div>
				<Navbar />
				<Switch>
					<Route exact path="/" render={() => <Redirect to="/home" />} />
					<Route path="/home" component={HomePage} />
					<Route path="/about" component={AboutPage} />
					<Route path="/contact" component={ContactPage} />
					<Route
						path="*"
						component={({ staticContext }) => {
							if (staticContext) staticContext.status = 404;

							return <h1>Not Found :(</h1>;
						}}
					/>
				</Switch>
			</div>
		);

		if (typeof window === 'undefined') {
			return (
				<StaticRouter context={this.props.context} location={this.props.path}>
					{app}
				</StaticRouter>
			);
		}
		return <BrowserRouter>{app}</BrowserRouter>;
	}
}
