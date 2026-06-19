using Shared.Models;

namespace Business.Services
{
    public interface IUserService
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password);
        Task UpdateRoleAsync(int userId, string newRole);
    }
}
