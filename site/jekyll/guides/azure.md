---
layout: docs
title: Azure
---

From **version 2.2.0** onwards ReactJS.NET works out of the box in Azure using the V8 JavaScript engine. Versions prior to 2.2.0 will fall back to using MSIE JavaScript engine and you may experience JavaScript errors during server-side rendering that you aren't experiencing locally.

You can run the following code to check which JavaScript engines are available on the machine that your application is running on. The engine with the lowest priority is used by ReactJS.NET.

```csharp
public string GetAvailableEngines()
{
    var sb = new StringBuilder();
    var registrations = React.TinyIoC.TinyIoCContainer.Current.ResolveAll<JavaScriptEngineFactory.Registration>();
    foreach (var registration in registrations.OrderBy(r => r.Priority))
    {
        try
        {
            var engine = registration.Factory();
            var result = engine.Evaluate<int>("1 + 1");
            if (result == 2)
            {
                sb.AppendLine($"Engine: {engine.Name}, version: {engine.Version}, priority: {registration.Priority}");
            }

        }
        catch { }
    }

    return sb.ToString();
}
```

To force ReactJS.NET to use the V8 JavaScript engine (and throw an exception if it isn't available) set the AllowMsieEngine configuration property to false.

```csharp
app.UseReact(config =>
            {
                config
                    // ..other configuration settings
                    .SetAllowMsieEngine(false);
            });
```