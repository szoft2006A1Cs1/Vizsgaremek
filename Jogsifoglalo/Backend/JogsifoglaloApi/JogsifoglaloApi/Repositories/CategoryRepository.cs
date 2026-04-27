using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly JogsifoglaloContext _context;

        public CategoryRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.Include(c => c.Instructs).FirstOrDefaultAsync(c => c.Kategoria_Id == id);

            if (category == null)
            {
                return false;
            }

            if (category.Instructs != null && category.Instructs.Any())
            {
                throw new InvalidOperationException("A kategória nem törölhető, mert oktatók vannak hozzárendelve!");
            }

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
