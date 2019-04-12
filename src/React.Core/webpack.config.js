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
		node: {
			fs: 'empty',
		},
		performance: {
			hints: false,
		},
	},
];
