using CommunityToolkit.Mvvm.Messaging;
using JewelryPOS.App.Enums;
using JewelryPOS.App.Helpers;
using JewelryPOS.App.Messages;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class AddProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public Product NewProduct { get; set; } = new Product();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<Currency> Currencies { get; set; } = new ObservableCollection<Currency>();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (_selectedCategory != null)
                {
                    NewProduct.CategoryId = _selectedCategory.Id;
                }
                OnPropertyChanged();
            }
        }

        private Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                NewProduct.Currency = _selectedCurrency;
                OnPropertyChanged();
            }
        }

        public ICommand SaveProductCommand { get; }
        public ICommand CancelCommand { get; }

        public AddProductViewModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;

            SaveProductCommand = new RelayCommand<object>(SaveProduct);
            CancelCommand = new RelayCommand<object>(Cancel);

            LoadCategoriesAsync();
            LoadCurrencies();
        }

        private async void LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                Categories.Clear();

                var dummyCategory = new Category { Id = Guid.Empty, Name = "Lütfen Kategori Seçiniz" };
                Categories.Add(dummyCategory);

                foreach (var category in categories)
                {
                    if (category.IsActive)
                    {
                        Categories.Add(category);
                    }
                }

                SelectedCategory = dummyCategory;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategoriler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCurrencies()
        {
            Currencies.Clear();
            foreach (Currency currency in Enum.GetValues(typeof(Currency)))
            {
                Currencies.Add(currency);
            }
            SelectedCurrency = Currency.TRY;
        }

        private async void SaveProduct(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewProduct.Name) || NewProduct.Price <= 0 || NewProduct.Stock < 0 ||
                SelectedCategory == null || SelectedCategory.Id == Guid.Empty)
            {
                MessageBox.Show("Lütfen tüm zorunlu alanları doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewProduct.CreatedBy = UserSession.Instance.CurrentUser;
            NewProduct.CreatedAt = DateTime.UtcNow;
            NewProduct.IsActive = true;

            try
            {
                await _productService.AddProductAsync(NewProduct);
                MessageBox.Show("Ürün başarıyla eklendi!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                WeakReferenceMessenger.Default.Send(new ProductAddedMessage(NewProduct));
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün eklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Application.Current.Windows.OfType<AddProductView>().FirstOrDefault()?.Close();
        }
    }
}