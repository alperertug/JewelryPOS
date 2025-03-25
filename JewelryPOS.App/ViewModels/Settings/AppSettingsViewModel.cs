using Microsoft.Win32;
using System.Windows.Input;
using JewelryPOS.App.Helpers;

namespace JewelryPOS.App.ViewModels.Settings
{
    public class AppSettingsViewModel : BaseViewModel
    {
        private string _customerViewImagePath;

        public string CustomerViewImagePath
        {
            get => _customerViewImagePath;
            set
            {
                _customerViewImagePath = value;
                OnPropertyChanged(nameof(CustomerViewImagePath));
                Properties.Settings.Default.CustomerViewImagePath = value;
                Properties.Settings.Default.Save();
            }
        }

        public ICommand SelectImageCommand { get; }

        public AppSettingsViewModel()
        {
            SelectImageCommand = new RelayCommand<object>(_ => SelectImage());
            CustomerViewImagePath = Properties.Settings.Default.CustomerViewImagePath;
        }

        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CustomerViewImagePath = openFileDialog.FileName;
            }
        }
    }
}