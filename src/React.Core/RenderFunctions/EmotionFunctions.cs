using System;
using System.Text;

namespace React.RenderFunctions
{
	/// <summary>
	/// Render functions for Emotion. https://github.com/emotion-js/emotion
	/// Requires `emotion-server` to be exposed globally as `EmotionServer`
	/// </summary>
	public class EmotionFunctions : RenderFunctionsBase
	{
		/// <summary>
		/// Implementation of TransformRenderedHtml
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public override string TransformRenderedHtml(string input)
		{
			return $"EmotionServer.renderStylesToString({input})";
		}
	}
}
