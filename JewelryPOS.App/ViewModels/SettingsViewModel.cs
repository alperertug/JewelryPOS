using JewelryPOS.App.Helpers;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private object _currentContent;
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

        public SettingsViewModel()
        {
            ShowUserSettingsCommand = new RelayCommand<object>((_) => ShowUserSettings());
            ShowAppSettingsCommand = new RelayCommand<object>((_) => ShowAppSettings());

            // Varsayılan olarak Kullanıcı Ayarları açılır
            ShowUserSettings();
        }

        private void ShowUserSettings()
        {
            CurrentContent = new UserSettingsViewModel(); // Kullanıcı Ayarları içeriği
        }

        private void ShowAppSettings()
        {
            CurrentContent = new AppSettingsViewModel(); // Uygulama Ayarları içeriği
        }
    }

    // Örnek Kullanıcı Ayarları ViewModel
    public class UserSettingsViewModel : BaseViewModel
    {
        public string UserSettingsMessage { get; } = "Burası Kullanıcı Ayarları bölümü.";
    }

    // Örnek Uygulama Ayarları ViewModel
    public class AppSettingsViewModel : BaseViewModel
    {
        public string AppSettingsMessage { get; } = "Burası Uygulama Ayarları bölümü.";
    }
}