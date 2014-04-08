/** @jsx React.DOM */

var Form = React.createClass({
    render: function () {
        return (
            <form method="POST">
				Name: <input type="text" name="name" />
				<input type="submit" value="submit"/>
			</form>
        );
    }
});

var Home = React.createClass({
	render: function () {
		return (
			<html>
				<head></head>
				<body>
					<Form />
				</body>
			</html>
		)
	}
});

module.exports = Home;