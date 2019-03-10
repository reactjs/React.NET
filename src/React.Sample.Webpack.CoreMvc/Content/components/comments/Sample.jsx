/**
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

import React from 'react';
import PropTypes from 'prop-types';
import * as Reactstrap from 'reactstrap';

export function CommentsBox(props) {
	let [state, updateState] = React.useState({
		comments: props.initialComments,
		page: props.page,
		hasMore: true,
		loadingMore: false,
	});

	function loadMoreClicked(evt) {
		let nextPage = state.page + 1;
		let comments = state.comments;
		updateState(prevState => ({
			...prevState,
			page: nextPage,
			loadingMore: true,
		}));

		let url = '/comments/page-' + (state.page + 1);
		let xhr = new XMLHttpRequest();
		xhr.open('GET', url, true);
		xhr.setRequestHeader('Content-Type', 'application/json');

		xhr.onload = () => {
			let data = JSON.parse(xhr.responseText);
			updateState(prevState => ({
				...prevState,
				comments: comments.concat(data.comments),
				hasMore: data.hasMore,
				loadingMore: false,
			}));
		};
		xhr.send();
		evt.preventDefault();
	}

	let commentNodes = state.comments.map(comment => (
		<Comment author={comment.author}>{comment.text}</Comment>
	));

	function renderMoreLink() {
		if (state.loadingMore) {
			return <em>Loading...</em>;
		} else if (state.hasMore) {
			return (
				<Reactstrap.Button onClick={loadMoreClicked}>
					Load More
				</Reactstrap.Button>
			);
		} else {
			return <em>No more comments</em>;
		}
	}

	return (
		<div>
			<p className="lead">
				This is an example of ReactJS.NET's server-side rendering. The
				initial state of this comments box is rendered server-side, and
				additional data is loaded via AJAX and rendered client-side.
			</p>
			<div className="comments">
				<h1>Comments</h1>
				<ol className="commentList">{commentNodes}</ol>
				{renderMoreLink()}
				<hr />
			</div>
		</div>
	);
}

class Comment extends React.Component {
	static propTypes = {
		author: PropTypes.object.isRequired,
	};

	render() {
		return (
			<li>
				<Avatar author={this.props.author} />
				<strong>{this.props.author.name}</strong>
				{': '}
				{this.props.children}
			</li>
		);
	}
}

class Avatar extends React.Component {
	static propTypes = {
		author: PropTypes.object.isRequired,
	};

	render() {
		return (
			<img
				src={this.getPhotoUrl(this.props.author)}
				alt={'Photo of ' + this.props.author.name}
				width={50}
				height={50}
				className="commentPhoto mr-1"
			/>
		);
	}

	getPhotoUrl = author => {
		return (
			'https://avatars.githubusercontent.com/' +
			author.githubUsername +
			'?s=50'
		);
	};
}
