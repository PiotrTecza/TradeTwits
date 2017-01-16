using System;
using System.Web.Mvc;

namespace TradeTwits.JS.Test.Controllers
{
    public class JasmineController : Controller
    {
        public ViewResult Run()
        {
            return View("SpecRunner");
        }
    }
}
