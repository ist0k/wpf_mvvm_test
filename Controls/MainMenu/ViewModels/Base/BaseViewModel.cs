using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinForm.Controls.MainMenu.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Стандартный метод вызова события
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Крутой метод-помощник для сеттеров
        // Он сам проверяет, изменилось ли значение, обновляет его и пинает интерфейс
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
