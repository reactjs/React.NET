using System;
using System.Text;

namespace React.StylesheetHelpers
{
	/// <summary>
	/// Render functions for Emotion. https://github.com/emotion-js/emotion
	/// Requires `emotion-server` to be exposed globally as `EmotionServer`
	/// </summary>
	public class EmotionFunctions : RenderFunctions
	{
		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		public EmotionFunctions(RenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}

		/// <summary>
		/// Implementation of TransformRenderedHtml
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		protected override string TransformRenderedHtmlCore(string input)
		{
			return $"EmotionServer.renderStylesToString({input})";
		}
	}
}
