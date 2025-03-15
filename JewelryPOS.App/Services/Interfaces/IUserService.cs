using JewelryPOS.App.Models;

namespace JewelryPOS.App.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterAsync(string username, string email, string password);
        Task<ApplicationUser> LoginAsync(string username, string password);
        Task<bool> IsUsernameTakenAsync(string username, Guid excludeId);
        Task<bool> IsEmailTakenAsync(string email, Guid excludeId);
        bool VerifyPassword(ApplicationUser user, string password);
        Task UpdateUserAsync(ApplicationUser user);
        string GenerateSalt();
        string HashPassword(string password, string salt);
    }
}
