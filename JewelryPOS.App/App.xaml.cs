using JewelryPOS.App.Data;
using JewelryPOS.App.Data.Concrete;
using JewelryPOS.App.Data.Interface;
using JewelryPOS.App.Services;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.ViewModels;
using JewelryPOS.App.ViewModels.Settings;
using JewelryPOS.App.Views;
using JewelryPOS.App.Views.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace JewelryPOS.App
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // **Database Context**
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration
                .GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging());

            // **Repository & UnitOfWork**
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // **Services**
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // **ViewModels**
            services.AddScoped<LoginViewModel>();
            services.AddScoped<RegisterViewModel>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<ProductsViewModel>();
            services.AddScoped<ProductAddViewModel>();
            services.AddScoped<CategoriesViewModel>();
            services.AddScoped<ProductEditViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<HomeViewModel>();
            services.AddScoped<CustomerViewModel>();
            services.AddScoped<UserSettingsViewModel>();

            // **Views**
            services.AddScoped<LoginWindow>();
            services.AddScoped<RegisterWindow>();
            services.AddScoped<MainWindow>();
            services.AddScoped<ProductsView>();
            services.AddScoped<ProductAddView>();
            services.AddScoped<ProductEditView>();
            services.AddScoped<CategoriesView>();
            services.AddScoped<SettingsView>();
            services.AddScoped<HomeView>();
            services.AddScoped<CustomerView>();
            services.AddScoped<UserSettingsView>(provider =>
            {
                var viewModel = provider.GetService<UserSettingsViewModel>();
                return new UserSettingsView(viewModel);
            });
        }
    }
}