/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

#if LEGACYASPNET
using HttpResponse = System.Web.HttpResponseBase;
#else
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace React.Router
{
	/// <summary>
	/// Helper class that takes the values in the <see cref="RoutingContext"/>
	/// to modify the servers response. 
	/// F.x. return an http status code of 404 not found
	/// or redirect the client to a new URL.
	/// </summary>
	public static class SetServerResponse
	{
		/// <summary>
		/// Uses the values in the <see cref="RoutingContext"/> to modify
		/// the servers response. 
		/// F.x. return an http status code of 404 not found
		/// or redirect the client to a new URL.
		/// </summary>
		/// <param name="context">
		/// The routing context returned by React Router.
		/// It contains new values for the server response.
		/// </param>
		/// <param name="Response">The response object to use.</param>
		public static void ModifyResponse(RoutingContext context, HttpResponse Response)
		{
			var statusCode = context.status ?? 302;

			// 300-399
			if (statusCode >= 300 && statusCode < 400)
			{
				if (!string.IsNullOrEmpty(context.url))
				{
					if (statusCode == 301)
					{
#if LEGACYASPNET
						Response.RedirectPermanent(context.url);
#else
						Response.Redirect(context.url, true);
#endif
					}
					else // 302 and all others
					{
						Response.Redirect(context.url);
					}
				}
				else
				{
					throw new ReactRouterException("Router requested redirect but no url provided.");
				}
			}
			else
			{
				Response.StatusCode = statusCode;
			}
		}
	}
}
