using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using Database;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Recognizer
{

    public class NumberRecognizer
    {
        InferenceSession session = new InferenceSession("model.onnx");
        AutoResetEvent are = new AutoResetEvent(true);
        Provider provider = new Provider();
        public NumberRecognizer()
        {
            
            Console.WriteLine("Recognizer initialize");
        }
        public OutTriple recognizeNumber(string image_path, byte[] bytes)
        {
            try
            {
                //Console.WriteLine("recognize number");
                //if (session == null) Console.WriteLine("SESSION == NULL");
                int hash; //вычислим хэш
                hash = new BigInteger(bytes).GetHashCode();
                //если два потока будут обращаться к бд, может возникнуть рассинхронизация
                //связанная с тем, что если один поток будет распознавать, а другой читать из бд
                //то может получиться так, что второй поток может иметь зависимость от результатов первого
                //параллельная программа должна вести себя как последовательная
                are.WaitOne();
                Database.File result = provider.GetFileIfExist(hash, bytes, image_path);
                if (result != null)
                {
                    are.Set();
                    return new OutTriple(image_path, result.Number.ToString(), result.Probability.ToString());
                }
                var insert_lst = new List<NamedOnnxValue>();
                //метаданные нейросети
                var metaData = session.InputMetadata;
                //массив входных данных нейросети
                float[] inputData;
                inputData = getImageFloat(bytes);
                if (inputData == null)
                {
                    are.Set();
                    return null;
                }
                //загружаем данные в список для нейросети
                foreach (var name in metaData.Keys)
                {
                    var tensor = new DenseTensor<float>(inputData, metaData[name].Dimensions);
                    insert_lst.Add(NamedOnnxValue.CreateFromTensor<float>(name, tensor));
                }
                float max_flt = 0;
                int max_num = 0;
                int j = 0;
                OutTriple triple = null;
                //запускаем нейросеть
                using (var results = session.Run(insert_lst))
                {
                    //вычисляем номер максимальной вероятности
                    foreach (var res in results)
                    {
                        foreach (var flt in res.AsEnumerable<float>())
                        {
                            if (flt > max_flt)
                            {
                                max_flt = flt;
                                max_num = j;
                            }
                            j++;
                        }
                        provider.Add(image_path, hash, max_num, max_flt, bytes);
                        //азсылаем номер в очередь
                        triple = new OutTriple(image_path, max_num.ToString(), max_flt.ToString());
                    }

                }
                are.Set();
                // Console.WriteLine("recognize end");
                return triple;
            }
            catch
            {
                are.Set();
                return null;
            }
        }

        //парсинг картинки
        private static float[] getImageFloat(byte[] bytes)
        {
            const int dim = 28 * 28;
            float[] gray_scale = new float[] { 0.299f, 0.587f, 0.114f };
            float[] ret = new float[dim];
            byte[] bytes_array;
            int head_shift;
            MemoryStream ms = new MemoryStream(bytes);
            Image image = new Bitmap(ms);
            image = new Bitmap(image, new Size(28, 28));
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bytes_array = ms.ToArray();
            head_shift = bytes_array.Length - ret.Length * 4;
            for (int i = 0; i < dim; i++)
            {
                ret[i] =
                       (float)bytes_array[i * 4 + 2 + head_shift] / 255f * gray_scale[0] +
                       (float)bytes_array[i * 4 + 1 + head_shift] / 255f * gray_scale[1] +
                       (float)bytes_array[i * 4 + head_shift] / 255f * gray_scale[2];
                ret[i] = 1 - ret[i];
            }
            //закрываем поток записи
            ms.Close();
            //очистка потока
            if (ms != null) ms.Dispose();
            //возврат преобразованных данных
            return ret;
        }
        public void ClearDB()
        {
            provider.Clear();
        }
        public List<KeyValuePair<string, int>> GetStatisticsDB()
        {
            return provider.GetStatistics();
        }
    }
}