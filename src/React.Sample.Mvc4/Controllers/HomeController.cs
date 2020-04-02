﻿/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

// For clarity, this sample has all code in the one file. In a real project, you'd put every
// class in a separate file.

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using React.Sample.Mvc4.Models;
using React.Sample.Mvc4.ViewModels;

namespace React.Sample.Mvc4.Models
{
	public class AuthorModel
	{
		public string Name { get; set; }
		public string GithubUsername { get; set; }
	}
	public class CommentModel
	{
		public AuthorModel Author { get; set; }
		public string Text { get; set; }
	}
}

namespace React.Sample.Mvc4.ViewModels
{
	public class IndexViewModel
	{
		public IEnumerable<CommentModel> Comments { get; set; }
		public int CommentsPerPage { get; set; }
		public int Page { get; set; }
	}
}

namespace React.Sample.Mvc4.Controllers
{
	public class HomeController : Controller
	{
		private const int COMMENTS_PER_PAGE = 3;

		private readonly IDictionary<string, AuthorModel> _authors;
		private readonly IList<CommentModel> _comments;

		public HomeController()
		{
			// In reality, you would use a repository or something for fetching data
			// For clarity, we'll just use a hard-coded list.
			_authors = new Dictionary<string, AuthorModel>
			{
				{"daniel", new AuthorModel { Name = "Daniel Lo Nigro", GithubUsername = "Daniel15" }},
				{"vjeux", new AuthorModel { Name = "Christopher Chedeau", GithubUsername = "vjeux" }},
				{"cpojer", new AuthorModel { Name = "Christoph Pojer", GithubUsername = "cpojer" }},
				{"jordwalke", new AuthorModel { Name = "Jordan Walke", GithubUsername = "jordwalke" }},
				{"zpao", new AuthorModel { Name = "Paul O'Shannessy", GithubUsername = "zpao" }},
			};
			_comments = new List<CommentModel>
			{
				new CommentModel { Author = _authors["daniel"], Text = "First!!!!111!" },
				new CommentModel { Author = _authors["zpao"], Text = "React is awesome!" },
				new CommentModel { Author = _authors["cpojer"], Text = "Awesome!" },
				new CommentModel { Author = _authors["vjeux"], Text = "Hello World" },
				new CommentModel { Author = _authors["daniel"], Text = "Foo" },
				new CommentModel { Author = _authors["daniel"], Text = "Bar" },
				new CommentModel { Author = _authors["daniel"], Text = "FooBarBaz" },
			};
		}

		public ActionResult Index()
		{
			return View(new IndexViewModel
			{
				Comments = _comments.Take(COMMENTS_PER_PAGE),
				CommentsPerPage = COMMENTS_PER_PAGE,
				Page = 1
			});
		}

		[OutputCache(Duration = 0, Location = OutputCacheLocation.Any, VaryByHeader = "Content-Type")]
		public ActionResult Comments(int page)
		{
			Response.Cache.SetOmitVaryStar(true);
			var comments = _comments.Skip((page - 1) * COMMENTS_PER_PAGE).Take(COMMENTS_PER_PAGE);
			var hasMore = page * COMMENTS_PER_PAGE < _comments.Count;

			if (ControllerContext.HttpContext.Request.ContentType == "application/json")
			{
				return Json(new
				{
					comments = comments,
					hasMore = hasMore
				}, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return View("Index", new IndexViewModel
				{
					Comments = _comments.Take(COMMENTS_PER_PAGE * page), 
					CommentsPerPage = COMMENTS_PER_PAGE,
					Page = page
				});
			}
		}
	}
}
