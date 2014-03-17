namespace React.Sample.Mvc4
{
	public class ReactConfig : IReactSiteInitializer
	{
		public void Configure(IReactSiteConfiguration config)
		{
			config
				.AddScript("~/Scripts/HelloWorld.react.js")
				.AddScript("~/Scripts/HelloWorld2.react.jsx");
		}
	}
}