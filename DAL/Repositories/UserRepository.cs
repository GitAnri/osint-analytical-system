using Shared.Models;
using DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IndividualsDbContext _context;

        public UserRepository(IndividualsDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await Task.CompletedTask;
        }
        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }
    }
}