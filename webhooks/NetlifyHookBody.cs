using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace React.Site.Hooks
{
	/// <summary>
	/// Body of Netlify webhook requests
	/// </summary>
	public class NetlifyHookBody
	{
		public string Id { get; set; }
		public string SiteId { get; set; }
		public string BuildId { get; set; }
		public string State { get; set; }
		public string Name { get; set; }
		public Uri Url { get; set; }
		public Uri SslUrl { get; set; }
		public Uri AdminUrl { get; set; }
		public Uri DeployUrl { get; set; }
		public Uri DeploySslUrl { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string UserId { get; set; }
		public string ErrorMessage { get; set; }
		public string CommitRef { get; set; }
		public int? ReviewId { get; set; }
		public string Branch { get; set; }
		public Uri CommitUrl { get; set; }
		public string Title { get; set; }
		public int? DeployTime { get; set; }

		public static NetlifyHookBody CreateFromRequest(string request)
		{
			return JsonConvert.DeserializeObject<NetlifyHookBody>(request, new JsonSerializerSettings
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				}
			});
		}
	}
}
