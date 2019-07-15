---
layout: docs
title: Typescript compilation
---

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4).

Typescript is a library for writing type-safe Javascript. Starting with version 5, ReactJS.NET supports stripping out type definitions from `.TS` and .`TSX` files, powered by Babel.

Note that just using the library will compile your Typescript to Javascript, but will _not_ warn you about code that does not type check. To set up type checking in your project during the build:

1. Install a supported version of [Node](https://nodejs.org/en/download/) (either LTS or Current is fine)
1. Create an empty `package.json` to your project root by running `npm init`. Optionally fill out the questions asked by `npm`, or press Enter to accept the defaults.
1. Run `npm i typescript --save-dev`, which will update the freshly generated `package.json`. It's important that the Typescript version is declared in this file so every developer on your project has the same type checking rules.
1. Copy the [tsconfig.json](https://github.com/reactjs/react.net/blob/master/src/React.Sample.Mvc4/tsconfig.json) file from the Mvc sample to your project root. If your components are not located in `Content`, change that path to the appropriate directory.
1. Typescript needs to be informed of the libraries available on the global scope. To do this, create [types/index.d.ts](https://github.com/reactjs/react.net/blob/master/src/React.Sample.Mvc4/types/index.d.ts) in your project root:

```ts
import _React from 'react';
import _PropTypes from 'prop-types'; // @types/prop-types is a dependency of `@types/react`
import _Reactstrap from 'reactstrap'; // Third party library example

declare global {
	const React: typeof _React; // the React types _also_ exported by the React namespace, but export them again here just in case.
	const PropTypes: typeof _PropTypes;
	const Reactstrap: typeof _Reactstrap;
}
```

Libraries imported in `types/index.d.ts` must be listed in `package.json` before typescript will load their type definitions. Types for `react` are defined by the `@types/react` library in the [DefinitelyTyped](https://github.com/DefinitelyTyped/DefinitelyTyped/tree/master/types/react) repo, so install the types package with `npm i --save-dev @types/react`. Sometimes libraries will ship with typescript support; if so, install the package directly via `npm i --save-dev <library name>` to make the types resolve. If a library you're using does not ship with types, chances are there will be community-provided types in DefinitelyTyped.

To check that everything works at this point, run `node_modules/.bin/tsc` from your project's working directory. You'll see empty output from `tsc` if the type checking succeeds.

Finally, add a compile-time step to your project file to get type checking with every Visual Studio build (works in both ASP.NET and .NET Core):

```xml
<Target Name="Typecheck" AfterTargets="AfterBuild">
	<Exec WorkingDirectory="$(MSBuildProjectDirectory)" Command="node_modules/.bin/tsc" />
</Target>
```

You're done! Introduce a type error in your project to verify things are working as expected. For example, you will see a message similar to `6>C:\code\react.net\src\React.Sample.Mvc4\Content/Sample.tsx(27,19): error TS2551: Property 'initialCommentss' does not exist on type 'CommentsBoxProps'. Did you mean 'initialComments'?`

Check out the [sample project](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4) for the completed integration.
