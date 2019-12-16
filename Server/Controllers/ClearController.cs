using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recognizer;
namespace Server.Controllers
{
    [Controller]
    [Route("/clear")]
    public class ClearController : Controller
    {
        private NumberRecognizer NumberRecognizer;
        public ClearController(INumberRecognizer numberRecognizer)
        {
            NumberRecognizer = numberRecognizer.NumberRecognizer;
        }
        [HttpGet]
        public void clear()
        {
            Console.WriteLine("clear");
            NumberRecognizer.ClearDB();
        }
    }
}