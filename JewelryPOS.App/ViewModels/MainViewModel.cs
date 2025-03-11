using JewelryPOS.App.Helpers;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private string _loggedInUser;
        private CustomerView _customerView;
        private readonly CustomerViewModel _customerViewModel;

        public ICommand OpenHomeCommand { get; }
        public ICommand OpenProductsCommand { get; }
        public ICommand OpenSalesCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SwitchUserCommand { get; }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private object _currentView;

        public string LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
            }
        }

        public MainViewModel(IProductService productService)
        {
            _productService = productService;
            _customerViewModel = new CustomerViewModel();

            OpenHomeCommand = new RelayCommand<object>((_) => CurrentView = new HomeView(_customerViewModel));
            OpenProductsCommand = new RelayCommand<object>((_) => CurrentView = App.ServiceProvider.GetRequiredService<ProductsView>());
            OpenSalesCommand = new RelayCommand<object>((_) => CurrentView = new SalesViewModel());
            OpenSettingsCommand = new RelayCommand<object>((_) => CurrentView = new SettingsWindow());
            LogoutCommand = new RelayCommand<object>((_) => Logout());
            SwitchUserCommand = new RelayCommand<object>((_) => SwitchUser());

            UpdateLoggedInUser();
            UserSession.Instance.UserChanged += (s, e) => UpdateLoggedInUser();

            CurrentView = new HomeView(_customerViewModel);

            OpenCustomerViewOnSecondaryScreen();
        }

        private void UpdateLoggedInUser()
        {
            LoggedInUser = UserSession.Instance.CurrentUser?.Username ?? "Misafir";
        }

        private void Logout()
        {
            UserSession.Instance.ClearUser();
            Application.Current.Shutdown();
        }

        private void SwitchUser()
        {
            UserSession.Instance.ClearUser();

            var loginWindow = new LoginWindow();
            loginWindow.Show();

            Application.Current.Windows.OfType<CustomerView>().FirstOrDefault()?.Close();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }

        private void OpenCustomerViewOnSecondaryScreen()
        {
            var monitors = MonitorHelper.GetMonitors();
            if (monitors.Count > 1 && monitors.Any(m => !m.Primary))
            {
                var secondary = monitors.First(m => !m.Primary);
                var customerView = new CustomerView();
                customerView.DataContext = _customerViewModel;
                customerView.Left = secondary.WorkArea.Left;
                customerView.Top = secondary.WorkArea.Top;
                customerView.Width = secondary.WorkArea.Width;
                customerView.Height = secondary.WorkArea.Height;
                customerView.Show();
            }
            else
            {
                MessageBox.Show("İkincil ekran bulunamadı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }
}