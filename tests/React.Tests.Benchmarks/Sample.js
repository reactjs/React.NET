function HelloWorld(props) {
	return React.createElement(
		"div",
		null,
		"Hello ",
		props.name,
		"!"
	);
}
