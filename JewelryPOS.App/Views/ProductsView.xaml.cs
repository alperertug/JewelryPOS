using JewelryPOS.App.ViewModels;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryPOS.App.Views
{
    public partial class ProductsView : UserControl
    {
        public ProductsView(ProductsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
