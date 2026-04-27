using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class InstructRepository : IInstructRepository
    {
        private readonly JogsifoglaloContext _context;

        public InstructRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instruct>> GetByInstructorIdAsync(int oktatId)
        {
            return await _context.Instructs.Where(o => o.Oktat_Id == oktatId).ToListAsync();
        }

        public async Task AddAsync(Instruct instruct)
        {
            await _context.Instructs.AddAsync(instruct);
        } 

        public void Delete(Instruct instruct)
        {
            _context.Instructs.Remove(instruct);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
