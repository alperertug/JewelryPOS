using CommunityToolkit.Mvvm.Messaging;
using JewelryPOS.App.Helpers;
using JewelryPOS.App.Messages;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public ICommand AddProductCommand { get; }
        public ICommand ManageCategoriesCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public ProductsViewModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;

            AddProductCommand = new RelayCommand<object>((_) => AddProduct());
            ManageCategoriesCommand = new RelayCommand<object>((_) => ManageCategories());
            EditProductCommand = new RelayCommand<Product>(EditProduct);
            DeleteProductCommand = new RelayCommand<Product>(async (product) => await DeleteProductAsync(product));

            WeakReferenceMessenger.Default.Register<ProductAddedMessage>(this, (recipient, message) =>
            {
                LoadProducts();
            });

            WeakReferenceMessenger.Default.Register<ProductUpdatedMessage>(this, (recipient, message) =>
            {
                LoadProducts();
            });

            WeakReferenceMessenger.Default.Register<CategoryMessage>(this, (recipient, message) =>
            {
                LoadProducts();
            });

            LoadProducts();
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsWithCategoryAsNoTrackingAsync();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürünler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct()
        {
            var productAddView = new ProductAddView
            {
                DataContext = new ProductAddViewModel(_productService, _categoryService)
            };
            productAddView.ShowDialog();
        }

        private void ManageCategories()
        {
            var categoriesView = new CategoriesView
            {
                DataContext = new CategoriesViewModel(_categoryService)
            };
            categoriesView.ShowDialog();
        }

        private void EditProduct(Product product)
        {
            if (product == null) return;

            var productEditView = new ProductEditView
            {
                DataContext = new ProductEditViewModel(_productService, _categoryService, product)
            };
            productEditView.ShowDialog();

            LoadProducts();
        }

        private async Task DeleteProductAsync(Product product)
        {
            if (product == null) return;

            var result = MessageBox.Show($"'{product.Name}' adlı ürünü silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var productInDb = await _productService.GetProductByIdAsync(product.Id);
                    productInDb.IsActive = false;
                    await _productService.UpdateProductAsync(productInDb);
                    Products.Remove(product);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ürün silinirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        ~ProductsViewModel()
        {
            WeakReferenceMessenger.Default.Unregister<ProductAddedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ProductUpdatedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<CategoryMessage>(this);
        }
    }
}