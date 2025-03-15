using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.ViewModels.Settings;
using System.Windows.Controls;

namespace JewelryPOS.App.Views.Settings
{
    public partial class UserSettingsView : UserControl
    {
        public UserSettingsView(UserSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            NewPasswordBox.PasswordChanged += (s, e) =>
            {
                viewModel.NewPassword = NewPasswordBox.Password;
                viewModel.ValidatePassword();
            };
            ConfirmPasswordBox.PasswordChanged += (s, e) =>
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
                viewModel.ValidateConfirmPassword();
            };
        }
    }
}