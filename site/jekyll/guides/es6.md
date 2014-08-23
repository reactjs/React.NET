---
layout: docs
title: ES6 Features
---

React can optionally use some ECMAScript 6 features thanks to the bundled version of [JSTransform](https://github.com/facebook/jstransform). ECMAScript 6 (or "ES6" for short) is the next version of ECMAScript/JavaScript and contains several useful features:

* **[Arrow functions](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/arrow_functions)** &mdash; A syntax for inline lambda functions similar to C#. These are very useful when combined with the `map` and `filter` methods of arrays:

```javascript
var numbers = [1, 2, 3, 4];
var doubled = numbers.map(number => number * 2); // [2, 4, 6, 8]
```

Arrow functions also implicitly bind `this`, so you do not need to write `.bind(this)` when passing around a function as a callback.

* **Concise methods** &mdash; You no longer need to write `: function` in your object literals:

```javascript{13,16}
// The old way
var OldAndBusted = React.createClass({
  render: function() {
    // ...
  },
  doStuff: function() {
    // ...
  }
});

// The new way
var NewHotness = React.createClass({
  render() {
    // ...
  },
  doStuff() {
    // ...
  }
});
```

* **[Classes](http://wiki.ecmascript.org/doku.php?id=strawman:maximally_minimal_classes)** &mdash; Similar to the class systems included in JavaScript frameworks such as Prototype and MooTools, but will (eventually) be native to JavaScript

```javascript
class AwesomeStuff {
  add(first, second) {
    return first + second;
  }
}

var foo = new AwesomeStuff();
foo.add(2, 3); // 5
```

* **[Short object notation](http://ariya.ofilabs.com/2013/02/es6-and-object-literal-property-value-shorthand.html)**
* And more! See the [JSTransform source code](https://github.com/facebook/jstransform/tree/master/visitors), you never know what goodies you'll find.

How to use
----------
To use the ES6 transforms, you'll need to enable them. For ASP.NET MVC sites, this is done in your `ReactConfig.cs` by calling `.SetUseHarmony(true)`:

```csharp{2}
ReactSiteConfiguration.Configuration
  .SetUseHarmony(true)
  .AddScript("~/Content/Sample.jsx");
```
If you are using [MSBuild to precompile your JSX](/guide/msbuild.html), you also need to enable it in MSBuild via the `UseHarmony="true"` flag in your build script (`TransformJsx.proj` by default):

```xml
<TransformJsx SourceDir="$(MSBuildProjectDirectory)" UseHarmony="true" />
```
