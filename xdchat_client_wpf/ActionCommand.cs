using System;
using System.Windows;
using System.Windows.Input;

namespace xdchat_client_wpf {
    public class ActionCommand : ICommand {
        public event EventHandler CanExecuteChanged;

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action execute, Func<bool> canExecute = null) {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) {
            return _canExecute();
        }

        public void Execute(object parameter) {
            _execute();
        }

        public void RaiseCanExecuteChanged() {
            if (CanExecuteChanged != null)
                Application.Current.Dispatcher?.BeginInvoke((Action<ActionCommand, EventArgs>) CanExecuteChanged.Invoke,
                    this, EventArgs.Empty);
        }
    }
}