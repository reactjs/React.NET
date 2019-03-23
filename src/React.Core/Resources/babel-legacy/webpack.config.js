const path = require('path');

module.exports = [
	{
		entry: {
			'babel-legacy': './babel.js'
		},
		output: {
			filename: '[name].generated.min.js',
			globalObject: 'this',
			path: path.resolve(__dirname, '../'),
			libraryTarget: 'this'
		},
		mode: 'production',
		performance: {
			hints: false
		}
	}
];
