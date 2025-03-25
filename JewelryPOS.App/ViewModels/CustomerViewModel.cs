using JewelryPOS.App.Models;

namespace JewelryPOS.App.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private Cart _cart;
        public string CustomerViewImagePath => Properties.Settings.Default.CustomerViewImagePath;

        public CustomerViewModel()
        {
            Properties.Settings.Default.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Properties.Settings.Default.CustomerViewImagePath))
                {
                    OnPropertyChanged(nameof(CustomerViewImagePath));
                }
            };
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
        }

        private void CartItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Toplamlar zaten Cart sınıfında güncellendiği için ek bir işlem gerekmiyor
        }

        public void UpdateCart(Cart cart)
        {
            if (cart != _cart) // Döngüsel çağrıyı önlemek için kontrol
            {
                Cart = cart;
            }
        }

        public void ClearCart()
        {
            Cart = new Cart(); // Yeni bir Cart nesnesi oluştur
        }
    }
}