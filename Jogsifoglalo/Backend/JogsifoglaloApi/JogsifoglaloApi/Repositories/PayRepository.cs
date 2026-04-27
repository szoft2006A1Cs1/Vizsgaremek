using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class PayRepository : IPayRepository
    {
        private readonly JogsifoglaloContext _context;

        public PayRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pay pay)
        {
            await _context.Pays.AddAsync(pay);
        }

        public async Task<IEnumerable<Pay>> GetByUserIdAsync(int felhasznaloId)
        {
            return await _context.Pays.Where(p => p.Felhasznalo_Id == felhasznaloId).OrderByDescending(p => p.Datum).ToListAsync();
        }

        public async Task<Pay?> GetByIdAsync(int id)
        {
            return await _context.Pays.FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
