var HelloWorld = React.createClass({
	render: function () {
		return React.DOM.div(null, 'Hello ', this.props.name);
	}
});