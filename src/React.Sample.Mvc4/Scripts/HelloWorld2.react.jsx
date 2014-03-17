/** @jsx React.DOM */
var HelloWorld2 = React.createClass({
	render: function () {
		return (
			<div className="awesome">
				Hello {this.props.name}
			</div>
		);
	}
});