using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Recognizer;
using System.Threading;
using Server;

namespace ASPServer.Controllers
{
    [ApiController]
    [Route("/rec")]
    public class RecognizeController : Controller
    {
        private readonly NumberRecognizer recognizer;
        public RecognizeController(INumberRecognizer nm)
        {
            recognizer = nm.NumberRecognizer;
        }
        [HttpPost]
        public string PostMessage(API.ServerInput serverInput)
        {
            Console.WriteLine(serverInput.Filename);
            //byte[] bytes = Convert.FromBase64String(serverInput.Bytes);
            OutTriple out_triple;
            out_triple = recognizer.recognizeNumber(serverInput.Filename, serverInput.Bytes);
            if (out_triple == null)
            {
                Console.WriteLine("415 "+serverInput.Filename);
                Response.StatusCode = 415; //usupported media type
                return "Not correct";//null
            }
            Response.StatusCode = 200;
            var serveroutput = new API.ServerOutput() { Class = out_triple.Value, Filename = out_triple.Key, Probability = out_triple.Probability };
            return JsonConvert.SerializeObject(serveroutput);


            #region trash
            //if (Request.Form.Count != 1) return null;
            //OutTriple out_triple = null;
            //foreach (var item in Request.Form)
            //{
            //    str = item.Value;
            //    var serverinput = JsonConvert.DeserializeObject<API.ServerInput>(str);
            //    Console.WriteLine(serverinput.Filename);
            //    byte[] bytes = Convert.FromBase64String(serverinput.Bytes);
            //    try
            //    {
            //        out_triple = recognizer.recognizeNumber(serverinput.Filename, bytes);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        Console.WriteLine("MODEL EXCEPTION");
            //        return null;
            //    }
            //}
            //if (out_triple == null) throw new Exception();
            //var serveroutput = new API.ServerOutput() { Class = out_triple.Value, Filename = out_triple.Key, Probability = out_triple.Probability };
            //return JsonConvert.SerializeObject(serveroutput);
            #endregion
        }
    }
}
