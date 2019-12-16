using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel
{
    public interface WPFInterface
    {
        void Dispatcher(Action action);
        void AddDescription(ICollection<NotifyTriple> collection,string path, string description, string probability);
        bool GetDirectory(out string Directory);
        ICommand FactoryMeCommand(Action<object> action, Predicate<object> predicate);
        //void AddElement(ICollection<NotifyTriple> Collection,string name, string description,string probability);
    }
}
