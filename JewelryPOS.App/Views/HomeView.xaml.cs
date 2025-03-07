using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace JewelryPOS.App.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView(CustomerViewModel customerViewModel)
        {
            InitializeComponent();
            DataContext = new HomeViewModel(App.ServiceProvider.GetRequiredService<IProductService>(), customerViewModel);
        }
    }
}
