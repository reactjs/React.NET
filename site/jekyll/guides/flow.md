---
layout: docs
title: Flow
---

ReactJS.NET has support for stripping out [Flow](http://flowtype.org/) type
annotations from your code. Flow is an open-source static type checker for
JavaScript developed by Facebook. It adds static typing to JavaScript to improve
developer productivity and code quality. You can learn more about Flow in
[its release announcement](https://code.facebook.com/posts/1505962329687926/flow-a-new-static-type-checker-for-javascript/).

How To Use It
-------------
Support for Flow is disabled by default. If you would like to use Flow type
annotations, you must enable it in your site's configuration (normally
`ReactConfig.cs`):

```csharp
ReactSiteConfiguration.Configuration.SetStripTypes(true);
```

This basically does the same thing as the offline transform tool mentioned in
[the Flow documentation](http://flowtype.org/docs/running.html) so you do
**not** need to run that tool manually.
