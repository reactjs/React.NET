class HelloWorld extends React.Component {
	render() {
		return (
			<div>
				Hello {this.props.name}!
				All props: {JSON.stringify(this.props)}
			</div>
		);
	}
}
