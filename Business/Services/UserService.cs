using Shared.Helpers;
using Shared.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using DAL.Infrastructure;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _jwtSecret;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtSecret = configuration["JwtSettings:Secret"];
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !PasswordHelper.VerifyPassword(password, user.Salt, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password");

            return user;
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            if (await _userRepository.ExistsByUsernameAsync(username))
                throw new InvalidOperationException("Username already exists");

            var (hash, salt) = PasswordHelper.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hash,
                Salt = salt,
                Role = "User"
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task UpdateRoleAsync(int userId, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Business.Exceptions.NotFoundException("User not found");

            user.Role = newRole;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}