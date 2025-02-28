using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Models;
using JewelryPOS.App.Helpers;

namespace JewelryPOS.App.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        public ICommand RegisterCommand { get; }
        public Action CloseAction { get; set; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new RelayCommand<object>(async (_) => await RegisterAsync(), (_) => CanRegister());
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   Password == ConfirmPassword;
        }

        private async Task RegisterAsync()
        {
            try
            {
                var newUser = await _userService.RegisterAsync(Username, Email, Password);
                MessageBox.Show("Kayıt başarılı! Giriş yapabilirsiniz.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
