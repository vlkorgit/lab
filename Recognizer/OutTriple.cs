using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognizer
{
    public class OutTriple 
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Probability { get; set; }
        public OutTriple(string key, string value,string probability)
        {
            Key = key;
            Value = value;
            Probability = probability;
        }
    }
}
