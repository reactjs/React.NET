/**
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var Avatar = require('./Avatar');
var React = require('react');

class Comment extends React.Component {
	static propTypes = {
		author: PropTypes.object.isRequired
	};

	render() {
		return (
			<li>
				<Avatar author={this.props.author} />
				<strong>{this.props.author.Name}</strong>{': '}
				{this.props.children}
			</li>
		);
	}
}

module.exports = Comment;
