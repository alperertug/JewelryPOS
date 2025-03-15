using JewelryPOS.App.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace JewelryPOS.App.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
        }
    }
}
