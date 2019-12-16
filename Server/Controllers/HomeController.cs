using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Controllers
{
    [Controller]
    [Route("/home")]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        [HttpPost]
        public HttpResponseMessage Post()
        {
            return null;
        }
    }
}
