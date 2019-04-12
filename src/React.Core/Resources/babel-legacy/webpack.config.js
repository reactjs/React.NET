const path = require('path');

module.exports = [
	{
		entry: {
			'babel-legacy': './babel.js',
		},
		output: {
			filename: '[name].generated.min.js',
			globalObject: 'this',
			path: path.resolve(__dirname, '../'),
			libraryTarget: 'this',
		},
		mode: 'production',
		module: {
			rules: [
				{
					test: /\.jsx?$/,
					exclude: /node_modules/,
					loader: 'babel-loader',
					query: {
						presets: ['es2015', 'stage-0'],
					},
				},
			],
		},
		performance: {
			hints: false,
		},
	},
];
