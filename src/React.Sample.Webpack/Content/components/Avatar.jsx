/**
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var React = require('react');

class Avatar extends React.Component {
	static propTypes = {
		author: PropTypes.object.isRequired
	};

	render() {
		return (
			<img
				src={this.getPhotoUrl(this.props.author)}
				alt={'Photo of ' + this.props.author.Name}
				width={50}
				height={50}
				className="commentPhoto"
			/>
		);
	}

	getPhotoUrl = (author) => {
		return 'https://avatars.githubusercontent.com/' + author.GithubUsername + '?s=50';
	};
}

module.exports = Avatar;
