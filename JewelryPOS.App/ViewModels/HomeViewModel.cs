using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using System;
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
        private bool _isAmountDiscount;
        private bool _isPercentageDiscount;
        private string _discountInput;

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
            }
        }

        public string DiscountInput
        {
            get => _discountInput;
            set
            {
                _discountInput = value;
                OnPropertyChanged(nameof(DiscountInput));
            }
        }

        public bool IsAmountDiscount
        {
            get => _isAmountDiscount;
            set
            {
                _isAmountDiscount = value;
                if (value)
                {
                    IsPercentageDiscount = false;
                }
                OnPropertyChanged(nameof(IsAmountDiscount));
            }
        }

        public bool IsPercentageDiscount
        {
            get => _isPercentageDiscount;
            set
            {
                _isPercentageDiscount = value;
                if (value)
                {
                    IsAmountDiscount = false;
                }
                OnPropertyChanged(nameof(IsPercentageDiscount));
            }
        }

        public ICommand ReadBarcodeCommand { get; }
        public ICommand ApplyDiscountCommand { get; }
        public ICommand ClearCustomerViewCommand { get; }

        public HomeViewModel(IProductService productService, CustomerViewModel customerViewModel)
        {
            _productService = productService;
            _customerViewModel = customerViewModel;

            ReadBarcodeCommand = new RelayCommand<object>(async (_) => await ReadBarcodeAsync());
            ApplyDiscountCommand = new RelayCommand<object>(_ => ApplyDiscount());
            ClearCustomerViewCommand = new RelayCommand<object>(_ => ClearCustomerView());

            IsAmountDiscount = true;
            IsPercentageDiscount = false;
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
                    SelectedProduct.DiscountPrice = null;
                    DiscountAmount = 0;
                    DiscountInput = "0";
                    OnPropertyChanged(nameof(SelectedProduct));
                    UpdateCustomerView();
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
            if (SelectedProduct == null) return;

            if (!decimal.TryParse(DiscountInput, out decimal discountValue))
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (discountValue < 0)
            {
                MessageBox.Show("İndirim değeri negatif olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsPercentageDiscount)
            {
                if (discountValue < 1 || discountValue > 100)
                {
                    MessageBox.Show("Yüzde indirim 1 ile 100 arasında olmalıdır.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DiscountAmount = (SelectedProduct.Price * discountValue) / 100;
            }
            else
            {
                DiscountAmount = discountValue;
            }

            if (DiscountAmount >= SelectedProduct.Price)
            {
                MessageBox.Show("İndirim, ürün fiyatından büyük olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedProduct.DiscountPrice = SelectedProduct.Price - DiscountAmount;
            OnPropertyChanged(nameof(SelectedProduct));
            UpdateCustomerView();
        }

        private void ClearCustomerView()
        {
            SelectedProduct = null;
            DiscountAmount = 0;
            DiscountInput = "0";
            _customerViewModel.ClearProduct();
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