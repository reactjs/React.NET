/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.IO;

#if LEGACYASPNET
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
		/// <summary>Returns an HTML-encoded string.</summary>
		/// <returns>An HTML-encoded string.</returns>
		public string ToHtmlString()
		{
			var sw = new StringWriter();
			_textWriter(sw);
			return sw.ToString();
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
