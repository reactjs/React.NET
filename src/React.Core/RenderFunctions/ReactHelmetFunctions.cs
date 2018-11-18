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
		/// Dictionary of Helmet properties, rendered as raw HTML tags
		/// Available keys: "base", "bodyAttributes", "htmlAttributes", "link", "meta", "noscript", "script", "style", "title"
		/// </summary>
		public Dictionary<string, string> RenderedHelmet { get; private set; }

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PostRender(Func<string, string> executeJs)
		{
			var helmetString = executeJs(@"
var helmetResult = Helmet.renderStatic();
JSON.stringify(['base', 'bodyAttributes', 'htmlAttributes', 'link', 'meta', 'noscript', 'script', 'style', 'title']
	.reduce((mappedResults, helmetKey) => Object.assign(mappedResults, { [helmetKey]: helmetResult[helmetKey] && helmetResult[helmetKey].toString() }), {}));");

			RenderedHelmet = JsonConvert.DeserializeObject<Dictionary<string, string>>(helmetString);
		}
	}
}
