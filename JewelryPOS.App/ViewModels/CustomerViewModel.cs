using JewelryPOS.App.Enums;
using JewelryPOS.App.Models;

namespace JewelryPOS.App.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private string _productName;
        private decimal _originalPrice;
        private decimal? _discountedPrice;
        private Currency _currency;

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(nameof(ProductName)); }
        }

        public decimal OriginalPrice
        {
            get => _originalPrice;
            set { _originalPrice = value; OnPropertyChanged(nameof(OriginalPrice)); }
        }

        public decimal? DiscountedPrice
        {
            get => _discountedPrice;
            set { _discountedPrice = value; OnPropertyChanged(nameof(DiscountedPrice)); }
        }

        public Currency Currency
        {
            get => _currency;
            set { _currency = value; OnPropertyChanged(nameof(Currency)); }
        }

        public void UpdateProduct(Product product)
        {
            ProductName = product.Name;
            OriginalPrice = product.Price;
            DiscountedPrice = product.DiscountPrice > 0 ? product.DiscountPrice : null;
            Currency = product.Currency;
        }

        public void ClearProduct()
        {
            ProductName = "Ürün Seçilmedi";
            OriginalPrice = 0;
            DiscountedPrice = null;
            Currency = Currency.TRY;
        }
    }
}