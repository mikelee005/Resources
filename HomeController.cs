using adr.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using adr.Web.Services;
using adr.Web.Models.ViewModels;
// ********** FYI: System Generated File

namespace adr.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        [AllowAnonymous]
        public ActionResult Contact()
        {

            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ConversationTest()
        {
            ViewBag.Message = "Your messages/conversations page.";

            return View();
        }
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            ViewBag.Message = "Your login page.";

            return View();
        }

        public ActionResult FormExample()
        {
            ViewBag.Message = "Your Form Example page.";

            return View();
        }

        public ActionResult SliderExample()
        {
            ViewBag.Message = "Your Slider Example page.";

            return View();
        }

        public ActionResult AdminPage()
        {
            ViewBag.Message = "Your Admin page.";

            return View();
        }


        public ActionResult Account()
        {
            ViewBag.Message = "Edit your profile.";

            return View();
        }

        public ActionResult Timeline()
        {
            ViewBag.Message = "Your Timeline page.";

            return View();
        }

        public ActionResult CompanyProfile()
        {
            ViewBag.Message = "Create Company Profile.";

            BaseViewModel model = new BaseViewModel();

            return View("CompanyProfileNG", model);
        }

        public ActionResult References()
        {
            ViewBag.Message = "Your Reference page.";

            return View("ReferencesNG");
        }

        public ActionResult MapUtility()
        {
            ViewBag.Message = "Map Utility page.";

            return View("MapUtilityNG");
        }

        public ActionResult HomeyPage()
        {
            ViewBag.Message = "Homey Page page.";

            return View("");
        }

        public ActionResult Notification()
        {
            ViewBag.Message = "Your Notification page.";

            BaseViewModel vm = new BaseViewModel();
            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult ProductMarketPlace()
        {
            ViewBag.Message = "Your Materials Search page.";


            return View();
        }

        public ActionResult Chat()
        {
            ViewBag.Message = "Your Chat page.";

            return View();
        }


    }
}