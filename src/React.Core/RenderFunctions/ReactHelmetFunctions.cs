using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace React.RenderFunctions
{
	/// <summary>
	/// Render functions for React-Helmet. https://github.com/nfl/react-helmet
	/// Requires `react-helmet` to be exposed globally as `Helmet`
	/// </summary>
	public class ReactHelmetFunctions : RenderFunctionsBase
	{
		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		public ReactHelmetFunctions(IRenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}

		/// <summary>
		/// Dictionary of Helmet properties, rendered as raw HTML tags
		/// Available keys: "base", "bodyAttributes", "htmlAttributes", "link", "meta", "noscript", "script", "style", "title"
		/// </summary>
		public Dictionary<string, string> RenderedHelmet { get; private set; }

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			var helmetString = executeJs(@"
var helmetResult = Helmet.default.renderStatic();
JSON.stringify(['base', 'bodyAttributes', 'htmlAttributes', 'link', 'meta', 'noscript', 'script', 'style', 'title']
	.reduce((mappedResults, helmetKey) => Object.assign(mappedResults, { [helmetKey]: helmetResult[helmetKey] && helmetResult[helmetKey].toString() }), {}));");

			RenderedHelmet = JsonConvert.DeserializeObject<Dictionary<string, string>>(helmetString);
		}
	}
}
