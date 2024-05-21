using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inchoqate.GUI.Titlebar
{
    public class ActionButtonCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action _action;

        public ActionButtonCommand(Action action)
        {
            _action = action;
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return true;
        }

        void ICommand.Execute(object? parameter)
        {
            _action();
        }
    }
}
