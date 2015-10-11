---
layout: default
title: React integration for ASP.NET MVC
id: home
---
<div class="hero">
  <div class="wrap">
    <div class="text"><strong>ReactJS.NET</strong></div>
    <div class="minitext">
      React ♥ C# and ASP.NET MVC
    </div>
  </div>
</div>
<section class="content wrap">
  <section class="home-section">
    <p>
      ReactJS.NET makes it easier to use Facebook's
      [React](http://facebook.github.io/react/) and
      [JSX](http://facebook.github.io/react/docs/jsx-in-depth.html) from C# and
      other .NET languages, focusing specifically on ASP.NET MVC (although it
      also works in other environments). It assumes you already have some basic
      knowledge about React. It is cross-platform and can run on Linux via Mono.
      Now with support for [ASP.NET 5 Beta](/getting-started/aspnet5.html)!
      Take a look at [the tutorial](/getting-started/tutorial.html) to see how
      easy it is to get started with React and ReactJS.NET!
    </p>
    <p>
      <em>
        Latest news:
        <a href="{{ site.posts.first.url }}">{{ site.posts.first.title }}</a>
        ({{ site.posts.first.date | date: "%B %e, %Y" }})
      </em>
    </p>
    <div id="examples">
      <div class="example">
        <h3>On-the-fly [JSX to JavaScript compilation](/getting-started/usage.html)</h3>
        <div class="example-desc">
          <p>
            Simply name your file with a `.jsx` extension and link to the
            file via a `script` tag.
          </p>
          <p>
            The files will automatically be compiled to JavaScript and cached
            server-side. No precompilation required. Perfect for development.
          </p>
        </div>
        <div class="example-code">

```javascript
// /Scripts/HelloWorld.jsx
var HelloWorld = React.createClass({
  render: function() {
    return <div>Hello world!</div>;
  }
});
```
```html
<!-- Reference it from HTML -->
<script src="@Url.Content("~/Scripts/HelloWorld.jsx")"></script>
```
</div>
      </div>
      <div class="example">
        <h3>JSX to JavaScript compilation via popular minification/combination libraries</h3>
        <div class="example-desc">
          <p>
            Use Cassette or ASP.NET Minification and Combination? ReactJS.NET's
            got you covered.
          </p>
          <p>
            Reference your JSX files and they will be included in your bundles
            along with your other JavaScript files.
          </p>
          <p>
            If you're a fan of Node.js-based libraries, you can use
            [Webpack](/guides/webpack.html) or Browserify instead, and still
            take advantage of ReactJS.NET's server-side rendering.
          </p>
        </div>
        <div class="example-code">

```csharp
// In BundleConfig.cs
bundles.Add(new JsxBundle("~/bundles/main").Include(
    // Add your JSX files here
    "~/Scripts/HelloWorld.jsx",
    "~/Scripts/AnythingElse.jsx",
    // You can include regular JavaScript files in the bundle too
    "~/Scripts/ajax.js",
));
```
```html
<!-- In your view -->
@Scripts.Render("~/bundles/main")
```
</div>
      </div>
      <div class="example">
        <h3>[Server-side component rendering](http://reactjs.net/guides/server-side-rendering.html)</h3>
        <div class="example-desc">
          <p>
            Pre-render the initial state of your React components server-side to
            make the initial load feel faster.
          </p>
        </div>
        <div class="example-code">

```html
<!-- This will render the component server-side -->
@Html.React("CommentsBox", new {
    initialComments = Model.Comments
})

<!-- Initialise the component in JavaScript too -->
<script src="https://fb.me/react-0.14.0.min.js"></script>
<script src="https://fb.me/react-dom-0.14.0.min.js"></script>
@Scripts.Render("~/bundles/main")
@Html.ReactInitJavaScript()
```
</div>
      </div>
    </div>
  </section>
  <hr class="home-divider" />
  <section class="home-bottom-section">
    <div class="buttons-unit">
      <a href="/getting-started/download.html" class="button">Get Started</a>
      <a href="/getting-started/tutorial.html" class="button">Tutorial</a>
    </div>
  </section>
</section>
