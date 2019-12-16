using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recognizer;
namespace Server
{
    public interface INumberRecognizer
    {
        NumberRecognizer NumberRecognizer { get; }
    }
    public class SingletoneRecognizer : INumberRecognizer
    {
        public SingletoneRecognizer()
        {
            NumberRecognizer = new NumberRecognizer();
        }
        public NumberRecognizer NumberRecognizer
        {
            get;
        }
    }
}
