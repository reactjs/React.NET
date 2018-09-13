using System;

namespace React.RenderFunctions
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRenderFunctions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		void PreRender(Func<string, string> executeJs);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		string TransformRender(string componentToRender);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		void PostRender(Func<string, string> executeJs);
	}
}
