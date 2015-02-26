---
id: tutorial
title: Tutorial
layout: docs
---

This tutorial covers the end-to-end process of creating a brand new ASP.NET MVC website and adding a React component in it. We will start from scratch and end with a fully functioning component. It assumes you have basic knowledge of ASP.NET MVC and using Visual Studio. This tutorial is based off the [original React tutorial](http://facebook.github.io/react/docs/tutorial.html) but has been modified specifically for ReactJS.NET.

We'll be building a simple, but realistic comments box that you can drop into a blog, a basic version of the realtime comments offered by Disqus, LiveFyre or Facebook comments.

We'll provide:

* A view of all of the comments
* A form to submit a comment
* Simple server-side in-memory storage for comments

It'll also have a few neat features:

* **Optimistic commenting:** comments appear in the list before they're saved on the server so it feels fast.
* **Live updates:** as other users comment we'll pop them into the comment view in real time
* **Markdown formatting:** users can use Markdown to format their text

## Getting started

For this tutorial we'll be using Visual Studio 2013, although any version of Visual Studio from 2010 onwards is fine, including [Visual Studio Express 2013](http://www.visualstudio.com/en-us/products/visual-studio-express-vs.aspx) which is completely free. We will be using ASP.NET MVC 4, although similar steps apply for ASP.NET MVC 5.

### New Project

Start by creating a new ASP.NET MVC 4 project:

1. File → New &rarr; Project
2. Select ".NET Framework 4" and Templates → Visual C# → Web → ASP.NET MVC 4 Web Application. Call it "ReactDemo"
   <img src="/img/tutorial/newproject.png" alt="Screenshot: New Project" width="650" />
3. In the "New ASP.NET MVC 4 Project" dialog, select the Empty template. I always recommend using this template for new sites, as the others include a large amount of third-party packages that you may not even use.
   <img src="/img/tutorial/basicmvc.png" alt="Screenshot: New ASP.NET MVC 4 Project dialog" width="500" />

### Install ReactJS.NET

We need to install ReactJS.NET to the newly-created project. This is accomplished using NuGet, a package manager for .NET. Right-click on the "ReactDemo" project in the Solution Explorer and select "Manage NuGet Packages". Search for "ReactJS.NET" and install the **ReactJS.NET (MVC 4 and 5)** package.

<img src="/img/tutorial/nuget.png" alt="Screenshot: Install NuGet Packages" width="650" />

### Create basic controller and view

Since this tutorial focuses mainly on ReactJS.NET itself, we will not cover creation of an MVC controller in much detail. To learn more about ASP.NET MVC, refer to [its official website](http://www.asp.net/mvc).

Right-click on the Controllers folder and click Add → Controller. Name the controller "HomeController" and select "Empty MVC Controller" as the template. Once the controller has been created, right-click on `return View()` and click "Add View". Enter the following details:

 - View name: Index
 - View Engine: Razor (CSHTML)
 - Create a strongly-typed view: Unticked
 - Create as a partial view: Unticked
 - Use a layout or master page: Unticked

*Note: In a real ASP.NET MVC site, you'd use a layout. However, to keep this tutorial simple, we will keep all HTML in the one view file.*

Replace the contents of the new view file with the following:

```html
@{
    Layout = null;
}
<html>
<head>
	<title>Hello React</title>
</head>
<body>
	<div id="content"></div>
	<script src="http://fb.me/react-0.12.2.js"></script>
	<script src="@Url.Content("~/Scripts/Tutorial.jsx")"></script>
</body>
</html>
```

We also need to create the referenced JavaScript file (`Tutorial.jsx`). Right-click on ReactDemo project, select Add → New Folder, and enter "Scripts" as the folder name. Once created, right-click on the folder and select Add → New Item. Select Web → JavaScript File, enter "Tutorial.jsx" as the file name, and click "Add".

For the remainder of this tutorial, we'll be writing our JavaScript code in this file.

### Your first component

React is all about modular, composable components. For our comment box example, we'll have the following component structure:

```
- CommentBox
  - CommentList
    - Comment
  - CommentForm
```

Let's build the `CommentBox` component, which is just a simple `<div>`. Add this code to `Tutorial.jsx`:

```javascript
var CommentBox = React.createClass({
  render: function() {
    return (
      <div className="commentBox">
        Hello, world! I am a CommentBox.
      </div>
    );
  }
});
React.render(
  <CommentBox />,
  document.getElementById('content')
);
```

At this point, run your application by clicking the "Play" button in Visual Studio. If successful, your default browser should start and you should see "Hello, world! I am a CommentBox."

<img src="/img/tutorial/helloworld.png" alt="Screenshot: Hello ReactJS.NET World!" width="335" />

If you see this, congratulations! You've just built your first React component. You can leave the application running while you continue this tutorial. Simply change the JSX file and refresh to see your changes.

#### JSX Syntax

The first thing you'll notice is the XML-ish syntax in your JavaScript. We have a simple precompiler that translates the syntactic sugar to this plain JavaScript:

```javascript
var CommentBox = React.createClass({displayName: 'CommentBox',
  render: function() {
    return (
      React.createElement('div', {className: "commentBox"},
        "Hello, world! I am a CommentBox."
      )
    );
  }
});
React.render(
  React.createElement(CommentBox, null),
  document.getElementById('content')
);
```

Its use is optional but we've found JSX syntax easier to use than plain JavaScript. Read more on the [JSX Syntax article](http://facebook.github.io/react/docs/jsx-in-depth.html).

#### What's going on

We pass some methods in a JavaScript object to `React.createClass()` to create a new React component. The most important of these methods is called `render` which returns a tree of React components that will eventually render to HTML.

The `<div>` tags are not actual DOM nodes; they are instantiations of React `div` components. You can think of these as markers or pieces of data that React knows how to handle. React is **safe**. We are not generating HTML strings so XSS protection is the default.

You do not have to return basic HTML. You can return a tree of components that you (or someone else) built. This is what makes React **composable**: a key tenet of maintainable frontends.

`React.render()` instantiates the root component, starts the framework, and injects the markup into a raw DOM element, provided as the second argument.

## Composing components

Let's build skeletons for `CommentList` and `CommentForm` which will, again, be simple `<div>`s:

```javascript
var CommentList = React.createClass({
  render: function() {
    return (
      <div className="commentList">
        Hello, world! I am a CommentList.
      </div>
    );
  }
});

var CommentForm = React.createClass({
  render: function() {
    return (
      <div className="commentForm">
        Hello, world! I am a CommentForm.
      </div>
    );
  }
});
```

Next, update the `CommentBox` component to use its new friends:

```javascript{5-7}
var CommentBox = React.createClass({
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList />
        <CommentForm />
      </div>
    );
  }
});
```

Notice how we're mixing HTML tags and components we've built. HTML components are regular React components, just like the ones you define, with one difference. The JSX compiler will automatically rewrite HTML tags to `React.createElement(tagName)` expressions and leave everything else alone. This is to prevent the pollution of the global namespace.

### Component Properties

Let's create our third component, `Comment`. We will want to pass it the author name and comment text so we can reuse the same code for each unique comment. First let's add some comments to the `CommentList`:

```javascript{5-7}
var CommentList = React.createClass({
  render: function() {
    return (
      <div className="commentList">
        <Comment author="Daniel Lo Nigro">Hello ReactJS.NET World!</Comment>
        <Comment author="Pete Hunt">This is one comment</Comment>
        <Comment author="Jordan Walke">This is *another* comment</Comment>
      </div>
    );
  }
});
```

Note that we have passed some data from the parent `CommentList` component to the child `Comment` components. For example, we passed *Pete Hunt* (via an attribute) and *This is one comment* (via an XML-like child node) to the first `Comment`. Data passed from parent to children components is called **props**, short for properties.

### Using props

Let's create the Comment component. Using **props** we will be able to read the data passed to it from the `CommentList`, and render some markup:

```javascript
var Comment = React.createClass({
  render: function() {
    return (
      <div className="comment">
        <h2 className="commentAuthor">
          {this.props.author}
        </h2>
        {this.props.children}
      </div>
    );
  }
});
```

By surrounding a JavaScript expression in braces inside JSX (as either an attribute or child), you can drop text or React components into the tree. We access named attributes passed to the component as keys on `this.props` and any nested elements as `this.props.children`.

### Adding Markdown

Markdown is a simple way to format your text inline. For example, surrounding text with asterisks will make it emphasized.

First, add the third-party **Showdown** library to your application. This is a JavaScript library which takes Markdown text and converts it to raw HTML. We will add it via NuGet (search for "Showdown" and install it, similar to how you installed ReactJS.NET earlier) and reference the script tag in your view:

```html{2}
<script src="http://fb.me/react-0.12.2.js"></script>
<script src="@Url.Content("~/Scripts/showdown.min.js")"></script>
<script src="@Url.Content("~/Scripts/Tutorial.jsx")"></script>
```

Next, let's convert the comment text to Markdown and output it:

```javascript{3,9}
var Comment = React.createClass({
  render: function() {
    var converter = new Showdown.converter();
    return (
      <div className="comment">
        <h2 className="commentAuthor">
          {this.props.author}
        </h2>
        {converter.makeHtml(this.props.children.toString())}
      </div>
    );
  }
});
```

All we're doing here is calling the Showdown library. We need to convert `this.props.children` from React's wrapped text to a raw string that Showdown will understand so we explicitly call `toString()`.

But there's a problem! Our rendered comments look like this in the browser: "`<p>`This is `<em>`another`</em>` comment`</p>`". We want those tags to actually render as HTML.

That's React protecting you from an XSS attack. There's a way to get around it but the framework warns you not to use it:

```javascript{4,10}
var Comment = React.createClass({
  render: function() {
    var converter = new Showdown.converter();
    var rawMarkup = converter.makeHtml(this.props.children.toString());
    return (
      <div className="comment">
        <h2 className="commentAuthor">
          {this.props.author}
        </h2>
        <span dangerouslySetInnerHTML={{"{{"}}__html: rawMarkup}} />
      </div>
    );
  }
});
```

This is a special API that intentionally makes it difficult to insert raw HTML, but for Showdown we'll take advantage of this backdoor.

**Remember:** by using this feature you're relying on Showdown to be secure.

### Hook up the data model

So far we've been inserting the comments directly in the source code. Instead, let's render a blob of JSON data into the comment list. Eventually this will come from the server, but for now, write it in your source:

```javascript
var data = [
  { Author: "Daniel Lo Nigro", Text: "Hello ReactJS.NET World!" },
  { Author: "Pete Hunt", Text: "This is one comment" },
  { Author: "Jordan Walke", Text: "This is *another* comment" }
];
```

We need to get this data into `CommentList` in a modular way. Modify `CommentBox` and the `React.render()` call to pass this data into the `CommentList` via props:

```javascript{6,14}
var CommentBox = React.createClass({
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.props.data} />
        <CommentForm />
      </div>
    );
  }
});

React.render(
  <CommentBox data={data} />,
  document.getElementById('content')
);
```

Now that the data is available in the `CommentList`, let's render the comments dynamically:

```javascript{3-9,12}
var CommentList = React.createClass({
  render: function() {
    var commentNodes = this.props.data.map(function (comment) {
      return (
        <Comment author={comment.Author}>
          {comment.Text}
        </Comment>
      );
    });
    return (
      <div className="commentList">
        {commentNodes}
      </div>
    );
  }
});
```

That's it!

### Server-side Data

Let's return some data from the server. To do so, we need to first create a C# class to represent our comments. Right-click on the Models folder (which should be empty), select Add → Class, and enter "CommentModel.cs" as the file name. We'll create a basic comment model:

```csharp
namespace ReactDemo.Models
{
	public class CommentModel
	{
		public string Author { get; set; }
		public string Text { get; set; }
	}
}
```

In a real application, you'd use the repository pattern here, and retrieve the comments from a database. For simplicity, we'll just modify our controller to have a hard-coded list of comments.

```csharp{9,13-30}
using System.Collections.Generic;
using System.Web.Mvc;
using ReactDemo.Models;

namespace ReactDemo.Controllers
{
    public class HomeController : Controller
    {
	    private static readonly IList<CommentModel> _comments;

	    static HomeController()
	    {
			_comments = new List<CommentModel>
			{
				new CommentModel
				{
					Author = "Daniel Lo Nigro",
					Text = "Hello ReactJS.NET World!"
				},
				new CommentModel
				{
					Author = "Pete Hunt",
					Text = "This is one comment"
				},
				new CommentModel
				{
					Author = "Jordan Walke",
					Text = "This is *another* comment"
				},
			};
	    }

        public ActionResult Index()
        {
            return View();
        }
    }
}
```

Let's also add a new controller action to return the list of comments:

```csharp
public ActionResult Comments()
{
	return Json(_comments, JsonRequestBehavior.AllowGet);
}
```

And a corresponding route in `App_Start\RouteConfig.cs`:

```csharp{12-16}
using System.Web.Mvc;
using System.Web.Routing;

namespace ReactDemo
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Comments",
				url: "comments",
				defaults: new { controller = "Home", action = "Comments" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
```

If you hit `/comments` in your browser, you should now see the data encoded as JSON:

<img src="/img/tutorial/json.png" alt="Screenshot: JSON data source" width="468" />

### Fetching from the server

Now that we have a data source, we can replace the hard-coded data with the dynamic data from the server. We will remove the data prop and replace it with a URL to fetch:

```javascript{2}
React.render(
  <CommentBox url="/comments" />,
  document.getElementById('content')
);
```

This component is different from the prior components because it will have to re-render itself. The component won't have any data until the request from the server comes back, at which point the component may need to render some new comments.

### Reactive state

So far, each component has rendered itself once based on its props. `props` are immutable: they are passed from the parent and are "owned" by the parent. To implement interactions, we introduce mutable **state** to the component. `this.state` is private to the component and can be changed by calling `this.setState()`. When the state is updated, the component re-renders itself.

`render()` methods are written declaratively as functions of `this.props` and `this.state`. The framework guarantees the UI is always consistent with the inputs.

When the server fetches data, we will be changing the comment data we have. Let's add an array of comment data to the `CommentBox` component as its state:

```javascript{2-4,9}
var CommentBox = React.createClass({
  getInitialState: function() {
    return {data: []};
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm />
      </div>
    );
  }
});
```

`getInitialState()` executes exactly once during the lifecycle of the component and sets up the initial state of the component.

#### Updating state
When the component is first created, we want to GET some JSON from the server and update the state to reflect the latest data. We'll use the standard XMLHttpRequest API to retrieve the data. If you need support for old browsers (mainly old Internet Explorer), you can use an AJAX library or a multipurpose library such as jQuery.

```javascript{6-12}
var CommentBox = React.createClass({
  getInitialState: function() {
    return {data: []};
  },
  componentWillMount: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm />
      </div>
    );
  }
});
```

Here, `componentDidMount` is a method called automatically by React when a component is rendered. The key to dynamic updates is the call to `this.setState()`. We replace the old array of comments with the new one from the server and the UI automatically updates itself. Because of this reactivity, it is only a minor change to add live updates. We will use simple polling here but you could easily use [SignalR](http://signalr.net/) or other technologies.

```javascript{2,15-16,30}
var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    this.loadCommentsFromServer();
    window.setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm />
      </div>
    );
  }
});

React.render(
  <CommentBox url="/comments" pollInterval={2000} />,
  document.getElementById('content')
);
```

All we have done here is move the AJAX call to a separate method and call it when the component is first loaded and every 2 seconds after that.

### Adding new comments

To accept new comments, we need to first add a controller action to handle it. This will just be some simple C# code that appends the new comment to the static list of comments:

```csharp
[HttpPost]
public ActionResult AddComment(CommentModel comment)
{
	_comments.Add(comment);
	return Content("Success :)");
}
```
Let's also add it to the `App_Start\RouteConfig.cs` file, like we did earlier for the comments list:

```csharp
routes.MapRoute(
	name: "NewComment",
	url: "comments/new",
	defaults: new { controller = "Home", action = "AddComment" }
);

```

#### The Form

Now it's time to build the form. Our `CommentForm` component should ask the user for their name and comment text and send a request to the server to save the comment.

```javascript{4-8}
var CommentForm = React.createClass({
  render: function() {
    return (
      <form className="commentForm">
        <input type="text" placeholder="Your name" />
        <input type="text" placeholder="Say something..." />
        <input type="submit" value="Post" />
      </form>
    );
  }
});
```

Let's make the form interactive. When the user submits the form, we should clear it, submit a request to the server, and refresh the list of comments. To start, let's listen for the form's submit event and clear it.

```javascript{2-13,16-18}
var CommentForm = React.createClass({
  handleSubmit: function(e) {
    e.preventDefault();
    var author = this.refs.author.getDOMNode().value.trim();
    var text = this.refs.text.getDOMNode().value.trim();
    if (!text || !author) {
      return;
    }
    // TODO: send request to the server
    this.refs.author.getDOMNode().value = '';
    this.refs.text.getDOMNode().value = '';
    return;
  },
  render: function() {
    return (
      <form className="commentForm" onSubmit={this.handleSubmit}>
        <input type="text" placeholder="Your name" ref="author" />
        <input type="text" placeholder="Say something..." ref="text" />
        <input type="submit" value="Post" />
      </form>
    );
  }
});
```

##### Events

React attaches event handlers to components using a camelCase naming convention. We attach an `onSubmit` handler to the form that clears the form fields when the form is submitted with valid input.

Call `preventDefault()` on the event to prevent the browser's default action of submitting the form.

##### Refs

We use the `ref` attribute to assign a name to a child component and `this.refs` to reference the component. We can call `getDOMNode()` on a component to get the native browser DOM element.

##### Callbacks as props

When a user submits a comment, we will need to refresh the list of comments to include the new one. It makes sense to do all of this logic in `CommentBox` since `CommentBox` owns the state that represents the list of comments.

We need to pass data from the child component to its parent. We do this by passing a `callback` in props from parent to child:

```javascript{11-13,26}
var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  handleCommentSubmit: function(comment) {
    // TODO: submit to the server and refresh the list
  },
  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    this.loadCommentsFromServer();
    window.setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm onCommentSubmit={this.handleCommentSubmit} />
      </div>
    );
  }
});
```

Let's call the callback from the `CommentForm` when the user submits the form:

```javascript{9}
var CommentForm = React.createClass({
  handleSubmit: function(e) {
    e.preventDefault();
    var author = this.refs.author.getDOMNode().value.trim();
    var text = this.refs.text.getDOMNode().value.trim();
    if (!text || !author) {
      return;
    }
    this.props.onCommentSubmit({Author: author, Text: text});
    this.refs.author.getDOMNode().value = '';
    this.refs.text.getDOMNode().value = '';
    return;
  },
  render: function() {
    return (
      <form className="commentForm" onSubmit={this.handleSubmit}>
        <input type="text" placeholder="Your name" ref="author" />
        <input type="text" placeholder="Say something..." ref="text" />
        <input type="submit" value="Post" />
      </form>
    );
  }
});
```

Now that the callbacks are in place, all we have to do is submit to the server and refresh the list:

```javascript{12-21,42}
var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  handleCommentSubmit: function(comment) {
    var data = new FormData();
    data.append('Author', comment.Author);
    data.append('Text', comment.Text);

    var xhr = new XMLHttpRequest();
    xhr.open('post', this.props.submitUrl, true);
    xhr.onload = function() {
      this.loadCommentsFromServer();
    }.bind(this);
    xhr.send(data);
  },
  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    this.loadCommentsFromServer();
    window.setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm onCommentSubmit={this.handleCommentSubmit} />
      </div>
    );
  }
});

React.render(
  <CommentBox url="/comments" submitUrl="/comments/new" pollInterval={2000} />,
  document.getElementById('content')
);
```

## Congrats!

You have just built a comment box in a few simple steps. The below tweaks are not absolutely necessary, but they will improve the performance and polish of your application, so we suggest reading through them :)

We hope you have enjoyed learning about React, and how ReactJS.NET allows you to easily use it from an ASP.NET MVC web application. Learn more about [why to use React](http://facebook.github.io/react/docs/why-react.html) and how to [think about React components](http://facebook.github.io/react/docs/thinking-in-react.html), or dive into the [API reference](http://facebook.github.io/react/docs/top-level-api.html) and start hacking!

Continue on for more awesomeness!

## Optimization: optimistic updates

Our application is now feature complete but it feels slow to have to wait for the request to complete before your comment appears in the list. We can optimistically add this comment to the list to make the app feel faster.

```javascript{12-14}
var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  handleCommentSubmit: function(comment) {
    var comments = this.state.data;
    var newComments = comments.concat([comment]);
    this.setState({data: newComments});

    var data = new FormData();
    data.append('Author', comment.Author);
    data.append('Text', comment.Text);

    var xhr = new XMLHttpRequest();
    xhr.open('post', this.props.submitUrl, true);
    xhr.onload = function() {
      this.loadCommentsFromServer();
    }.bind(this);
    xhr.send(data);
  },
  getInitialState: function() {
    return {data: []};
  },
  componentDidMount: function() {
    this.loadCommentsFromServer();
    window.setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm onCommentSubmit={this.handleCommentSubmit} />
      </div>
    );
  }
});
```

## Optimization: Bundling and minification
Bundling refers to the practice of combining multiple JavaScript files into a single large file to reduce the number of HTTP requests to load a page. Minification refers to the removal of comments and unnecessary whitespace from JavaScript files to make them smaller. Together, bundling and minification can help to significantly improve the performance of your website. ReactJS.NET supports ASP.NET Bundling and Minification to achieve this. You can refer to [Microsoft's official documentation](http://www.asp.net/mvc/tutorials/mvc-4/bundling-and-minification) for more information on ASP.NET Bundling and Minification. This tutorial will just cover the basics.

To get started, install the "ReactJS.NET - JSX for ASP.NET Web Optimization Framework" NuGet package. This will automatically install the ASP.NET Bundling and Minification package along with all its dependencies.

Once installed, modify `BundleConfig.cs` to reference the Showdown and Tutorial JavaScript files:

```csharp{11-19}
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace ReactDemo
{
	public static class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new JsxBundle("~/bundles/main").Include(
				"~/Scripts/Tutorial.jsx",
				"~/Scripts/showdown.js"
			));

			// Forces files to be combined and minified in debug mode
			// Only used here to demonstrate how combination/minification works
			// Normally you would use unminified versions in debug mode.
			BundleTable.EnableOptimizations = true;
		}
	}
}
```

Now that the bundle has been registered, we need to reference it from the view:

```html{12}
@model IEnumerable<ReactDemo.Models.CommentModel>
@{
    Layout = null;
}
<html>
<head>
	<title>Hello React</title>
</head>
<body>
	<div id="content"></div>
	<script src="http://fb.me/react-0.12.2.js"></script>
	@Scripts.Render("~/bundles/main")
	@Html.ReactInitJavaScript()
</body>
</html>
```

That's it! Now if you view the source for the page, you should see a single script tag for the bundle:

```html
<!-- This is just an example; your URL will be different -->
<script src="/bundles/main?v=Or-R8LndNHguz2FwrDeQQg_o3wo7TjIZZnPKxmYJfRs1"></script>
```

If you go to this URL in your browser, you should notice that the code has been minified, and both the tutorial code and the Showdown code are in the same file.

## Optimization: Server-side rendering

Server-side rendering means that your application initially renders the components on the server-side, rather than fetching data from the server and rendering using JavaScript. This enhances the performance of your application since the user will see the initial state immediately.

We need to make some modifications to `CommentBox` to support server-side rendering. Firstly, we need to accept an `initialData` prop, which will be used to set the initial state of the component, rather than doing an AJAX request. We also need to remove the `loadCommentsFromServer` call from `componentDidMount`, since it is no longer required.

```javascript{28}
var CommentBox = React.createClass({
  loadCommentsFromServer: function() {
    var xhr = new XMLHttpRequest();
    xhr.open('get', this.props.url, true);
    xhr.onload = function() {
      var data = JSON.parse(xhr.responseText);
      this.setState({ data: data });
    }.bind(this);
    xhr.send();
  },
  handleCommentSubmit: function(comment) {
    var comments = this.state.data;
    var newComments = comments.concat([comment]);
    this.setState({data: newComments});

	var data = new FormData();
	data.append('Author', comment.Author);
	data.append('Text', comment.Text);

    var xhr = new XMLHttpRequest();
    xhr.open('post', this.props.submitUrl, true);
    xhr.onload = function() {
      this.loadCommentsFromServer();
    }.bind(this);
    xhr.send(data);
  },
  getInitialState: function() {
    return { data: this.props.initialData };
  },
  componentDidMount: function() {
    window.setInterval(this.loadCommentsFromServer, this.props.pollInterval);
  },
  render: function() {
    return (
      <div className="commentBox">
        <h1>Comments</h1>
        <CommentList data={this.state.data} />
        <CommentForm onCommentSubmit={this.handleCommentSubmit} />
      </div>
    );
  }
});
```

In the view, we will accept the list of comments as the model, and use `Html.React` to render the component. This will replace the `React.render` call that currently exists in Tutorial.jsx. All the props from the current `React.render` call should be moved here, and the `React.render` call should be deleted.

```html{1,10-16,20}
@model IEnumerable<ReactDemo.Models.CommentModel>
@{
    Layout = null;
}
<html>
<head>
	<title>Hello React</title>
</head>
<body>
	@Html.React("CommentBox", new
	{
		initialData = Model,
		url = Url.Action("Comments"),
		submitUrl = Url.Action("AddComment"),
		pollInterval = 2000,
	})
	<script src="http://fb.me/react-0.12.2.js"></script>
	<script src="@Url.Content("~/Scripts/showdown.min.js")"></script>
	<script src="@Url.Content("~/Scripts/Tutorial.jsx")"></script>
	@Html.ReactInitJavaScript()
</body>
</html>
```

We need to modify the controller action to pass the data to the view:

```csharp{3}
public ActionResult Index()
{
    return View(_comments);
}
```

We also need to modify `App_Start\ReactConfig.cs` to tell ReactJS.NET which JavaScript files it requires for the server-side rendering:

```csharp{13-15}
using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ReactDemo.ReactConfig), "Configure")]

namespace ReactDemo
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration
				.AddScript("~/Scripts/showdown.js")
				.AddScript("~/Scripts/Tutorial.jsx");
		}
	}
}
```

That's it! Now if you build and refresh your application, you should notice that the comments box is rendered immediately rather than having a slight delay. If you view the source of the page, you will see the initial comments directly in the HTML itself:

```html
<html>
<head>
  <title>Hello React</title>
</head>
<body>
  <div id="react1">
    <div class="commentBox" data-reactid=".2ged0u96as7" data-react-checksum="118121939">
      <h1 data-reactid=".2ged0u96as7.0">Comments</h1>
      <div class="commentList" data-reactid=".2ged0u96as7.1">
        <div class="comment" data-reactid=".2ged0u96as7.1.0">
          <h2 class="commentAuthor" data-reactid=".2ged0u96as7.1.0.0">Daniel Lo Nigro</h2>
          <span data-reactid=".2ged0u96as7.1.0.1"><p>Hello ReactJS.NET World!</p></span>
        </div>

	<!-- Rest of the contents ommitted for brevity -->
```
