To complete the installation of System.Web.Optimization.React, you need to create one or more
bundles containing your JSX files:

// In BundleConfig.cs
bundles.Add(new BabelBundle("~/bundles/main").Include(
    // Add your JSX files here
    "~/Content/HelloWorld.react.jsx",
    "~/Content/AnythingElse.react.jsx",
    // You can include regular JavaScript files in the bundle too
    "~/Content/ajax.js",
));

Please refer to http://reactjs.net/ for more details, usage examples and sample code.