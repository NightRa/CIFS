using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CifsStartupApp.IconHandling
{
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; }
        public Func<bool> CanExecuteFunc { get; }

        public DelegateCommand(Action commandAction, Func<bool> canExecuteFunc)
        {
            CommandAction = commandAction;
            CanExecuteFunc = canExecuteFunc;
        }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
