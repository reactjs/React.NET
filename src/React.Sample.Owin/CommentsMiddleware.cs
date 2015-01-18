/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Owin;

using Newtonsoft.Json;

using React.Sample.Owin.Models;

namespace React.Sample.Owin.Models
{
    public class AuthorModel
    {
        public string Name { get; set; }
        public string Facebook { get; set; }
    }
    public class CommentModel
    {
        public AuthorModel Author { get; set; }
        public string Text { get; set; }
    }
}

namespace React.Sample.Owin
{
    internal class CommentsMiddleware
    {
        private const int COMMENTS_PER_PAGE = 3; 
        
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly List<CommentModel> _comments;

        public CommentsMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;

            // In reality, you would use a repository or something for fetching data
            // For clarity, we'll just use a hard-coded list.
            var authors = new Dictionary<string, AuthorModel>
            {
                {"daniel", new AuthorModel { Name = "Daniel Lo Nigro", Facebook = "daaniel" }},
                {"vjeux", new AuthorModel { Name = "Christopher Chedeau", Facebook = "vjeux" }},
                {"cpojer", new AuthorModel { Name = "Christoph Pojer", Facebook = "cpojer" }},
                {"jordwalke", new AuthorModel { Name = "Jordan Walke", Facebook = "jordwalke" }},
                {"zpao", new AuthorModel { Name = "Paul O'Shannessy", Facebook = "zpao" }},
            };

            _comments = new List<CommentModel>
            {
                new CommentModel { Author = authors["daniel"], Text = "First!!!!111!" },
                new CommentModel { Author = authors["zpao"], Text = "React is awesome!" },
                new CommentModel { Author = authors["cpojer"], Text = "Awesome!" },
                new CommentModel { Author = authors["vjeux"], Text = "Hello World" },
                new CommentModel { Author = authors["daniel"], Text = "Foo" },
                new CommentModel { Author = authors["daniel"], Text = "Bar" },
                new CommentModel { Author = authors["daniel"], Text = "FooBarBaz" },
            };
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            // Determine if this middleware should handle the request
            if (!context.Request.Path.Value.StartsWith("/comments/page-") || context.Request.Method != "GET")
            {
                await _next(environment);
                return;
            }

            // prepare the response data
            int page = int.Parse(context.Request.Path.Value.Replace("/comments/page-", string.Empty));
            var responseObject = new
            {
                comments = _comments.Skip((page - 1) * COMMENTS_PER_PAGE).Take(COMMENTS_PER_PAGE),
                hasMore = page * COMMENTS_PER_PAGE < _comments.Count
            };

            var json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(responseObject));

            await context.Response.WriteAsync(json);
        }
    }
}