using JewelryPOS.App.Data.Interface;
using JewelryPOS.App.Enums;
using JewelryPOS.App.Helpers;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<bool> IsUsernameTakenAsync(string username, Guid excludeId)
        {
            var existingUser = await _unitOfWork.Repository<ApplicationUser>()
                .FindByUsernameAsync(u => u.Username == username && u.Id != excludeId);
            return existingUser != null;
        }

        public async Task<bool> IsEmailTakenAsync(string email, Guid excludeId)
        {
            var existingEmail = await _unitOfWork.Repository<ApplicationUser>()
                .FindByUsernameAsync(u => u.Email == email && u.Id != excludeId);
            return existingEmail != null;
        }

        public bool VerifyPassword(ApplicationUser user, string password)
        {
            return PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            _unitOfWork.Repository<ApplicationUser>().Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public string GenerateSalt()
        {
            using (var hmac = new HMACSHA512())
            {
                return Convert.ToBase64String(hmac.Key);
            }
        }

        public string HashPassword(string password, string salt)
        {
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = Convert.FromBase64String(salt);
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
    }
}