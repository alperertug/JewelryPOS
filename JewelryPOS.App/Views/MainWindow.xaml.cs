using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.ViewModels;
using System.Windows;

namespace JewelryPOS.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IProductService _productService;

        public MainWindow(IProductService productService)
        {
            _productService = productService;
            InitializeComponent();
            DataContext = new MainViewModel(_productService);
        }
    }
}
