using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class NotifyTriple : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string key;
        private string value;
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Key"));
            }
        }
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }
        private string probability;
        public  string Probability
        {
            get
            {
                return probability;
            }
            set
            {
                var a = value.Split('.',',');
                if (a.Length > 1)
                    probability = a[0] + "," + a[1].Substring(0, 2) + "%";
                else
                    probability = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Probability"));
            }
        }
        public NotifyTriple(string key, string value,string probability)
        {
            Key = key;
            Value = value;
            Probability = probability;
        }

    }
}
