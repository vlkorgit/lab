using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public partial class ViewModel
    {
        public ViewModel(WPFInterface realization)
        {
            WPFInterface = realization;
            InitializeCommands();
            InitializeProperties();
        }
    }
}
