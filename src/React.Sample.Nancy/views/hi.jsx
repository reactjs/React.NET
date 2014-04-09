/** @jsx React.DOM */

var Hi = React.createClass({
	render: function () {
		return (
			<html>
				<head></head>
				<body>
					<h1>Hi {this.props.name}</h1>
				</body>
			</html>
		)
	}
});
