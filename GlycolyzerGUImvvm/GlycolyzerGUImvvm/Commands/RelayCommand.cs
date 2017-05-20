using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GlycolyzerGUImvvm.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object> action_execute;

        public RelayCommand(Action<object> action)
        {
            action_execute = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            action_execute(parameter);
        }
        #endregion
    }
}
