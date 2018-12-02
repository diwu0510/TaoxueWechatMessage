using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HZC.Core;
using Microsoft.AspNetCore.Mvc;
using Taoxue.Mp.Sms.Website.Extensions;
using Taoxue.Mp.Sms.Website.Models;

namespace Taoxue.Mp.Sms.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWxNoticeService _ns;

        public HomeController(IWxNoticeService ns)
        {
            _ns = ns;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(int id, string client)
        {
            _ns.Add($"{client}：{id}");
            return Json(ResultUtil.Success());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
