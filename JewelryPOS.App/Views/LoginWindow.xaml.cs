using JewelryPOS.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace JewelryPOS.App.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            _viewModel.CloseAction = new Action(() => this.Close());
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

    }
}
