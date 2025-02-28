using JewelryPOS.App.Models;

namespace JewelryPOS.App.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterAsync(string username, string email, string password);
        Task<ApplicationUser> LoginAsync(string username, string password);
    }
}
