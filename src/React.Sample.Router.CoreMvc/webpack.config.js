module.exports = {
	entry: './Content/components/expose-components.js',
	output: {
		filename: './wwwroot/components-bundle.generated.js'
	},
	module: {
		rules: [
			{
				test: /\.jsx?$/,
				exclude: /node_modules/,
				loader: 'babel-loader'
			}
		]
	}
};
