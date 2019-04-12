type HelloProps = { name: string, testProp: string };

class HelloTypescript extends React.Component<HelloProps> {
	testProp = this.props.testProp || 'no prop';
	spreadDemo = { ...this.props };
	render() {
		return (
			<div>
				Hello {this.spreadDemo.name}! Passed in: {this.testProp}
			</div>
		);
	}
}
