using JewelryPOS.App.ViewModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryPOS.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
        }
    }
}
