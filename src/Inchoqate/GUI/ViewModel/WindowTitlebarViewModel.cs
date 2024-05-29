

using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using System.Windows;
using System.Windows.Input;

namespace Inchoqate.GUI.ViewModel
{
    public class WindowTitlebarViewModel : BaseViewModel
    {
        private readonly RelayCommand _minimizeCommand, _maximizeCommand, _closeCommand;

        private Window? _window;


        public event EventHandler? MinimizeClick, MaximizeClick, CloseClick;

        public ICommand MinimizeCommand => _minimizeCommand;
        public ICommand MaximizeCommand => _maximizeCommand;
        public ICommand CloseCommand => _closeCommand;


        public WindowTitlebarViewModel()
        {
            _minimizeCommand = new RelayCommand(OnMinimize);
            _maximizeCommand = new RelayCommand(OnMaximize);
            _closeCommand = new RelayCommand(OnClose);
        }


        public void SetWindow(Window window)
        {
            _window = window;
        }


        private void OnMinimize()
        {
            if (_window != null)
            {
                if (IsNotBusy)
                {
                    IsBusy = true;
                    _window.WindowState = WindowState.Minimized;
                    MinimizeClick?.Invoke(this, EventArgs.Empty);
                    IsBusy = false;
                }
            }
        }

        private void OnMaximize()
        {
            if (_window != null)
            {
                if (IsNotBusy)
                {
                    IsBusy = true;
                    _window.WindowState = _window.WindowState switch
                    {
                        WindowState.Normal => WindowState.Maximized,
                        WindowState.Maximized => WindowState.Normal,
                        _ => _window.WindowState
                    };
                    MaximizeClick?.Invoke(this, EventArgs.Empty);
                    IsBusy = false;
                }
            }
        }

        private void OnClose()
        {
            if (_window != null)
            {
                if (IsNotBusy)
                {
                    IsBusy = true;
                    _window.Close();
                    CloseClick?.Invoke(this, EventArgs.Empty);
                    IsBusy = false;
                }
            }
        }
    }
}
