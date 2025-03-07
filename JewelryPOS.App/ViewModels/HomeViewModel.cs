using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly CustomerViewModel _customerViewModel;
        private Product _selectedProduct;
        private string _barcode;
        private decimal _discountAmount;

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
                UpdateCustomerView();
            }
        }

        public string Barcode
        {
            get => _barcode;
            set
            {
                _barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                _discountAmount = value;
                OnPropertyChanged(nameof(DiscountAmount));
                ApplyDiscount();
            }
        }

        public ICommand ReadBarcodeCommand { get; }

        public HomeViewModel(IProductService productService, CustomerViewModel customerViewModel)
        {
            _productService = productService;
            _customerViewModel = customerViewModel;

            ReadBarcodeCommand = new RelayCommand<object>(async (_) => await ReadBarcodeAsync());
        }

        private async Task ReadBarcodeAsync()
        {
            if (string.IsNullOrWhiteSpace(Barcode)) return;

            try
            {
                var product = await _productService.GetProductByBarcodeAsync(Barcode);
                if (product != null)
                {
                    SelectedProduct = product;
                    DiscountAmount = 0;
                }
                else
                {
                    MessageBox.Show("Ürün bulunamadı.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Barkod okunurken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyDiscount()
        {
            if (SelectedProduct != null)
            {
                SelectedProduct.DiscountPrice = SelectedProduct.Price - DiscountAmount;
                UpdateCustomerView();
            }
        }

        private void UpdateCustomerView()
        {
            if (SelectedProduct != null)
            {
                _customerViewModel.UpdateProduct(SelectedProduct);
            }
            else
            {
                _customerViewModel.ClearProduct();
            }
        }
    }
}