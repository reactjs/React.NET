using System;

using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;

using Owin;

using React.Owin;

namespace React.Sample.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif

            app.Use(
                async (context, next) =>
                {
                    Console.WriteLine("{0} {1} {2}", context.Request.Method, context.Request.Path, context.Request.QueryString);

                    try
                    {
                        await next();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                        throw;
                    }
                });

            var contentFileSystem = new PhysicalFileSystem("Content");

            app.UseJsxFiles(new JsxFileOptions() { StaticFileOptions = new StaticFileOptions() { FileSystem = contentFileSystem }});
            app.UseFileServer(new FileServerOptions() { FileSystem = contentFileSystem });
        }
    }
}