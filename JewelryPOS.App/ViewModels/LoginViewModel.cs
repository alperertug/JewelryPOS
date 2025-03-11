using JewelryPOS.App.Helpers;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private string _username;
        private string _password;
        private string _errorMessage;
        public Action CloseAction { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand OpenForgotPasswordCommand { get; set; }
        public ICommand OpenRegisterWindowCommand { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public LoginViewModel(IUserService userService, IProductService productService)
        {
            _userService = userService;
            _productService = productService;
            LoginCommand = new RelayCommand<object>((_) => _ = LoginAsync());
            OpenForgotPasswordCommand = new RelayCommand<object>((_) => OpenForgotPasswordWindow());
            OpenRegisterWindowCommand = new RelayCommand<object>((_) => OpenRegisterWindow());
        }

        private async Task LoginAsync()
        {
            try
            {
                var user = await _userService.LoginAsync(Username, Password);

                if (user != null)
                {
                    UserSession.Instance.SetUser(user);
                    MainWindow mainWindow = new MainWindow(_productService);
                    mainWindow.Show();

                    Application.Current.Windows[0]?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Giriş Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = ex.Message;
            }
        }

        private void OpenForgotPasswordWindow()
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }

        private void OpenRegisterWindow()
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
