using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels.Settings
{
    public class UserSettingsViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private ApplicationUser _currentUser;

        private string _username;
        private string _email;
        private string _newPassword;
        private string _confirmPassword;
        private string _usernameError;
        private string _emailError;
        private string _passwordError;
        private string _confirmPasswordError;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                ValidatePassword();
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                ValidateConfirmPassword();
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string UsernameError
        {
            get => _usernameError;
            set { _usernameError = value; OnPropertyChanged(nameof(UsernameError)); }
        }

        public string EmailError
        {
            get => _emailError;
            set { _emailError = value; OnPropertyChanged(nameof(EmailError)); }
        }

        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(nameof(PasswordError)); }
        }

        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set { _confirmPasswordError = value; OnPropertyChanged(nameof(ConfirmPasswordError)); }
        }

        public ICommand SaveChangesCommand { get; }

        public UserSettingsViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            SaveChangesCommand = new RelayCommand<object>(async _ => await SaveChangesAsync());

            _currentUser = UserSession.Instance.CurrentUser;
            Username = _currentUser.Username;
            Email = _currentUser.Email;
        }

        private async Task SaveChangesAsync()
        {
            var passwordDialog = new PasswordDialog();
            if (passwordDialog.ShowDialog() != true)
            {
                return;
            }

            string enteredPassword = passwordDialog.EnteredPassword;

            if (!_userService.VerifyPassword(_currentUser, enteredPassword))
            {
                MessageBox.Show("Mevcut şifre yanlış.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClearErrors();

            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Kullanıcı adı boş olamaz.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "E-posta boş olamaz.";
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (NewPassword.Length < 8)
                {
                    PasswordError = "Şifre en az 8 karakter olmalıdır.";
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    ConfirmPasswordError = "Şifreler uyuşmuyor.";
                    return;
                }
            }

            if (await _userService.IsUsernameTakenAsync(Username, _currentUser.Id))
            {
                UsernameError = "Bu kullanıcı adı zaten kullanılıyor.";
                return;
            }

            if (await _userService.IsEmailTakenAsync(Email, _currentUser.Id))
            {
                EmailError = "Bu e-posta adresi zaten kullanılıyor.";
                return;
            }

            _currentUser.Username = Username;
            _currentUser.Email = Email;

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                var salt = _userService.GenerateSalt();
                _currentUser.PasswordSalt = salt;
                _currentUser.PasswordHash = _userService.HashPassword(NewPassword, salt);
            }

            await _userService.UpdateUserAsync(_currentUser);
            UserSession.Instance.SetUser(_currentUser);
            ClearTextBoxes();
            MessageBox.Show("Değişiklikler başarıyla kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearErrors()
        {
            UsernameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
        }

        private void ClearTextBoxes()
        {
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        public void ValidatePassword()
        {
            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword.Length < 8)
            {
                PasswordError = "Şifre en az 8 karakter olmalıdır.";
            }
            else
            {
                PasswordError = string.Empty;
            }
        }

        public void ValidateConfirmPassword()
        {
            if (!string.IsNullOrWhiteSpace(ConfirmPassword) && !string.IsNullOrWhiteSpace(NewPassword) && NewPassword != ConfirmPassword)
            {
                ConfirmPasswordError = "Şifreler uyuşmuyor.";
            }
            else
            {
                ConfirmPasswordError = string.Empty;
            }
        }
    }
}