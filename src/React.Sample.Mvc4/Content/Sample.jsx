/**
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var CommentsBox = React.createClass({
	propTypes: {
		initialComments: React.PropTypes.array.isRequired
	},
	getInitialState() {
		return {
			comments: this.props.initialComments,
			page: 1,
			hasMore: true,
			loadingMore: false
		};
	},
	loadMoreClicked(evt) {
		var nextPage = this.state.page + 1;
		this.setState({
			page: nextPage,
			loadingMore: true
		});

		var url = evt.target.href;
		var xhr = new XMLHttpRequest();
		xhr.open('GET', url, true);
		xhr.onload = () => {
			var data = JSON.parse(xhr.responseText);
			this.setState({
				comments: this.state.comments.concat(data.comments),
				hasMore: data.hasMore,
				loadingMore: false
			});
		};
		xhr.send();
		return false;
	},
	render() {
		var commentNodes = this.state.comments.map(comment =>
			<Comment author={comment.Author}>{comment.Text}</Comment>
		);

		return (
			<div className="comments">
				<h1>Comments</h1>
				<ol className="commentList">
					{commentNodes}
				</ol>
				{this.renderMoreLink()}
			</div>
		);
	},
	renderMoreLink() {
		if (this.state.loadingMore) {
			return <em>Loading...</em>;
		} else if (this.state.hasMore) {
			return (
				<a href={'comments/page-' + (this.state.page + 1)} onClick={this.loadMoreClicked}>
					Load More
				</a>
			);
		} else {
			return <em>No more comments</em>;
		}
	}
});

var Comment = React.createClass({
	propTypes: {
		author: React.PropTypes.object.isRequired
	},
	render() {
		return (
			<li>
				<Avatar author={this.props.author} />
				<strong>{this.props.author.Name}</strong>{': '}
				{this.props.children}
			</li>
		);
	}
});

var Avatar = React.createClass({
	propTypes: {
		author: React.PropTypes.object.isRequired	
	},
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
	},
	getPhotoUrl(author) {
		return 'http://graph.facebook.com/' + author.Facebook + '/picture';
	}
});
