using System;
using System.Windows.Input;

namespace WnekoPacMan.Commands
{
    public class CommandHandler : ICommand
    {
        private Action<object> action;
        private Func<object, bool> canExecute;

        public CommandHandler(Action<object> act, Func<object, bool> canExec)
        {
            action = act;
            canExecute = canExec;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        private void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            try
            {
                handler.Invoke(this, EventArgs.Empty);
            }
            catch (NullReferenceException)
            {
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }
    }
}
