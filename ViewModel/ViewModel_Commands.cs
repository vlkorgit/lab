using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Database;
using System.Net.Http.Headers;

namespace ViewModel
{
    public partial class ViewModel
    {
        public ICommand Open { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Start { get; set; }
        public ICommand GetStat { get; set; }
        public ICommand Clear { get; set; }
        public ICommand ClearDiag { get; set; }
        public void InitializeCommands()
        {
            Open = WPFInterface.FactoryMeCommand(Open_Click, _ => true);
            Stop = WPFInterface.FactoryMeCommand(Stop_Click, Stop_Can);
            Start = WPFInterface.FactoryMeCommand(Start_Click, Start_Can);
            GetStat = WPFInterface.FactoryMeCommand(GetStat_Click, GetStat_Can);
            Clear = WPFInterface.FactoryMeCommand(Clear_Click, Clear_Can);
            ClearDiag = WPFInterface.FactoryMeCommand(ClearDiag_Click, (obj) => Diag.Count > 0 ? true : false);
        }

        private void ClearDiag_Click(object obj)
        {
            Diag.Clear();
        }
        #region handlers
        private void Open_Click(object obj)
        {
            string buf;
            if (WPFInterface.GetDirectory(out buf))
            {
                Directory_Path = buf;
                DirectoryInfo directoryInfo = new DirectoryInfo(Directory_Path);
                Items.Clear();
                foreach (var file in directoryInfo.GetFiles())
                {
                    Items.Add(new NotifyTriple(file.FullName, "N/A", "N/A"));
                }
            }
        }
        private bool Start_Can(object obj)
        {
            if (Directory_Path == null) return false;
            if (isDb || isRec) return false;
            return true;
        }
        private void Start_Click(object obj)
        {
            if (Directory_Path == null) return;
            Task.Factory.StartNew(() =>
            {
                isRec = true;
                //шлем по одной картинке, ибо если слать пачку, то обновить не сможем, пока всю пачку не получим
                using (HttpClient client = new HttpClient())
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Directory_Path);
                    var files = directoryInfo.GetFiles();
                    Task[] tasks = new Task[files.Length];
                    for (int i = 0; i < files.Length; i++)
                    {
                        byte[] bytes = null;
                        try
                        {
                            bytes = System.IO.File.ReadAllBytes(files[i].FullName);
                        }
                        catch
                        {
                            WPFInterface.Dispatcher(() => Diag.Add("Cannot read file " + files[i].FullName));
                            Debug.WriteLine("Cannot read file " + files[i].FullName);
                            continue;
                        }
                        //string stringbytes = Convert.ToBase64String(bytes);
                        API.ServerInput serverinput = new API.ServerInput { Filename = files[i].FullName, Bytes = bytes };


                        string JSONserverinput = JsonConvert.SerializeObject(serverinput);
                        var httpcontent = new StringContent(JSONserverinput);
                        httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        tasks[i] = addAsync("http://localhost:5000/rec", httpcontent, client, files[i].FullName);



                    }
                    Task.WaitAll(tasks);
                }
                Debug.WriteLine("RECOGNIZE END");
                isRec = false;
            });
        }
        private async Task addAsync(string url, HttpContent content, HttpClient client, string filename)
        {
            Debug.WriteLine("rec " + filename);
            foreach (var item in Items)
            {
                if (item.Key == filename)
                {
                    string str = null;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = await client.PostAsync(url, content);
                    }
                    catch
                    {
                        WPFInterface.Dispatcher(() => Diag.Add("SERVER DOESN'T RESPONSE " + filename));
                        return;
                    }
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        WPFInterface.Dispatcher(() => Diag.Add("NOT CORRECT " + filename));
                        return;
                    }
                    str = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<API.ServerOutput>(str);
                    WPFInterface.Dispatcher(() =>
                    {

                        item.Value = result.Class;
                        item.Probability = result.Probability;
                    });
                }
            }
            Debug.WriteLine("end " + filename);
        }
        private bool Stop_Can(object obj)
        {
            if (isRec) return true;
            return false;
        }
        private void Stop_Click(object obj)
        {
            if (cts != null)
                cts.Cancel();
        }
        private bool GetStat_Can(object obj)
        {
            if (isDb || isRec) return false;
            return true;
        }
        private void GetStat_Click(object obj)
        {
            Task.Factory.StartNew(async () =>
           {
               isDb = true;
               using (HttpClient client = new HttpClient())
               {
                   HttpResponseMessage response = null;
                   API.ServerStat serverstat = null;
                   try
                   {
                       response = await client.GetAsync("http://localhost:5000/stat");
                       string str = await response.Content.ReadAsStringAsync();
                       serverstat = JsonConvert.DeserializeObject<API.ServerStat>(str);
                       WPFInterface.Dispatcher(() => Statistics = serverstat);
                   }
                   catch
                   {
                       WPFInterface.Dispatcher(() => Diag.Add("GET STAT - SERVER DOESN'T RESPONSE"));
                   }
               }
               isDb = false;
           });
        }
        private bool Clear_Can(object obj)
        {
            if (isRec || isDb) return false;
            return true;
        }
        private void Clear_Click(object obj)
        {
            Task.Factory.StartNew(async () =>
            {
                isDb = true;
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = null;
                    try
                    {
                        response = await client.GetAsync("http://localhost:5000/clear");
                        WPFInterface.Dispatcher(() => Statistics = null);
                    }
                    catch
                    {
                        WPFInterface.Dispatcher(() => Diag.Add("CLEAR - SERVER DOESN'T RESPONSE"));
                    }
                }
                isDb = false;
            });
        }
        #endregion
    }
}
