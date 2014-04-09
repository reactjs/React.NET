using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nancy.Responses;
using React;

namespace Nancy.ViewEngines.React
{
    /// <summary>
    /// 
    /// </summary>
    public class ReactViewEngine : IViewEngine
    {
        private static readonly IEnumerable<string> SupportedExtensions = new[] { "jsx" };

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Extensions
        {
            get { return SupportedExtensions; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewEngineStartupContext"></param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewLocationResult"></param>
        /// <param name="model"></param>
        /// <param name="renderContext"></param>
        /// <returns></returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(statusCode: HttpStatusCode.OK, contents: stream => {
                var html = RenderComponent(viewLocationResult, model, renderContext);
                var writer = new StreamWriter(stream);
                writer.Write("<!DOCTYPE html>");
                writer.Write(html);
                writer.Flush();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewLocationResult"></param>
        /// <param name="model"></param>
        /// <param name="renderContext"></param>
        /// <returns></returns>
        public string RenderComponent(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            string content;

            using (var reader = viewLocationResult.Contents.Invoke())
            {
                content = reader.ReadToEnd();
            }

            using (var jsEngine = new JavaScriptEngineFactory())
            {
                var react = new ReactEnvironment(
                    jsEngine,
                    new ReactSiteConfiguration(),
                    new NullReactCache(),
                    new NullReactFileSystem());

                // this is bad and should be cached instead
                var js = react.TransformJsx(content);
                react.Execute(js);

                var componentName = GetComponentName(viewLocationResult);

                IReactComponent component = react.CreateComponent<object>(componentName, model);

                var html = component.RenderHtml();

                return html;
            }
        }

        private string GetComponentName(ViewLocationResult viewLocationResult)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(viewLocationResult.Name);
        }

        // we don't use this currently
        class NullReactFileSystem : IFileSystem
        {
            public string MapPath(string relativePath)
            {
                throw new NotImplementedException();
            }

            public string ReadAsString(string relativePath)
            {
                throw new NotImplementedException();
            }
        }

    }
}