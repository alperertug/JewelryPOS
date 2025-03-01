﻿using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class EditProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public Product CurrentProduct { get; set; }
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (_selectedCategory != null)
                {
                    CurrentProduct.CategoryId = _selectedCategory.Id;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SaveProductCommand { get; }
        public ICommand CancelCommand { get; }

        public EditProductViewModel(IProductService productService, ICategoryService categoryService, Product product)
        {
            _productService = productService;
            _categoryService = categoryService;
            CurrentProduct = product;

            SaveProductCommand = new RelayCommand<object>(SaveProduct);
            CancelCommand = new RelayCommand<object>(Cancel);

            LoadCategoriesAsync();
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == CurrentProduct.CategoryId);
        }

        private async void LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesForComboBoxesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CurrentProduct.CategoryId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategoriler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveProduct(object parameter)
        {
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name) || CurrentProduct.Price <= 0 || CurrentProduct.Stock < 0 || SelectedCategory == null)
            {
                MessageBox.Show("Lütfen tüm zorunlu alanları doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var trackedProduct = await _productService.GetProductByIdAsync(CurrentProduct.Id);
                if (trackedProduct != null)
                {
                    trackedProduct.Name = CurrentProduct.Name;
                    trackedProduct.Price = CurrentProduct.Price;
                    trackedProduct.DiscountPrice = CurrentProduct.DiscountPrice;
                    trackedProduct.Stock = CurrentProduct.Stock;
                    trackedProduct.Karat = CurrentProduct.Karat;
                    trackedProduct.Weight = CurrentProduct.Weight;
                    trackedProduct.Description = CurrentProduct.Description;
                    trackedProduct.Barcode = CurrentProduct.Barcode;
                    trackedProduct.CategoryId = SelectedCategory.Id;
                    await _productService.UpdateProductAsync(trackedProduct);
                }
                MessageBox.Show("Ürün başarıyla güncellendi!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Application.Current.Windows.OfType<EditProductView>().FirstOrDefault()?.Close();
        }
    }
}