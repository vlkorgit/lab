using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ViewModel
{
    public partial class ViewModel: INotifyPropertyChanged
    {
        private WPFInterface WPFInterface;
        CancellationTokenSource cts = null;
        public ObservableCollection<string> Diag { get; set; }
        public ObservableCollection<NotifyTriple> Items { get; set; }
        string directory_path = null;
        string Directory_Path
        {
            get
            {
                return directory_path;
            }
            set
            {
                directory_path = value;
                OnPropertyChanged("Directory_Path");
            }
        }
        //bool correctDirectoryFlag = false;
        void InitializeProperties()
        {
            Items = new ObservableCollection<NotifyTriple>();
            Diag = new ObservableCollection<string>();
            Directory_Path = null;
        }
        private List<KeyValuePair<string,int>> statistics;
        public List<KeyValuePair<string,int>> Statistics { get
            {
                return statistics;
            }
            set
            {
                statistics = value;
                OnPropertyChanged("Statistics");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        private bool isDb = false;
        private bool isRec = false;
    }
}
