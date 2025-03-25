using JewelryPOS.App.Helpers;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.ViewModels.Settings;
using JewelryPOS.App.Views.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private object _currentContent;
        private readonly IUserService _userService;

        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public ICommand ShowUserSettingsCommand { get; }
        public ICommand ShowAppSettingsCommand { get; }

        public SettingsViewModel(IUserService userService)
        {
            _userService = userService;
            ShowUserSettingsCommand = new RelayCommand<object>((_) => ShowUserSettings());
            ShowAppSettingsCommand = new RelayCommand<object>((_) => ShowAppSettings());

            // Varsayılan olarak Kullanıcı Ayarları açılır
            ShowUserSettings();
        }

        private void ShowUserSettings()
        {
            // DI container üzerinden UserSettingsViewModel al
            var userSettingsViewModel = App.ServiceProvider.GetService<UserSettingsViewModel>();
            var userSettingsView = new UserSettingsView(userSettingsViewModel);
            CurrentContent = userSettingsView;
        }

        private void ShowAppSettings()
        {
            var appSettingsView = new AppSettingsView();
            appSettingsView.DataContext = new AppSettingsViewModel();
            CurrentContent = appSettingsView;
        }
    }
}