using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class DelegateCommand : ICommand
    {

        Predicate<object> canExecute;
        Action<object> execute;
        private Action closeForm;

        public DelegateCommand(Predicate<object> _canexecute, Action<object> _execute)
            : this()
        {
            canExecute = _canexecute;
            execute = _execute;
        }

        public DelegateCommand()
        {

        }

        public DelegateCommand(Action closeForm)
        {
            this.closeForm = closeForm;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
