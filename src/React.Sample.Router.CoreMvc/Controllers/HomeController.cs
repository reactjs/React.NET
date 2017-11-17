using Microsoft.AspNetCore.Mvc;

namespace React.Sample.Router.CoreMvc.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
