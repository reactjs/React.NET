const path = require('path');

const bundle = {
	entry: {
		components: './Content/components/expose-components.js',
	},
	devtool: 'sourcemap',
	output: {
		filename: '[name].js',
		globalObject: 'this',
		path: path.resolve(__dirname, 'wwwroot/dist'),
		publicPath: 'dist/'
	},
	mode: process.env.NODE_ENV === 'production' ? 'production' : 'development',
	optimization: {
		runtimeChunk: {
			name: 'runtime', // necessary when using multiple entrypoints on the same page
		},
		splitChunks: {
			cacheGroups: {
				commons: {
					test: /[\\/]node_modules[\\/](react|react-dom)[\\/]/,
					name: 'vendor',
					chunks: 'all',
				},
			},
		},
	},
	module: {
		rules: [
			{
				test: /\.jsx?$/,
				exclude: /node_modules/,
				loader: 'babel-loader',
			},
		],
	},
};

module.exports = [
	{
		...bundle,
		target: 'web'
	},
	{
		devtool: bundle.devtool,
		entry: bundle.entry,
		mode: bundle.mode,
		module: bundle.module,
		target: 'node',
		output: {
			...bundle.output,
			path: path.resolve(__dirname, 'wwwroot/server/dist'),
			libraryTarget: 'commonjs',
		},
	}
]
