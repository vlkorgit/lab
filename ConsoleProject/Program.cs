using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace ConsoleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                using (var client = new HttpClient())
                {
                    string enter = Console.ReadLine();
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(enter));
                    var answer = client.PostAsync("http://localhost:5000/home", content).Result;
                    Console.WriteLine( answer.Content.ReadAsStringAsync().Result);
                    //string str = answer.RequestMessage.Content.ReadAsStringAsync().Result;
                    //a = JsonConvert.DeserializeObject<API.Message>(answer.Content.ReadAsStringAsync().Result);
                    //answer.Content.
                    IEnumerable<string> lst = new List<string>();
                    answer.Content.Headers.TryGetValues("GOVNO",out lst);
                    if (lst == null) Console.WriteLine("NULL");
                    else foreach(var pair in lst)
                        {
                            Console.WriteLine(pair);
                        }
                    foreach (var pair in answer.Content.Headers)
                    {
                        Console.WriteLine("PAIR");
                        Console.WriteLine(pair.Key);
                        foreach (var str in pair.Value)
                        {
                            Console.WriteLine(str);
                        }
                    }
                    Console.WriteLine(answer.Content);
                }
            }
        }
    }
}
