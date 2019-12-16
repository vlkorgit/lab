using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WPF
{
    public class Command : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;
        public Command() { }

        public Command(Action<object> action, Predicate<object> predicate)
        {
            _canExecute = predicate;
            _execute = action;
            CanExecuteChanged += (obj,e) => CommandManager.InvalidateRequerySuggested();
        }
        public void setCanExecute(Predicate<object> obj)
        {
            _canExecute = obj;
        }
        public void setExecute(Action<object> obj)
        {
            _execute = obj;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
