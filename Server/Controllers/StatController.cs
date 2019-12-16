using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recognizer;

namespace Server.Controllers
{
    [Controller]
    [Route("/stat")]
    public class StatController : Controller
    {
        private NumberRecognizer recognizer = null;
        public StatController(INumberRecognizer recognizer)
        {
            this.recognizer = recognizer.NumberRecognizer;
        }
        [HttpGet]
        public string GetStat()
        {
            Console.WriteLine("Get stat");
            return JsonConvert.SerializeObject(recognizer.GetStatisticsDB());
        }
    }
}