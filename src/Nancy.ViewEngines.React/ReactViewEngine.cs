using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nancy.Responses;
using React;

namespace Nancy.ViewEngines.React
{
    /// <summary>
    /// Nancy React View Engine
    /// </summary>
    public class ReactViewEngine : IViewEngine
    {
        private static readonly ISet<string> _extensions = new HashSet<string>(new[] { "jsx" });

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Extensions
        {
            get { return _extensions; }
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
                    new NullCache(),
                    new NullFileSystem());

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

    }
}