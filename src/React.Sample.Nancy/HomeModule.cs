using Nancy;

namespace React.Sample.Nancy
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => View["home"];

            Post["/"] = _ => Response.AsRedirect("~/hi?name=" + (string)Request.Form.name.Value);

            Get["/hi"] = _ => {
                if (Request.Query.name.HasValue)
                    return View["hi", new { name = Request.Query.name.Value }];
                return Response.AsRedirect("/");
            };
        }
    }
}