using Microsoft.EntityFrameworkCore;
using DAL.Infrastructure;
using Shared.Models;

namespace DAL.Repositories
{
    public class PhoneNumberRepository : IPhoneNumberRepository
    {
        private readonly IndividualsDbContext _context;

        public PhoneNumberRepository(IndividualsDbContext context) => _context = context;

        public async Task<bool> ExistsAsync(string number, int? excludingIndividualId = null)
        {
            return await _context.PhoneNumbers
                .AnyAsync(p => p.Number == number &&
                               (!excludingIndividualId.HasValue || p.IndividualId != excludingIndividualId.Value));
        }

        public async Task AddAsync(PhoneNumber phoneNumber)
        {
            _context.PhoneNumbers.Add(phoneNumber);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(PhoneNumber phoneNumber)
        {
            _context.PhoneNumbers.Update(phoneNumber);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(PhoneNumber phoneNumber)
        {
            _context.PhoneNumbers.Remove(phoneNumber);
            await Task.CompletedTask;
        }
        public async Task MarkDeletionAsync(PhoneNumber phoneNumber)
        {
            _context.PhoneNumbers.Remove(phoneNumber);
            await Task.CompletedTask;
        }

        public async Task<List<PhoneNumber>> GetByIndividualIdAsync(int individualId)
        {
            return await _context.PhoneNumbers
                .Where(p => p.IndividualId == individualId)
                .ToListAsync();
        }
    }
}