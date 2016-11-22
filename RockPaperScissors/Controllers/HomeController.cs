using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RockPaperScissors.Controllers
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
            return View();
        }

        /// <summary>
        /// <c>About</c> display information related with this solution.
        /// </summary>
        /// <returns>About view</returns>
        public ActionResult About()
        {
            ViewBag.Description = "This solution create a Rock-Paper-Scissors game using Restfull Services."
                + " I implement Microsoft .Net Technologies because it is a great bet for the future."
                + " The facilities, good practices and standars makes code with .Net a fantastic experience."
                + " The design MVC is in trend, object-oriented programming and the available libraries did"
                + " me easy choose the technology.";

            ViewBag.Solution = "The implement solution use two projects: Web API and MVC project. On Web API"
                + " I implement my knowlegde about Entity Framework 6. The MVC project is not connect to the database,"
                + " so, I create an extra web service for clean up the database and start over.";

            ViewBag.Me = "And finally, I work since February like Frontend developer on ObieCRE."
                + " I have nice design skills but really my passion is create new and awesome solutions."
                + " My short-term goal is to focus me on Microsoft Technologies, this is part of the"
                + " reason that led me to choose this technology. \nThis is the first time I use MVC and Web API!" 
                + " I tried to do it on the best way!";

            return View();
        }
    }
}