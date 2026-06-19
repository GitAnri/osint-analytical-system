using Microsoft.EntityFrameworkCore;
using DAL.Infrastructure;
using Shared.Models;

namespace DAL.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private readonly IndividualsDbContext _context;

        public RelationRepository(IndividualsDbContext context) => _context = context;

        public async Task<bool> ExistsAsync(int individualId, int relatedId, RelationType type) =>
            await _context.Relations
                .AnyAsync(r => r.IndividualId == individualId &&
                               r.RelatedIndividualId == relatedId &&
                               r.RelationType == type);

        public async Task AddAsync(Relation relation)
        {
            _context.Relations.Add(relation);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Relation relation)
        {
            _context.Relations.Remove(relation);
            await Task.CompletedTask;
        }

        public async Task<Relation?> GetAsync(int individualId, int relationId) =>
            await _context.Relations
                .FirstOrDefaultAsync(r => r.IndividualId == individualId && r.Id == relationId);

        public async Task<List<Relation>> GetByIndividualIdAsync(int individualId) =>
            await _context.Relations
                .Where(r => r.IndividualId == individualId)
                .ToListAsync();

        public async Task DeleteByIndividualIdAsync(int individualId)
        {
            var relations = await _context.Relations
                .Where(r => r.IndividualId == individualId || r.RelatedIndividualId == individualId)
                .ToListAsync();

            _context.Relations.RemoveRange(relations);
            await Task.CompletedTask;
        }
        public async Task MarkDeletionByIndividualIdAsync(int individualId)
        {
            var relations = await _context.Relations
                .Where(r => r.IndividualId == individualId || r.RelatedIndividualId == individualId)
                .ToListAsync();

            _context.Relations.RemoveRange(relations);
            await Task.CompletedTask;
        }

        public async Task<List<Individual>> GetAllIndividualsWithRelationsAsync()
        {
            return await _context.Individuals
                .Include(i => i.Relations)
                .ToListAsync();
        }
    }
}