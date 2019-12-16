using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ViewModel;

namespace WPF
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.ViewModel(new Methods(Dispatcher));
        }

    }
    public class Methods : WPFInterface
    {
        Dispatcher Dispatcher;
        public Methods(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
        public void AddDescription(ICollection<NotifyTriple> Collection, string name, string description, string probability)
        {
            foreach (var elem in Collection)
                if (elem.Key == name)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        elem.Value = description;
                        elem.Probability = probability;
                    }));
                }

        }

        public ICommand FactoryMeCommand(Action<object> action, Predicate<object> predicate)
        {
            return new Command(action, predicate);
        }

        public bool GetDirectory(out string Directory)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Directory = dialog.SelectedPath;
                    return true;
                }
                else
                {
                    Directory = null;
                    return false;
                }
            }
        }

        void WPFInterface.Dispatcher(Action action)
        {
            Dispatcher.BeginInvoke(action).Wait();
        }
    }

    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int[] arr = (int[])value;
            List<KeyValuePair<int, int>> lst = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < 10; i++)
            {
                if (arr[i] > 0) lst.Add(new KeyValuePair<int, int>(i, arr[i]));
            }
            return lst;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
