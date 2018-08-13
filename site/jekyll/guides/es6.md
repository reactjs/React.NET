---
layout: docs
title: ES6 Features (Babel)
---

ReactJS.NET supports the use of ECMAScript 6 features, thanks to [Babel](http://babeljs.io/). These features include:

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
class NewHotness extends React.Component {
  render() {
    // ...
  },
  doStuff() {
    // ...
  }
};
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
* And more! See the [Babel website](http://babeljs.io/docs/learn-es2015/) for a full list of supported features.
