using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class Provider
    {
        AutoResetEvent are = new AutoResetEvent(true);
        public void Clear()
        {
            are.WaitOne();
            using (DatabaseContext context = new DatabaseContext())
            {
                context.Pictures.RemoveRange(context.Pictures);
                context.Files.RemoveRange(context.Files);
                context.SaveChanges();
            }
            are.Set();
        }
        public void Add(string filename, int hash, int number, float probability, byte[] bytes)
        {
            are.WaitOne();
            using (DatabaseContext context = new DatabaseContext())
            {
                Picture blob = new Picture() { Bytes = bytes };
                File file = new File() { Name = filename, Hash = hash, Number = number, Probability = probability, Statistics = 0 , Blob=blob};
                context.Files.Add(file);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("add " + file.Name);
            }
            are.Set();
        }
        public File GetFileIfExist(int hash, byte[] bytes,string filename)
        {
            are.WaitOne();
            File res = null;
            using (DatabaseContext context = new DatabaseContext())
            {
                var buf2 = Path.GetFileName(filename);
                var result = context.Files.Where((obj) => obj.Hash == hash).Include(b=>b.Blob).ToArray();
                if (result != null)
                    foreach (var r in result)
                    {
                        //доп задание сравниваем по укороченному имени файла
                        var buf1 = Path.GetFileName(r.Name);
                        if (buf1 != buf2) continue; //если попали по хэшу то сравниваем имена файлов
                        bool f = true;
                        if (!bytes.SequenceEqual(r.Blob.Bytes))
                        {
                            f = false;
                        }
                        if (f)
                        {
                            r.Statistics++; //если файл найден, то увеличиваем счетчик
                            context.SaveChanges();
                            res = r;
                            break;
                        }
                    }
            }
            are.Set();
            return res;
        }
        public List<KeyValuePair<string, int>> GetStatistics()
        {
            are.WaitOne();
            List<KeyValuePair<string, int>> lst = new List<KeyValuePair<string, int>>();
            using (DatabaseContext context = new DatabaseContext())
            {
                foreach (var file in context.Files)
                {
                    lst.Add(new KeyValuePair<string, int>(file.Name, file.Statistics));
                }
            }
            are.Set();
            return lst;
        }
    }
}
