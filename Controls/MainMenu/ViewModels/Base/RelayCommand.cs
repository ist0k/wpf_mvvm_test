using System;
using System.Windows.Input;

namespace WinForm.Controls.MainMenu.ViewModels.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private ICommand cardClickCommand;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Конструктор для команд с параметрами и проверкой доступности
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Дополнительный легкий конструктор для простых команд без параметров () => {}
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = _ => execute();
            if (canExecute != null) _canExecute = _ => canExecute();
        }

        public RelayCommand(ICommand cardClickCommand)
        {
            this.cardClickCommand = cardClickCommand;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
