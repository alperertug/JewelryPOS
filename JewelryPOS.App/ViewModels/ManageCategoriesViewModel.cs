using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace JewelryPOS.App.ViewModels
{
    public class ManageCategoriesViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                ((RelayCommand)EditCategoryCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCategoryCommand).RaiseCanExecuteChanged();
            }
        }

        private string _newCategoryName;
        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                _newCategoryName = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public ManageCategoriesViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            AddCategoryCommand = new RelayCommand(AddCategoryAsync, CanAddCategory);
            EditCategoryCommand = new RelayCommand(EditCategoryAsync, CanEditOrDelete);
            DeleteCategoryCommand = new RelayCommand(DeleteCategoryAsync, CanEditOrDelete);

            LoadCategoriesAsync();
        }

        private void LoadCategoriesAsync()
        {
            try
            {
                var categories = _categoryService.GetQuery().AsNoTracking().Include(c => c.CreatedBy).ToList();
                Categories.Clear();
                foreach (var category in categories)
                {
                    if (category.IsActive)
                    {
                        Categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategoriler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddCategoryAsync(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                MessageBox.Show("Lütfen bir kategori adı giriniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategory = new Category { Name = NewCategoryName, CreatedBy = UserSession.Instance.CurrentUser };
                await _categoryService.AddCategoryAsync(newCategory);
                Categories.Add(newCategory);
                NewCategoryName = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategori eklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditCategoryAsync(object parameter)
        {
            if (SelectedCategory == null) return;

            try
            {
                await _categoryService.UpdateCategoryAsync(SelectedCategory);
                MessageBox.Show("Kategori güncellendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategori güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteCategoryAsync(object parameter)
        {
            if (SelectedCategory == null) return;

            var result = MessageBox.Show("Bu kategoriyi silmek istediğinizden emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            try
            {
                SelectedCategory.IsActive = false;
                await _categoryService.UpdateCategoryAsync(SelectedCategory);
                Categories.Remove(SelectedCategory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategori silinirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddCategory(object parameter) => !string.IsNullOrWhiteSpace(NewCategoryName);
        private bool CanEditOrDelete(object parameter) => SelectedCategory != null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}