using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatAppSOLID.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;          // The action to run
        private readonly Func<bool> _canExecute;   // The condition to check if it can run

        // Constructor takes both the action and the condition
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;  // can be null if no condition is needed
        }

        // This event tells the UI when the command's availability changes
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //
        // s if the command can run based on the condition
        public bool CanExecute(object parameter)
        {
            // If no condition was given, say yes
            // Otherwise, check the condition
            return _canExecute == null || _canExecute();
        }

        // Runs the stored action
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
