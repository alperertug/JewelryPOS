using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JewelryPOS.App.Models
{
    public class Cart : INotifyPropertyChanged
    {
        private ObservableCollection<CartItem> _items;

        public ObservableCollection<CartItem> Items
        {
            get => _items;
            set
            {
                // Eski koleksiyonun eventlerini temizle
                if (_items != null)
                {
                    _items.CollectionChanged -= Items_CollectionChanged;
                    foreach (var item in _items)
                    {
                        item.PropertyChanged -= CartItem_PropertyChanged;
                    }
                }

                _items = value;

                // Yeni koleksiyonun eventlerini bağla
                if (_items != null)
                {
                    _items.CollectionChanged += Items_CollectionChanged;
                    foreach (var item in _items)
                    {
                        item.PropertyChanged += CartItem_PropertyChanged;
                    }
                }

                OnPropertyChanged();
                UpdateTotals();
            }
        }

        public decimal TotalPrice => Items.Sum(item => item.Product.Price * item.Quantity);

        public decimal DiscountedTotalPrice => Items.Sum(item => (item.DiscountedPrice ?? item.Product.Price) * item.Quantity);

        public decimal TotalDiscount => Items.Sum(item => item.DiscountedPrice.HasValue ? (item.Product.Price - item.DiscountedPrice.Value) * item.Quantity : 0);

        public Cart()
        {
            _items = new ObservableCollection<CartItem>();
            _items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Koleksiyondan çıkarılan öğelerin eventlerini kaldır
            if (e.OldItems != null)
            {
                foreach (CartItem item in e.OldItems)
                {
                    item.PropertyChanged -= CartItem_PropertyChanged;
                }
            }

            // Koleksiyona eklenen öğelerin eventlerini bağla
            if (e.NewItems != null)
            {
                foreach (CartItem item in e.NewItems)
                {
                    item.PropertyChanged += CartItem_PropertyChanged;
                }
            }

            UpdateTotals();
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Quantity veya DiscountedPrice değiştiğinde toplamları güncelle
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.DiscountedPrice))
            {
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(DiscountedTotalPrice));
            OnPropertyChanged(nameof(TotalDiscount));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}