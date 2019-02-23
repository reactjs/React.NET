const path = require('path');

module.exports = [
	{
		entry: './Resources/react.js',
		output: {
			filename: 'react.generated.js',
			globalObject: 'this',
			path: path.resolve(__dirname, 'Resources/'),
			libraryTarget: 'this',
		},
		mode: 'development',
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
	{
		entry: {
			react: './Resources/react.js',
			babel: './Resources/babel.js',
		},
		output: {
			filename: '[name].generated.min.js',
			globalObject: 'this',
			path: path.resolve(__dirname, 'Resources/'),
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
