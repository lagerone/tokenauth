using System.Web.Mvc;
using Lagerone.Web.Attributes;

namespace Lagerone.Web.Controllers
{
    [AuthorizeMvc]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
