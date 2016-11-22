using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RockPaperScissorsApi.Controllers
{
    /// <summary>
    /// <c>HomeController</c> is the default controller and show general information about the project.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// <c>Index</c> redirect to the default view.
        /// </summary>
        /// <returns>Default view</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
