using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Octokit;

namespace React.Site.Hooks
{
	/// <summary>
	/// Webhook for Netlify website builds
	/// </summary>
	public static class NetlifyHook
	{
		[FunctionName("NetlifyHook")]
		public static async Task<object> Run(
			[HttpTrigger(AuthorizationLevel.Function, WebHookType = "genericJson")]HttpRequestMessage req, 
			TraceWriter log)
		{
			log.Info("Netlify webhook triggered");

			string jsonContent = await req.Content.ReadAsStringAsync();
			var data = NetlifyHookBody.CreateFromRequest(jsonContent);

			if (!data.ReviewId.HasValue)
			{
				log.Info("Build is not for a PR. Ignoring.");
				return req.CreateResponse(HttpStatusCode.NoContent);
			}

			var client = new GitHubClient(new ProductHeaderValue("reactjs-net-webhooks"))
			{
				Credentials = new Credentials(ConfigurationManager.AppSettings["GitHubToken"])
			};

			// Determine if the PR modified any site files
			var githubOwner = ConfigurationManager.AppSettings["GitHubOwner"];
			var githubRepo = ConfigurationManager.AppSettings["GitHubRepo"];
			var pullRequestFiles = await client.PullRequest.Files(githubOwner, githubRepo, data.ReviewId.Value);
			if (!pullRequestFiles.Any(IsWebsiteFile))
			{
				log.Info("PR did not modify the site. Ignoring.");
				return req.CreateResponse(HttpStatusCode.NoContent);
			}

			var body = BuildCommentBody(data);

			// Check if a comment already exists
			var existingComments = await client.Issue.Comment.GetAllForIssue(githubOwner, githubRepo, data.ReviewId.Value);
			var existingComment = existingComments.FirstOrDefault(
				comment => comment.User.Login == ConfigurationManager.AppSettings["GitHubBotUser"]
			);
			if (existingComment != null)
			{
				// There's already an existing comment, so we'll just edit that one
				// rather than posting a brand new comment.
				await client.Issue.Comment.Update(githubOwner, githubRepo, existingComment.Id, body);
				log.Info($"Updated existing commment {existingComment.Id}");
			}
			else
			{
				await client.Issue.Comment.Create(githubOwner, githubRepo, data.ReviewId.Value, body);
				log.Info("Posted commment");
			}

			return req.CreateResponse(HttpStatusCode.NoContent);
		}

		private static bool IsWebsiteFile(PullRequestFile file)
		{
			return file.FileName.StartsWith("site/");
		}

		private static string BuildCommentBody(NetlifyHookBody data)
		{
			switch (data.State)
			{
				case "ready":
					return $"Website preview is ready!\nBuilt with commit {data.CommitRef}\n{data.DeploySslUrl}";

				case "error":
					var netlifySite = ConfigurationManager.AppSettings["NetlifySite"];
					var buildUrl = $"https://app.netlify.com/sites/{netlifySite}/deploys/{data.BuildId}";
					return $"Failed to build the site :(\n\n```\n{data.ErrorMessage}\n```\n\nBuild log: {buildUrl}";

				default:
					return $"Unknown build state: {data.State}\n{data.DeploySslUrl}";
			}
		}
	}
}
