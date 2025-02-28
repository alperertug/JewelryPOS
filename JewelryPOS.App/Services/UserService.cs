using JewelryPOS.App.Data.Interface;
using JewelryPOS.App.Enums;
using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;

namespace JewelryPOS.App.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUser> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _unitOfWork.Repository<ApplicationUser>().FindByUsernameAsync(u => u.Username == username);
            if (existingUser != null)
            {
                throw new Exception("Kullanıcı adı zaten mevcut.");
            }

            var existingEmail = await _unitOfWork.Repository<ApplicationUser>().FindByUsernameAsync(u => u.Email == email);
            if (existingEmail != null)
            {
                throw new Exception("E-posta adresi zaten mevcut.");
            }

            var (passwordHash, passwordSalt) = PasswordHelper.HashPassword(password);

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ApplicationUser>().AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return newUser;
        }

        public async Task<ApplicationUser> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.Repository<ApplicationUser>().FindByUsernameAsync(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı.");
            }

            var isPasswordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı.");
            }

            user.LastLogin = DateTime.UtcNow;
            _unitOfWork.Repository<ApplicationUser>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }
    }
}
