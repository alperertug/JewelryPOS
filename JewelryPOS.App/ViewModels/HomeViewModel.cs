using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using System;
using System.Linq;
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
        private Cart _cart;

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
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
                if (value) IsPercentageDiscount = false;
                OnPropertyChanged(nameof(IsAmountDiscount));
            }
        }

        public bool IsPercentageDiscount
        {
            get => _isPercentageDiscount;
            set
            {
                _isPercentageDiscount = value;
                if (value) IsAmountDiscount = false;
                OnPropertyChanged(nameof(IsPercentageDiscount));
            }
        }

        public Cart Cart
        {
            get => _cart;
            set
            {
                if (_cart != null)
                {
                    // Eski koleksiyonun olaylarını kaldır
                    _cart.Items.CollectionChanged -= Items_CollectionChanged;
                    foreach (var item in _cart.Items)
                    {
                        item.PropertyChanged -= CartItem_PropertyChanged;
                    }
                }

                _cart = value;

                if (_cart != null)
                {
                    // Yeni koleksiyonun olaylarını ekle
                    _cart.Items.CollectionChanged += Items_CollectionChanged;
                    foreach (var item in _cart.Items)
                    {
                        item.PropertyChanged += CartItem_PropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(Cart));
                _customerViewModel.UpdateCart(_cart); // Cart değiştiğinde CustomerViewModel'i güncelle
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CartItem item in e.OldItems)
                {
                    item.PropertyChanged -= CartItem_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (CartItem item in e.NewItems)
                {
                    item.PropertyChanged += CartItem_PropertyChanged;
                }
            }

            // Döngüsel çağrıyı önlemek için UpdateCart'ı burada çağırmıyoruz
        }

        private void CartItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Toplamlar zaten Cart sınıfında güncellendiği için ek bir işlem gerekmiyor
            _customerViewModel.UpdateCart(Cart);
        }

        public ICommand ReadBarcodeCommand { get; }
        public ICommand ApplyDiscountCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public HomeViewModel(IProductService productService, CustomerViewModel customerViewModel)
        {
            _productService = productService;
            _customerViewModel = customerViewModel;
            Cart = new Cart();

            ReadBarcodeCommand = new RelayCommand<object>(async (_) => await ReadBarcodeAsync());
            ApplyDiscountCommand = new RelayCommand<object>(_ => ApplyDiscount());
            AddToCartCommand = new RelayCommand<object>(_ => AddToCart());
            ClearCartCommand = new RelayCommand<object>(_ => ClearCart());
            CheckoutCommand = new RelayCommand<object>(_ => Checkout());
            RemoveItemCommand = new RelayCommand<CartItem>(RemoveItem);

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

            if (!decimal.TryParse(DiscountInput, out decimal discountValue) || discountValue < 0)
            {
                MessageBox.Show("Geçerli ve pozitif bir sayı giriniz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
        }

        private void AddToCart()
        {
            if (SelectedProduct == null) return;

            // Aynı ürünün zaten eklenip eklenmediğini kontrol et
            var existingItem = Cart.Items.FirstOrDefault(item => item.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.Quantity += 1; // Ürün zaten varsa miktarı artır
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = SelectedProduct,
                    Quantity = 1,
                    DiscountedPrice = SelectedProduct.DiscountPrice
                };
                Cart.Items.Add(cartItem);
            }

            _customerViewModel.UpdateCart(Cart);
        }

        private void RemoveItem(CartItem item)
        {
            if (item != null)
            {
                Cart.Items.Remove(item);
                _customerViewModel.UpdateCart(Cart);
            }
        }

        private void ClearCart()
        {
            Cart = new Cart(); // Yeni bir Cart nesnesi oluştur
            SelectedProduct = null;
            DiscountAmount = 0;
            DiscountInput = "0";
            _customerViewModel.ClearCart();
        }

        private void Checkout()
        {
            MessageBox.Show("Satış işlemi gerçekleştirildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearCart();
        }
    }
}