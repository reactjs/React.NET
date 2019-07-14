#if !NET452
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace React.Tests.Mvc
{
	public static class TestUtilities
	{
		public static string ToHtmlString(this IHtmlContent source)
		{
			using (var writer = new StringWriter())
			{
				source.WriteTo(writer, HtmlEncoder.Default);
				return writer.ToString();
			}
		}
	}
}

#endif
