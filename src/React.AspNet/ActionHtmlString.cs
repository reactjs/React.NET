/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.IO;

#if LEGACYASPNET
using System.Text;
using System.Web;
#else
using System.Text.Encodings.Web;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

#if LEGACYASPNET
namespace React.Web.Mvc
#else
namespace React.AspNet
#endif
{
	/// <summary>
	/// IHtmlString or IHtmlString action wrapper implementation
	/// </summary>
	public class ActionHtmlString : IHtmlString
	{
		private readonly Action<TextWriter> _textWriter;

		/// <summary>
		/// Constructor IHtmlString or IHtmlString action wrapper implementation
		/// </summary>
		/// <param name="textWriter"></param>
		public ActionHtmlString(Action<TextWriter> textWriter)
		{
			_textWriter = textWriter;
		}

#if LEGACYASPNET
		[ThreadStatic]
		private static StringWriter _sharedStringWriter;

		/// <summary>Returns an HTML-encoded string.</summary>
		/// <returns>An HTML-encoded string.</returns>
		public string ToHtmlString()
		{
			var stringWriter = _sharedStringWriter;
			if (stringWriter != null)
			{
				stringWriter.GetStringBuilder().Clear();
			}
			else
			{
				_sharedStringWriter = stringWriter = new StringWriter(new StringBuilder(128));
			}

			_textWriter(stringWriter);

			return stringWriter.ToString();
		}
#else
		/// <summary>
		/// Writes the content by encoding it with the specified <paramref name="encoder" />
		/// to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written.</param>
		/// <param name="encoder">The <see cref="T:System.Text.Encodings.Web.HtmlEncoder" /> which encodes the content to be written.</param>
		public void WriteTo(TextWriter writer, HtmlEncoder encoder)
		{
			_textWriter(writer);
		}
#endif
	}
}
