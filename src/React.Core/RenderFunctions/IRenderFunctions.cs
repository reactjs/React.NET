using System;

namespace React.RenderFunctions
{
	public interface IRenderFunctions
	{
		void PreRender(Func<string, string> executeJs);
		string TransformRender(string componentToRender);
		void PostRender(Func<string, string> executeJs);
	}
}
