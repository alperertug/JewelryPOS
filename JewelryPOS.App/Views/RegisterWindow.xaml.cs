using JewelryPOS.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace JewelryPOS.App.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterWindow()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<RegisterViewModel>();
            _viewModel.CloseAction = new Action(() => this.Close());
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = ((PasswordBox)sender).Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}
